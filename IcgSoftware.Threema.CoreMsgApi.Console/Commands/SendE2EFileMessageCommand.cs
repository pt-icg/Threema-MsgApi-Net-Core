using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Helpers;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class SendE2EFileMessageCommand : Command
	{
		private readonly ThreemaIDField threemaId;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;
		private readonly PrivateKeyField privateKeyField;
		private readonly FileField fileField;
		private readonly FileField thumbnailField;

		public SendE2EFileMessageCommand() :
			base("Send End-to-End Encrypted File Message",
				"Encrypt the file (and thumbnail) and send a file message to the given ID. 'from' is the API identity and 'secret' is the API secret. Prints the message ID on success.")
		{
			this.threemaId = this.CreateThreemaId("to");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
			this.privateKeyField = this.CreatePrivateKeyField("privateKey");
			this.fileField = this.CreateFileField("file");
			this.thumbnailField = this.CreateFileField("thumbnail", false);

		}

		protected override void Execute()
		{
			string to = this.threemaId.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;
			byte[] privateKey = this.privateKeyField.GetValue();
			FileInfo file = this.fileField.GetValue();
			FileInfo thumbnail =  this.thumbnailField.GetValue();

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret), privateKey);
			string messageId = e2EHelper.SendFileMessage(to, file, thumbnail);
			System.Console.WriteLine("MessageId: " + messageId);
		}
	}
}
