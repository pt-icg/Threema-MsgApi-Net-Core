using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Messages;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Com
{
	[ComVisible(true)]
	[ProgId("Threema.MsgApi.Com.CryptTool")]
	[Guid("0B98D653-CD23-4827-9C5B-AEC0DCFBD142")]
	public class CryptToolWrapper : ICryptToolWrapper
	{
		/// <summary>
		/// Wrapper to encrypt text <see cref="Threema.MsgApi.CryptTool.EncryptTextMessage"/>
		/// </summary>
		/// <param name="text">Text to encrypt</param>
		/// <param name="senderPrivateKey">Sender private key as hex-string</param>
		/// <param name="recipientPublicKey">Recipient public key as hex-string</param>
		/// <returns>Array with encrypted text, nonce and size</returns>
		public ArrayList EncryptTextMessage(string text, string senderPrivateKey, string recipientPublicKey)
		{
			byte[] privateKey = GetKey(senderPrivateKey, Key.KeyType.PRIVATE);
			byte[] publicKey = GetKey(recipientPublicKey, Key.KeyType.PUBLIC);

			string textEncoded = DataUtils.Utf8Endcode(text);

			EncryptResult encryptResult = CryptTool.EncryptTextMessage(textEncoded, privateKey, publicKey);

			var result = new ArrayList();
			result.Add(DataUtils.ByteArrayToHexString(encryptResult.Result));
			result.Add(DataUtils.ByteArrayToHexString(encryptResult.Nonce));
			result.Add(encryptResult.Size.ToString());
			return result;
		}

		/// <summary>
		/// Wrapper to decrypt box <see cref="Threema.MsgApi.CryptTool.DecryptMessage"/>
		/// </summary>
		/// <param name="box">Encrypted box as hex-straing</param>
		/// <param name="recipientPrivateKey">Recipient private key as hex-string</param>
		/// <param name="senderPublicKey">Sender public key as hex-string</param>
		/// <param name="nonce">Nonce as hex-string</param>
		/// <returns>Array with type and decrypted message</returns>
		public ArrayList DecryptMessage(string box, string recipientPrivateKey, string senderPublicKey, string nonce)
		{
			byte[] privateKey = GetKey(recipientPrivateKey, Key.KeyType.PRIVATE);
			byte[] publicKey = GetKey(senderPublicKey, Key.KeyType.PUBLIC);
			byte[] nonceBytes = DataUtils.HexStringToByteArray(nonce);
			byte[] boxBytes = DataUtils.HexStringToByteArray(box);

			ThreemaMessage message = CryptTool.DecryptMessage(boxBytes, privateKey, publicKey, nonceBytes);

			var result = new ArrayList();
			result.Add(message.GetTypeCode().ToString());
			result.Add(message.ToString());
			return result;
		}

		/// <summary>
		/// Wrapper to hash email <see cref="Threema.MsgApi.CryptTool.HashEmail"/>
		/// </summary>
		/// <param name="email">Email adress</param>
		/// <returns>Hash of email adress as hex-string</returns>
		public string HashEmail(string email)
		{
			byte[] emailHash = CryptTool.HashEmail(email);
			return DataUtils.ByteArrayToHexString(emailHash);
		}

		/// <summary>
		/// Wrapper to hash email <see cref="Threema.MsgApi.CryptTool.HashPhoneNo"/>
		/// </summary>
		/// <param name="phoneNo">Phone number</param>
		/// <returns>Hash of phone number as hex-string</returns>
		public string HashPhoneNo(string phoneNo)
		{
			byte[] phoneHash = CryptTool.HashPhoneNo(phoneNo);
			return DataUtils.ByteArrayToHexString(phoneHash);
		}

		/// <summary>
		/// Wrapper to generate key pair <see cref="Threema.MsgApi.CryptTool.GenerateKeyPair"/>
		/// </summary>
		/// <param name="privateKeyPath">Full path name of private key file</param>
		/// <param name="publicKeyPath">Full path name of public key file</param>
		public void GenerateKeyPair(string privateKeyPath, string publicKeyPath)
		{
			byte[] privateKey = new byte[32]; //NaCl.SECRETKEYBYTES
			byte[] publicKey = new byte[32]; //NaCl.PUBLICKEYBYTES

			CryptTool.GenerateKeyPair(ref privateKey, ref publicKey);

			// Write both keys to file
			DataUtils.WriteKeyFile(privateKeyPath, new Key(Key.KeyType.PRIVATE, privateKey));
			DataUtils.WriteKeyFile(publicKeyPath, new Key(Key.KeyType.PUBLIC, publicKey));
		}

		/// <summary>
		/// Wrapper to derive public key <see cref="CryptTool.DerivePublicKey"/>
		/// </summary>
		/// <param name="privateKey">private key as file path or hex-string</param>
		/// <returns>Public key as hex-string</returns>
		public string DerivePublicKey(string privateKey)
		{
			byte[] privateKeyBytes = GetKey(privateKey, Key.KeyType.PRIVATE);
			byte[] publicKey = CryptTool.DerivePublicKey(privateKeyBytes);

			return new Key(Key.KeyType.PUBLIC, publicKey).Encode();
		}

		private byte[] GetKey(string argument, string expectedKeyType)
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
