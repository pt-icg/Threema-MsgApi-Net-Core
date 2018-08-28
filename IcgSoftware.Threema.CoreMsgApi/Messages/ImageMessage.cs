using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Messages
{
	class ImageMessage : ThreemaMessage
	{
		public const int TYPE_CODE = 0x02;

		private readonly byte[] blobId;
		private readonly int size;
		private readonly byte[] nonce;

		public ImageMessage(byte[] blobId, int size, byte[] nonce)
		{
			this.blobId = blobId;
			this.size = size;
			this.nonce = nonce;
		}

		public byte[] BlobId
		{
			get { return this.blobId; }
		}


		public int Size
		{
			get { return this.size; }
		}


		public byte[] Nonce
		{
			get { return this.nonce; }
		}

		public override int GetTypeCode()
		{
			return TYPE_CODE;
		}

		public override byte[] GetData()
		{
			byte[] data = new byte[BLOB_ID_LEN + 4 + ThreemaMessage.NONCEBYTES];
			int pos = 0;

			//System.arraycopy(this.blobId, 0, data, pos, BLOB_ID_LEN);
			this.blobId.CopyTo(data, 0);
			pos += BLOB_ID_LEN;

			//EndianUtils.writeSwappedInteger(data, pos, this.size);
			byte[] size = BitConverter.GetBytes(this.size);
			size.CopyTo(data, pos);
			pos += 4;

			//System.arraycopy(this.nonce, 0, data, pos, ThreemaMessage.NONCEBYTES);
			this.nonce.CopyTo(data, pos);

			return data;

		}

		public override string ToString()
		{
			return string.Format("blob {0}", DataUtils.ByteArrayToHexString(this.blobId));
		}
	}
}
