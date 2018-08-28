using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;

namespace IcgSoftware.Threema.CoreMsgApi.Messages
{
	class FileMessage : ThreemaMessage
	{
		public const int TYPE_CODE = 0x17;

		private const string KEY_BLOB_ID = "b";
		private const string KEY_THUMBNAIL_BLOB_ID = "t";
		private const string KEY_ENCRYPTION_KEY = "k";
		private const string KEY_MIME_TYPE = "m";
		private const string KEY_FILE_NAME = "n";
		private const string KEY_FILE_SIZE = "s";
		private const string KEY_TYPE = "i";

		private readonly byte[] blobId;
		private readonly byte[] encryptionKey;
		private readonly string mimeType;
		private readonly string fileName;
		private readonly int fileSize;
		private readonly byte[] thumbnailBlobId;

		public FileMessage(byte[] blobId, byte[] encryptionKey, String mimeType, String fileName, int fileSize, byte[] thumbnailBlobId)
		{
			this.blobId = blobId;
			this.encryptionKey = encryptionKey;
			this.mimeType = mimeType;
			this.fileName = fileName;
			this.fileSize = fileSize;
			this.thumbnailBlobId = thumbnailBlobId;
		}

		public byte[] BlobId
		{
			get { return this.blobId; }
		}

		public byte[] EncryptionKey
		{
			get { return this.encryptionKey; }
		}

		public string MimeType
		{
			get { return this.mimeType; }
		}

		public string FileName
		{
			get { return this.fileName; }
		}

		public int FileSize
		{
			get { return this.fileSize; }
		}

		public byte[] ThumbnailBlobId
		{
			get { return this.thumbnailBlobId; }
		}

		public override int GetTypeCode()
		{
			return TYPE_CODE;
		}

		public override string ToString()
		{
			return string.Format("file message {0}", this.fileName);
		}

		public override byte[] GetData()
		{
			JObject jo = new JObject();
			try
			{
				jo.Add(KEY_BLOB_ID, JToken.FromObject(DataUtils.ByteArrayToHexString(this.blobId)));
				if (this.thumbnailBlobId != null)
				{
					jo.Add(KEY_THUMBNAIL_BLOB_ID, JToken.FromObject(DataUtils.ByteArrayToHexString(this.thumbnailBlobId)));
				}
				jo.Add(KEY_ENCRYPTION_KEY, JToken.FromObject(DataUtils.ByteArrayToHexString(this.encryptionKey)));
				jo.Add(KEY_MIME_TYPE, JToken.FromObject(this.mimeType));
				jo.Add(KEY_FILE_NAME, JToken.FromObject(this.fileName));
				jo.Add(KEY_FILE_SIZE, JToken.FromObject(this.fileSize));
				jo.Add(KEY_TYPE, JToken.FromObject(0));
			}
			catch (Exception)
			{
				throw new BadMessageException();
			}

			return Encoding.UTF8.GetBytes(jo.ToString());
		}

		public static FileMessage FromString(string json)
		{
			try
			{
				JObject jo = JObject.Parse(json);
				byte[] encryptionKey = DataUtils.HexStringToByteArray(jo[KEY_ENCRYPTION_KEY].Value<string>());
				string mimeType = jo[KEY_MIME_TYPE].Value<string>();
				int fileSize = jo[KEY_FILE_SIZE].Value<int>();
				byte[] blobId = DataUtils.HexStringToByteArray(jo[KEY_BLOB_ID].Value<string>());

				string fileName;
				byte[] thumbnailBlobId = null;

				//optional field
				if (jo.Children().Any(e => e.Path.EndsWith(KEY_THUMBNAIL_BLOB_ID)))
				{
					thumbnailBlobId = DataUtils.HexStringToByteArray(jo[KEY_THUMBNAIL_BLOB_ID].Value<string>());
				}

				if (jo.Children().Any(e => e.Path.EndsWith(KEY_FILE_NAME)))
				{
					fileName = jo[KEY_FILE_NAME].Value<string>();
				}
				else
				{
					fileName = "unnamed";
				}

				return new FileMessage(
						blobId,
						encryptionKey,
						mimeType,
						fileName,
						fileSize,
						thumbnailBlobId
				);
			}
			catch
			{
				throw new BadMessageException();
			}
		}
	}
}
