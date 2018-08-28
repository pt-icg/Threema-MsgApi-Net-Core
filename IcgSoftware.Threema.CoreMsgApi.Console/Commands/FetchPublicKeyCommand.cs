using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class FetchPublicKeyCommand : Command
	{
		private readonly ThreemaIDField threemaIdField;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;

		public FetchPublicKeyCommand() :
			base("Fetch Public Key", "Lookup the public key for the given ID.")
		{
			this.threemaIdField = this.CreateThreemaId("id");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
		}

		protected override void Execute()
		{
			string threemaId = this.threemaIdField.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;

			APIConnector apiConnector = this.CreateConnector(from, secret);
			byte[] publicKey = apiConnector.LookupKey(threemaId);
			if (publicKey != null)
			{
				System.Console.WriteLine(new Key(Key.KeyType.PUBLIC, publicKey).Encode());
			}
		}
	}
}
