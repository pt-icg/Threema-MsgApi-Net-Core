using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using IcgSoftware.Threema.CoreMsgApi.Messages;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Test
{
	[TestClass]
	public class CryptToolTest
	{
		[TestMethod]
		public void TestRandomNonce()
		{
			byte[] randomNonce = CryptTool.RandomNonce();

			//random nonce should be a byte array
			Assert.IsNotNull(randomNonce, "random nonce");

			//with a length of 24
			Assert.AreEqual(randomNonce.Length, ThreemaMessage.NONCEBYTES, "nonce length");
		}

		[TestMethod]
		public void TestCreateKeyPair()
		{
			byte[] privateKey = new byte[32]; //NaCl.SECRETKEYBYTES
			byte[] publicKey = new byte[32]; //NaCl.PUBLICKEYBYTES

			CryptTool.GenerateKeyPair(ref privateKey, ref publicKey);

			Assert.IsFalse(privateKey.SequenceEqual(new byte[32]), "empty private key");
			Assert.IsFalse(publicKey.SequenceEqual(new byte[32]), "empty public key");
		}

		[TestMethod]
		public void TestDecrypt()
		{
			string nonce = "0a1ec5b67b4d61a1ef91f55e8ce0471fee96ea5d8596dfd0";
			string box = "45181c7aed95a1c100b1b559116c61b43ce15d04014a805288b7d14bf3a993393264fe554794ce7d6007233e8ef5a0f1ccdd704f34e7c7b77c72c239182caf1d061d6fff6ffbbfe8d3b8f3475c2fe352e563aa60290c666b2e627761e32155e62f048b52ef2f39c13ac229f393c67811749467396ecd09f42d32a4eb419117d0451056ac18fac957c52b0cca67568e2d97e5a3fd829a77f914a1ad403c5909fd510a313033422ea5db71eaf43d483238612a54cb1ecfe55259b1de5579e67c6505df7d674d34a737edf721ea69d15b567bc2195ec67e172f3cb8d6842ca88c29138cc33e9351dbc1e4973a82e1cf428c1c763bb8f3eb57770f914a";

			Key privateKey = Key.DecodeKey(Common.otherPrivateKey);
			Key publicKey = Key.DecodeKey(Common.myPublicKey);

			ThreemaMessage message = CryptTool.DecryptMessage(
					DataUtils.HexStringToByteArray(box),
					privateKey.key,
					publicKey.key,
					DataUtils.HexStringToByteArray(nonce)
			);

			Assert.IsNotNull(message);
			Assert.IsInstanceOfType(message, typeof(TextMessage), "message is not a instance of text message");
			Assert.AreEqual(((TextMessage) message).Text, "Dies ist eine Testnachricht. äöü");
		}

		[TestMethod]
		public void TestEncrypt()
		{
			string text = "Dies ist eine Testnachricht. äöü";

			Key privateKey = Key.DecodeKey(Common.myPrivateKey);
			Key publicKey = Key.DecodeKey(Common.otherPublicKey);

			EncryptResult res = CryptTool.EncryptTextMessage(text, privateKey.key, publicKey.key);
			Assert.IsNotNull(res);
			Assert.IsNotNull(res.Nonce);
			Assert.IsNotNull(res.Result);
			Assert.IsFalse(res.Nonce.SequenceEqual(new byte[res.Nonce.Length]));
			Assert.IsFalse(res.Result.SequenceEqual(new byte[res.Result.Length]));
		}

		[TestMethod]
		public void TestDerivePublicKey()
		{
			Key privateKey = Key.DecodeKey(Common.myPrivateKey);
			Key publicKey = Key.DecodeKey(Common.myPublicKey);

			byte[] derivedPublicKey = CryptTool.DerivePublicKey(privateKey.key);

			Assert.IsNotNull(derivedPublicKey, "derived public key");
			Assert.IsTrue(derivedPublicKey.SequenceEqual(publicKey.key));
		}

    }
}
