using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi
{
	class MessageId
	{
		public const int MESSAGE_ID_LEN = 8;

		private readonly byte[] messageId;

		public MessageId(byte[] messageId)
		{
			if (messageId.Length != MESSAGE_ID_LEN)
			{
				throw new ArgumentException("Bad message ID length");
			}

			this.messageId = messageId;
		}

		public MessageId(byte[] data, int offset)
		{
			if ((offset + MESSAGE_ID_LEN) > data.Length)
			{
				throw new ArgumentException("Bad message ID buffer length");
			}

			this.messageId = new byte[MESSAGE_ID_LEN];
			//System.arraycopy(data, offset, this.messageId, 0, MESSAGE_ID_LEN);
			data.Skip(offset).Take(MESSAGE_ID_LEN).ToArray().CopyTo(this.messageId, 0);
		}

		public byte[] GetMessageId
		{
			get { return messageId; }
		}

		public override string ToString() {
			return DataUtils.ByteArrayToHexString(messageId);
		}
	}
}
