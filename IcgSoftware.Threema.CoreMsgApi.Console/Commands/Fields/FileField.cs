using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class FileField : Field
	{
		public FileField(String key, bool required) :
			base(key, required)
		{
		}

		public FileInfo GetValue()
		{
			if(this.value != null)
			{
				return new FileInfo(this.value);
			}

			return null;
		}

		protected bool Validate()
		{
			return !this.IsRequired || (this.value != null && File.Exists(this.value));
		}
	}
}
