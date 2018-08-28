using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class EncryptCommand : Command
	{
		private readonly PrivateKeyField privateKeyField;
		private readonly PublicKeyField publicKeyField;

		public EncryptCommand() :
			base("Encrypt",
					"Encrypt standard input using the given sender private key and recipient public key. Prints two lines to standard output: first the nonce (hex), and then the box (hex).")
		{
			this.privateKeyField = this.CreatePrivateKeyField("privateKey");
			this.publicKeyField = this.CreatePublicKeyField("publicKey");
		}

		protected override void Execute()
		{
			byte[] privateKey = this.privateKeyField.GetValue();
			byte[] publicKey = this.publicKeyField.GetValue();

			/* read text from stdin */
			//string text = ReadStream(System.Console.In).Trim();
			string text = DataUtils.Utf8Endcode(System.Console.ReadLine().Trim());

			EncryptResult res = CryptTool.EncryptTextMessage(text, privateKey, publicKey);

			System.Console.WriteLine(DataUtils.ByteArrayToHexString(res.Nonce));
			System.Console.WriteLine(DataUtils.ByteArrayToHexString(res.Result));
		}
	}
}
