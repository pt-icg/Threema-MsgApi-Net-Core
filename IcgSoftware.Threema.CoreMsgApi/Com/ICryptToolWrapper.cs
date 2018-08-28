using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Com
{
	[ComVisible(true)]
	[Guid("4F376DEB-6F78-460A-836F-B38371712EFF")]
	public interface ICryptToolWrapper
	{
		ArrayList EncryptTextMessage(string text, string senderPrivateKey, string recipientPublicKey);
		ArrayList DecryptMessage(string box, string recipientPrivateKey, string senderPublicKey, string nonce);
		string HashEmail(string email);
		string HashPhoneNo(string phoneNo);
		void GenerateKeyPair(string privateKeyPath, string publicKeyPath);
		string DerivePublicKey(string privateKey);
	}
}
