using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Messages
{
	class DeliveryReceipt : ThreemaMessage
	{
		public const int TYPE_CODE = 0x80;

		public enum Type
		{
			RECEIVED = 1,
			READ = 2,
			USER_ACK = 3
		}

		private readonly Type receiptType;
		private readonly List<MessageId> ackedMessageIds;

		public DeliveryReceipt(Type receiptType, List<MessageId> ackedMessageIds) {
			this.receiptType = receiptType;
			this.ackedMessageIds = ackedMessageIds;
		}

		public Type ReceiptType
		{
			get { return receiptType; }
		}

		public List<MessageId> AckedMessageIds
		{
			get { return ackedMessageIds; }
		}

		public override int GetTypeCode() {
			return TYPE_CODE;
		}

		public override byte[] GetData()
		{
			//Not implemented yet
			return new byte[0];
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder("Delivery receipt (");
			sb.Append(receiptType);
			sb.Append("): ");
			int i = 0;
			ackedMessageIds.ForEach(messageId =>
			{
				if (i != 0)
				{
					sb.Append(", ");
				}
				sb.Append(messageId);
				i++;
			});
			return sb.ToString();
		}

		/**
		 * A delivery receipt type. The following types are defined:
		 *
		 * <ul>
		 *     <li>RECEIVED: the message has been received and decrypted on the recipient's device</li>
		 *     <li>READ: the message has been shown to the user in the chat view
		 *         (note that this status can be disabled)</li>
		 *     <li>USER_ACK: the user has explicitly acknowledged the message (usually by
		 *         long-pressing it and choosing the "acknowledge" option)</li>
		 * </ul>
		 */
		/*
		public enum Type {
			RECEIVED(1), READ(2), USER_ACK(3);

			private final int code;

			Type(int code) {
				this.code = code;
			}

			public int getCode() {
				return code;
			}

			public static Type get(int code) {
				for (Type t : values()) {
					if (t.code == code)
						return t;
				}
				return null;
			}
		}
		*/
	}
}
