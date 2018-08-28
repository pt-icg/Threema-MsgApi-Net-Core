using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi
{
	public class PublicKeyStoreNone : PublicKeyStore
	{
		protected override byte[] FetchPublicKey(string threemaId)
		{
			return null;
		}

		protected override void Save(string threemaId, byte[] publicKey)
		{
			//do nothing
		}
	}
}
