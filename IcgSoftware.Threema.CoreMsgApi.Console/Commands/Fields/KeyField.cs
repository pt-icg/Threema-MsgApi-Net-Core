using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public abstract class KeyField : Field
	{
		public KeyField(string key, bool required) :
			base(key, required)
		{
		}

		protected byte[] ReadKey(string argument, String expectedKeyType)
		{
			Key key;

			if (File.Exists(argument))
			{
				key = DataUtils.ReadKeyFile(argument, expectedKeyType);
			}
			else
			{
				key = IcgSoftware.Threema.CoreMsgApi.Key.DecodeKey(argument, expectedKeyType);
			}

			return key.key;
		}
	}
}
