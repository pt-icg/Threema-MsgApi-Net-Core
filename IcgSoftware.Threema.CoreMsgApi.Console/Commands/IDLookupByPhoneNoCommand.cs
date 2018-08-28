using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class IDLookupByPhoneNoCommand : Command
	{
		private readonly TextField phoneNoField;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;

		public IDLookupByPhoneNoCommand() :
			base("ID Lookup By Phone Number",
				"Lookup the ID linked to the given phone number (will be hashed locally).")
		{
			this.phoneNoField = this.CreateTextField("phoneNo");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
		}

		protected override void Execute()
		{
			string phoneNo = this.phoneNoField.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;

			APIConnector apiConnector = this.CreateConnector(from, secret);
			string id = apiConnector.LookupPhone(phoneNo);
			if (id != null)
			{
				System.Console.WriteLine(id);
			}
		}
	}
}
