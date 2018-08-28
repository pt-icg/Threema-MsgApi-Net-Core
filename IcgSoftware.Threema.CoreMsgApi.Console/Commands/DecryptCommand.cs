using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Messages;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	public class DecryptCommand : Command
	{
		private readonly PrivateKeyField privateKeyField;
		private readonly PublicKeyField publicKeyField;
		private readonly ByteArrayField nonceField;

		public DecryptCommand() :
			base("Decrypt",
					"Decrypt standard input using the given recipient private key and sender public key. The nonce must be given on the command line, and the box (hex) on standard input. Prints the decrypted message to standard output.")
		{
			this.privateKeyField = this.CreatePrivateKeyField("privateKey");
			this.publicKeyField = this.CreatePublicKeyField("publicKey");
			this.nonceField = this.CreateByteArrayField("nonce");
		}

		protected override void Execute()
		{
			byte[] privateKey = this.privateKeyField.GetValue();
			byte[] publicKey = this.publicKeyField.GetValue();
			byte[] nonce = this.nonceField.GetValue();

			/* read box from stdin */
			//byte[] box = DataUtils.HexStringToByteArray(ReadStream(System.Console.In));
			byte[] box = DataUtils.HexStringToByteArray(DataUtils.Utf8Endcode(System.Console.ReadLine().Trim()));

			ThreemaMessage message = CryptTool.DecryptMessage(box, privateKey, publicKey, nonce);

			System.Console.WriteLine(message);
		}
	}
}
