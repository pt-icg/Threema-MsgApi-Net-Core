using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi
{
	public static class DataUtils
	{
		private const int BUFFER_SIZE = 16384;

		/// <summary>
		/// Convert a byte array into a hexadecimal string (lowercase).
		/// </summary>
		/// <param name="bytes">the bytes to encode</param>
		/// <returns>hex encoded string</returns>
		public static string ByteArrayToHexString(byte[] bytes)
		{
			var hex = BitConverter.ToString(bytes);
			return hex.Replace("-", "");
		}

		/// <summary>
		/// Convert a string in hexadecimal representation to a byte array.
		/// </summary>
		/// <param name="s">hex string</param>
		/// <returns>decoded byte array</returns>
		public static byte[] HexStringToByteArray(string s)
		{
			string sc = s.Replace("[^0-9a-fA-F]", "");
			int len = sc.Length;
			byte[] data = new byte[len / 2];
			for (int i = 0; i < len; i += 2)
			{
				System.Diagnostics.Debug.WriteLine(sc.Substring(i, 2));
				data[i / 2] = Convert.ToByte(sc.Substring(i, 2), 16);
			}
			return data;
		}

		/// <summary>
		/// UTF8 encoded string.
		/// </summary>
		/// <param name="value">string to encode</param>
		/// <returns>encoded string</returns>
		public static string Utf8Endcode(string value)
		{
			return Encoding.UTF8.GetString(Encoding.Default.GetBytes(value));
		}

		/// <summary>
		/// Read hexadecimal data from a file and return it as a byte array.
		/// </summary>
		/// <param name="file">input file</param>
		/// <returns>the decoded data</returns>
		public static byte[] ReadHexFile(string file)
		{
			byte[] data = null;

			using (FileStream stream = File.OpenRead(file))
			using (StreamReader reader = new StreamReader(stream))
			{
				data = HexStringToByteArray(reader.ReadLine().Trim());
				reader.Close();
			}

			return data;
		}

		/// <summary>
		/// Read an encoded key from a file and return it as a key instance.
		/// </summary>
		/// <param name="file">input file</param>
		/// <returns>the decoded key</returns>
		public static Key ReadKeyFile(string file)
		{
			return Key.DecodeKey(ReadLineFromFile(file));
		}

		/// <summary>
		/// Read an encoded key from a file and return it as a key instance.
		/// </summary>
		/// <param name="file">input file</param>
		/// <param name="expectedKeyType">validates the key type (private or public)</param>
		/// <returns>the decoded key</returns>
		public static Key ReadKeyFile(string file, string expectedKeyType)
		{
			return Key.DecodeKey(ReadLineFromFile(file), expectedKeyType);
		}

		/// <summary>
		/// Wirte stream data to byte array.
		/// </summary>
		/// <param name="stream">data write to byte array</param>
		/// <param name="progressListener">progress</param>
		/// <returns>bytes from stream</returns>
		public static byte[] StreamToBytes(Stream stream, IProgressListener progressListener)
		{
			if (stream == null)
			{
				throw new ArgumentNullException("stream must not be null.");
			}

			byte[] bytes;

			// Content length known?
			if (stream.CanSeek)
			{
				bytes = new byte[stream.Length];
				int bytesRead = 0;
				int offset = 0;

				//reader.Read
				while (offset < stream.Length && (bytesRead = stream.Read(bytes, offset, (int)(bytes.Length - offset))) > 0)
				{
					offset += bytesRead;

					if (progressListener != null)
					{
						progressListener.updateProgress((int)(100 * offset / bytes.Length));
					}
				}

				if (offset != (int)bytes.Length)
				{
					throw new IOException("Unexpected read size. current: " + offset + ", excepted: " + bytes.Length);
				}
			}
			else
			{
				// Content length is unknown - need to read until EOF
				byte[] buffer = new byte[BUFFER_SIZE];

				using (MemoryStream outputStream = new MemoryStream())
				{
					try
					{
						//int offset = 0;
						while (true)
						{
							int bytesRead = stream.Read(buffer, 0, buffer.Length);
							if (bytesRead == 0)
							{
								break;
							}
							outputStream.Write(buffer, 0, bytesRead);
						}
					}
					catch (ArgumentOutOfRangeException)
					{
					}

					outputStream.Position = 0;
					bytes = new byte[outputStream.Length];
					outputStream.Read(bytes, 0, bytes.Length);
				}
			}

			return bytes;
		}

		/// <summary>
		/// Write a byte array into a file in hexadecimal format.
		/// </summary>
		/// <param name="file">output file</param>
		/// <param name="data">the data to be written</param>
		public static void WriteHexFile(string file, byte[] data)
		{
			using (FileStream stream = File.OpenWrite(file))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.Write(ByteArrayToHexString(data));
				writer.Write('\n');
				writer.Close();
			}
		}

		/// <summary>
		/// Write an encoded key to a file
		/// Encoded key format: type:hex_key.
		/// </summary>
		/// <param name="file">output file</param>
		/// <param name="key">a key that will be encoded and written to a file</param>
		public static void WriteKeyFile(string file, Key key)
		{
			using (FileStream stream = File.OpenWrite(file))
			using (StreamWriter writer = new StreamWriter(stream))
			{
				writer.Write(key.Encode());
				writer.Write('\n');
			}
		}

		private static string ReadLineFromFile(string file)
		{
			string data = null;

			using (FileStream stream = File.OpenRead(file))
			using (StreamReader reader = new StreamReader(stream))
			{
				data = reader.ReadLine().Trim();
				reader.Close();
			}

			return data;
		}
	}
}
