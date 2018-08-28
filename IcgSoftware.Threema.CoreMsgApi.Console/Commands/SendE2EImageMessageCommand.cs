using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Helpers;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class SendE2EImageMessageCommand : Command
	{
		private readonly ThreemaIDField toField;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;
		private readonly PrivateKeyField privateKeyField;
		private readonly FolderField imageFilePath;

		public SendE2EImageMessageCommand() : 
			base("Send End-to-End Encrypted Image Message",
				"Encrypt standard input and send the message to the given ID. 'from' is the API identity and 'secret' is the API secret. Prints the message ID on success.")
		{
			this.toField = this.CreateThreemaId("to", true);
			this.fromField = this.CreateThreemaId("from", true);
			this.secretField = this.CreateTextField("secret", true);
			this.privateKeyField = this.CreatePrivateKeyField("privateKey", true);
			this.imageFilePath = this.CreateFolderField("imageFilePath", true);
		}

		protected override void Execute()
		{
			string to = this.toField.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;
			byte[] privateKey = this.privateKeyField.GetValue();
			string imageFilePath = this.imageFilePath.GetValue();

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret), privateKey);
			string messageId = e2EHelper.SendImageMessage(to, imageFilePath);
			System.Console.WriteLine("MessageId: " + messageId);
		}
	}
}
