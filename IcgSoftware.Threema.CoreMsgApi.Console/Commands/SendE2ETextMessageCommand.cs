using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Helpers;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class SendE2ETextMessageCommand : Command
	{
		private readonly ThreemaIDField threemaId;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;
		private readonly PrivateKeyField privateKeyField;

		public SendE2ETextMessageCommand() :
			base("Send End-to-End Encrypted Text Message",
				"Encrypt standard input and send the message to the given ID. 'from' is the API identity and 'secret' is the API secret. Prints the message ID on success.")
		{
			this.threemaId = this.CreateThreemaId("to");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
			this.privateKeyField = this.CreatePrivateKeyField("privateKey");
		}

		protected override void Execute()
		{
			string to = this.threemaId.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;
			byte[] privateKey = this.privateKeyField.GetValue();

			//string text = ReadStream(System.Console.In).Trim();
			string text = DataUtils.Utf8Endcode(System.Console.ReadLine().Trim());

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret), privateKey);
			String messageId = e2EHelper.SendTextMessage(to, text);
			System.Console.WriteLine("MessageId: " + messageId);
		}
	}
}
