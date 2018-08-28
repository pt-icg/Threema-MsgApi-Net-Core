using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Commands.Fields
{
	public class PublicKeyField : KeyField
	{
		public PublicKeyField(string key, bool required) :
			base(key, required)
		{
		}

		public byte[] GetValue()
		{
			try
			{
				return this.ReadKey(this.value, IcgSoftware.Threema.CoreMsgApi.Key.KeyType.PUBLIC);
			}
			catch (Exception ex)
			{
				throw new InvalidKeyException("invalid public key", ex);
			}
		}
	}
}
