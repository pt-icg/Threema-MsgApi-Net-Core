using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class ByteArrayField : Field
	{
		public ByteArrayField(String key, bool required) :
			base(key, required)
		{
		}

		public byte[] GetValue()
		{
			return DataUtils.HexStringToByteArray(this.value);
		}
	}
}
