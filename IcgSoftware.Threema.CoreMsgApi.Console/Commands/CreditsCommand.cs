using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class CreditsCommand : Command
	{
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;

		public CreditsCommand() :
			base("Credits", "Fetch the remaining credits")
		{
			this.fromField = this.CreateThreemaId("from");
			this.secretField = this.CreateTextField("secret");
		}

		protected override void Execute()
		{
			string from = this.fromField.Value;
			string secret = this.secretField.Value;

			int? credits = this.CreateConnector(from, secret)
					.LookupCredits();

			if (credits != null)
			{
				System.Console.WriteLine("Remaining credits: {0}", credits);
			}
			else
			{
				System.Console.WriteLine("Error fetching credits");
			}
		}
	}
}
