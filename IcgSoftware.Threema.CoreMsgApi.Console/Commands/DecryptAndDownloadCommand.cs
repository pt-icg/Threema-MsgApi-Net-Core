using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Helpers;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class DecryptAndDownloadCommand : Command
	{
		private readonly ThreemaIDField threemaId;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;
		private readonly PrivateKeyField privateKeyField;
		private readonly ByteArrayField nonceField;
		private readonly FolderField outputFolderField;
		private readonly TextField messageIdField;

		public DecryptAndDownloadCommand() :
			base("Decrypt and download",
				"Decrypt a box (box from the stdin) message and download (if the message is a image or file message) the file(s) to the defined directory")
		{
			this.threemaId = this.CreateThreemaId("id");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
			this.privateKeyField = this.CreatePrivateKeyField("privateKey");
			this.messageIdField = this.CreateTextField("messageId");
			this.nonceField = this.CreateByteArrayField("nonce");
			this.outputFolderField = this.CreateFolderField("outputFolder", false);
		}

		protected override void Execute()
		{
			string id = this.threemaId.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;
			byte[] privateKey = this.privateKeyField.GetValue();
			byte[] nonce = this.nonceField.GetValue();
			string messageId = this.messageIdField.Value;
			string outputFolder = this.outputFolderField.GetValue();

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret), privateKey);

			byte[] box = DataUtils.HexStringToByteArray(System.Console.ReadLine().Trim());

			ReceiveMessageResult res = e2EHelper.ReceiveMessage(id, messageId, box, nonce, outputFolder);
			System.Console.WriteLine(res.Message.ToString());
			res.Files.ForEach(f => { System.Console.WriteLine(f.FullName); });
		}
	}
}
