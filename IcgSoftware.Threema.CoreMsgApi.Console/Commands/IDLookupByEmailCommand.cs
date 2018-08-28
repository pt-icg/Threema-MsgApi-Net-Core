using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class IDLookupByEmailCommand : Command
	{
		private readonly TextField emailField;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;

		public IDLookupByEmailCommand() :
			base("ID Lookup By Email Address", "Lookup the ID linked to the given email address (will be hashed locally).")
		{
			this.emailField = this.CreateTextField("email");
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
		}

		protected override void Execute()
		{
			string email = this.emailField.Value;
			string from = this.fromField.Value;
			string secret = this.secretField.Value;

			APIConnector apiConnector = this.CreateConnector(from, secret);
			string id = apiConnector.LookupEmail(email);
			if (id != null)
			{
				System.Console.WriteLine(id);
			}
		}
	}
}
