using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class FolderField : Field
	{
		public FolderField(String key, bool required) :
			base(key, required)
		{
		}

		public string GetValue()
		{
			if (this.value != null)
			{
				return Path.GetFullPath(this.value);
			}

			return null;
		}
	}
}
