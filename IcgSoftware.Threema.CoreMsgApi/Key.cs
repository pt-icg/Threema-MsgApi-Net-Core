using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;

namespace IcgSoftware.Threema.CoreMsgApi
{
	/// <summary>
	/// Encapsulates an asymmetric key, either public or private.
	/// </summary>
	public class Key
	{
		public static readonly char separator = ':';

		public static class KeyType
		{
			public const string PRIVATE = "private";
			public const string PUBLIC = "public";
		}

		public byte[] key;
		public string type;

		public Key(string type, byte[] key)
		{
			this.key = key;
			this.type = type;
		}


		/// <summary>
		/// Decodes and validates an encoded key.
		/// Encoded key format: type:hex_key
		/// </summary>
		/// <param name="encodedKey">an encoded key</param>
		/// <returns></returns>
		public static Key DecodeKey(string encodedKey)
		{
			// Split key and check length
			string[] keyArray = encodedKey.Split(Key.separator);
			if (keyArray.Length != 2)
			{
				throw new InvalidKeyException("Does not contain a valid key format");
			}

			// Unpack key
			string keyType = keyArray[0];
			string keyContent = keyArray[1];

			// Is this a valid hex key?
			if (!Regex.IsMatch(keyContent, "[0-9a-fA-F]{64}"))
			{
				throw new InvalidKeyException("Does not contain a valid key");
			}

			return new Key(keyType, DataUtils.HexStringToByteArray(keyContent));
		}

		/// <summary>
		/// Decodes and validates an encoded key.
		/// Encoded key format: type:hex_key
		/// </summary>
		/// <param name="encodedKey">an encoded key</param>
		/// <param name="expectedKeyType">the expected type of the key</param>
		/// <returns></returns>
		public static Key DecodeKey(String encodedKey, String expectedKeyType)
		{
			Key key = DecodeKey(encodedKey);

			// Check key type
			if (!key.type.Equals(expectedKeyType))
			{
				throw new InvalidKeyException("Expected key type: " + expectedKeyType + ", got: " + key.type);
			}

			return key;
		}

		/// <summary>
		/// Encodes a key.
		/// </summary>
		/// <returns>an encoded key</returns>
		public String Encode()
		{
			return this.type + Key.separator + DataUtils.ByteArrayToHexString(this.key);
		}
	}
}
