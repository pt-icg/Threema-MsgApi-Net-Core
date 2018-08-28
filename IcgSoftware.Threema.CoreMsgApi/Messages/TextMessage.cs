using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Messages
{
	/// <summary>
	/// A text message that can be sent/received with end-to-end encryption via Threema.
	/// </summary>
	public class TextMessage : ThreemaMessage
	{
		public const int TYPE_CODE = 0x01;

		private readonly string text;

		public TextMessage(String text)
		{
			this.text = text;
		}

		public string Text { get { return text; } }

		public override int GetTypeCode() {
			return TYPE_CODE;
		}

		public override byte[] GetData()
		{
			return Encoding.UTF8.GetBytes(text);
		}

		public override string ToString()
		{
			return text;
		}
	}
}
