using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	public class HashPhoneCommand : Command
	{
		private readonly TextField phoneNo;

		public HashPhoneCommand() :
			base("Hash Phone Number", "Hash a phone number for identity lookup. Prints the hash in hex.")
		{
			this.phoneNo = this.CreateTextField("phoneNo", true);
		}

		protected override void Execute()
		{
			string phoneNo = this.phoneNo.Value;

			byte[] phoneHash = CryptTool.HashPhoneNo(phoneNo);
			System.Console.WriteLine(DataUtils.ByteArrayToHexString(phoneHash));
		}
	}
}
