using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	public class DerivePublicKeyCommand : Command
	{
		private readonly PrivateKeyField privateKeyField;

		public DerivePublicKeyCommand() :
			base("Derive Public Key", "Derive the public key that corresponds with the given private key.")
		{
			this.privateKeyField = this.CreatePrivateKeyField("privateKey");
		}

		protected override void Execute()
		{
			byte[] privateKey = this.privateKeyField.GetValue();
			byte[] publicKey = CryptTool.DerivePublicKey(privateKey);

			System.Console.WriteLine("res");
			System.Console.WriteLine(new Key(Key.KeyType.PUBLIC, publicKey).Encode());
		}
	}
}
