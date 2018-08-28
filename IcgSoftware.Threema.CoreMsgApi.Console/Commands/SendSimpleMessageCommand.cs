using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class SendSimpleMessageCommand : Command
	{
		private readonly ThreemaIDField threemaId;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;

		public SendSimpleMessageCommand() :
			base("Send Simple Message", 
				"Send a message from standard input with server-side encryption to the given ID. 'from' is the API identity and 'secret' is the API secret. Returns the message ID on success.")
		{
			this.threemaId = this.CreateThreemaId("to");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
		}

		protected override void Execute()
		{
			string to = this.threemaId.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;

			//string text = ReadStream(System.Console.In, Encoding.UTF8).Trim();
			string text = DataUtils.Utf8Endcode(System.Console.ReadLine().Trim());

			APIConnector apiConnector = this.CreateConnector(from, secret);
			string messageId = apiConnector.SendTextMessageSimple(to, text);
			System.Console.WriteLine(messageId);
		}
	}
}
