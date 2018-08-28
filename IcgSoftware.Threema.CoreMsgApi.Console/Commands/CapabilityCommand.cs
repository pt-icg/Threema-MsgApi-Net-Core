using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands
{
	class CapabilityCommand : Command
	{
		private readonly ThreemaIDField threemaIdField;
		private readonly ThreemaIDField fromField;
		private readonly TextField secretField;

		public CapabilityCommand() :
			base("Fetch Capability", "Fetch the capability of a Threema ID")
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

			CapabilityResult capabilities = this.CreateConnector(from, secret)
					.LookupKeyCapability(threemaId);

			System.Console.WriteLine(capabilities);
		}
	}
}
