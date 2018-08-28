using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	public class GenerateKeyPairCommand : Command
	{
		private readonly TextField privateKeyPath;
		private readonly TextField publicKeyPath;

		public GenerateKeyPairCommand() :
			base("Generate Key Pair", "Generate a new key pair and write the private and public keys to the respective files (in hex).")
		{
			this.privateKeyPath = this.CreateTextField("privateKeyFile");
			this.publicKeyPath = this.CreateTextField("publicKeyFile");
		}

		protected override void Execute()
		{
			byte[] privateKey = new byte[32]; //NaCl.SECRETKEYBYTES
			byte[] publicKey = new byte[32]; //NaCl.PUBLICKEYBYTES

			CryptTool.GenerateKeyPair(ref privateKey, ref publicKey);

			// Write both keys to file
			DataUtils.WriteKeyFile(this.privateKeyPath.Value, new Key(Key.KeyType.PRIVATE, privateKey));
			DataUtils.WriteKeyFile(this.publicKeyPath.Value, new Key(Key.KeyType.PUBLIC, publicKey));
		}
	}
}
