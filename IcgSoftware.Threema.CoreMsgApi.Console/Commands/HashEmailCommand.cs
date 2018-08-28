using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	public class HashEmailCommand : Command
	{
		private readonly TextField emailField;

		public HashEmailCommand() :
			base("Hash Email Address", "Hash an email address for identity lookup. Prints the hash in hex.")
		{
			this.emailField = this.CreateTextField("email", true);
		}

		protected override void Execute()
		{
			byte[] emailHash = CryptTool.HashEmail(this.emailField.Value);
			System.Console.WriteLine(DataUtils.ByteArrayToHexString(emailHash));
		}
	}
}
