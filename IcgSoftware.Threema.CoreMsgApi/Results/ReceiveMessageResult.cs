using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Messages;

namespace IcgSoftware.Threema.CoreMsgApi.Results
{
	public class ReceiveMessageResult
	{
		private readonly string messageId;
		private readonly ThreemaMessage message;
		protected List<FileInfo> files = new List<FileInfo>();
		protected List<string> errors = new List<string>();

		public ReceiveMessageResult(string messageId, ThreemaMessage message)
		{
			this.messageId = messageId;
			this.message = message;
		}

		public List<FileInfo> Files
		{
			get { return this.files; }
		}

		public List<string> Errors
		{
			get { return this.errors; }
		}

		public ThreemaMessage Message
		{
			get { return this.message; }
		}

		public string MessageId
		{
			get { return messageId; }
		}
	}
}
