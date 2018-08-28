using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class ThreemaIDField : Field
	{
		public ThreemaIDField(String key, bool required) :
			base(key, required)
		{
		}

		public string Value
		{
			get { return this.value; }
		}
	}
}
