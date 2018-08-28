using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class PrivateKeyField : KeyField
	{
		public PrivateKeyField(string key, bool required) :
			base(key, required)
		{
		}

		public byte[] GetValue()
		{
			try
			{
				return this.ReadKey(this.value, IcgSoftware.Threema.CoreMsgApi.Key.KeyType.PRIVATE);
			}
			catch (Exception ex)
			{
				throw new InvalidKeyException("invalid private key", ex);
			}
		}
	}
}
