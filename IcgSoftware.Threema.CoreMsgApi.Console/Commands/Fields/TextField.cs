using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class TextField : Field
	{
		public TextField(String key, bool required) :
			base(key, required)
		{
		}

		public new string Value
		{
			get { return this.value; }
		}
	}
}
