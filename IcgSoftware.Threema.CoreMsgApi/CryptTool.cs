using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;
using IcgSoftware.Threema.CoreMsgApi.Messages;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi
{
	/// <summary>
	/// Contains static methods to do various Threema cryptography related tasks.
	/// </summary>
	public class CryptTool
	{
		// HMAC-SHA256 keys for email/mobile phone hashing
		private static readonly byte[] EMAIL_HMAC_KEY = new byte[] {(byte)0x30,(byte)0xa5,(byte)0x50,(byte)0x0f,(byte)0xed,(byte)0x97,(byte)0x01,(byte)0xfa,(byte)0x6d,(byte)0xef,(byte)0xdb,(byte)0x61,(byte)0x08,(byte)0x41,(byte)0x90,(byte)0x0f,(byte)0xeb,(byte)0xb8,(byte)0xe4,(byte)0x30,(byte)0x88,(byte)0x1f,(byte)0x7a,(byte)0xd8,(byte)0x16,(byte)0x82,(byte)0x62,(byte)0x64,(byte)0xec,(byte)0x09,(byte)0xba,(byte)0xd7};
		private static readonly byte[] PHONENO_HMAC_KEY = new byte[] {(byte)0x85,(byte)0xad,(byte)0xf8,(byte)0x22,(byte)0x69,(byte)0x53,(byte)0xf3,(byte)0xd9,(byte)0x6c,(byte)0xfd,(byte)0x5d,(byte)0x09,(byte)0xbf,(byte)0x29,(byte)0x55,(byte)0x5e,(byte)0xb9,(byte)0x55,(byte)0xfc,(byte)0xd8,(byte)0xaa,(byte)0x5e,(byte)0xc4,(byte)0xf9,(byte)0xfc,(byte)0xd8,(byte)0x69,(byte)0xe2,(byte)0x58,(byte)0x37,(byte)0x07,(byte)0x23};

		private static readonly byte[] FILE_NONCE = new byte[] {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x01};
		private static readonly byte[] FILE_THUMBNAIL_NONCE = new byte[] {0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x00,0x02};

		private const int SYMMKEYBYTES = 32;

		/// <summary>
		/// Encrypt a text message.
		/// </summary>
		/// <param name="text">the text to be encrypted (max. 3500 bytes)</param>
		/// <param name="senderPrivateKey">the private key of the sending ID</param>
		/// <param name="recipientPublicKey">the public key of the receiving ID</param>
		/// <returns></returns>
		public static EncryptResult EncryptTextMessage(String text, byte[] senderPrivateKey, byte[] recipientPublicKey)
		{
			return EncryptMessage(new TextMessage(text), senderPrivateKey, recipientPublicKey);
		}

		/// <summary>
		/// Encrypt an image message.
		/// </summary>
		/// <param name="encryptResult">result of the image encryption</param>
		/// <param name="uploadResult">result of the upload</param>
		/// <param name="senderPrivateKey">the private key of the sending ID</param>
		/// <param name="recipientPublicKey">the public key of the receiving ID</param>
		/// <returns>encrypted result</returns>
		public static EncryptResult EncryptImageMessage(EncryptResult encryptResult, UploadResult uploadResult, byte[] senderPrivateKey, byte[] recipientPublicKey)
		{
			return EncryptMessage(
					new ImageMessage(uploadResult.BlobId,
						encryptResult.Size,
						encryptResult.Nonce),
					senderPrivateKey,
					recipientPublicKey);
		}

		/// <summary>
		/// Encrypt a file message.
		/// </summary>
		/// <param name="encryptResult">result of the file data encryption</param>
		/// <param name="uploadResult">result of the upload</param>
		/// <param name="mimeType">MIME type of the file</param>
		/// <param name="fileName">File name</param>
		/// <param name="fileSize">Size of the file, in bytes</param>
		/// <param name="uploadResultThumbnail">result of thumbnail upload</param>
		/// <param name="senderPrivateKey">Private key of sender</param>
		/// <param name="recipientPublicKey">Public key of recipient</param>
		/// <returns>Result of the file message encryption (not the same as the file data encryption!)</returns>
		public static EncryptResult EncryptFileMessage(EncryptResult encryptResult,
				UploadResult uploadResult,
				String mimeType,
				String fileName,
				int fileSize,
				UploadResult uploadResultThumbnail,
				byte[] senderPrivateKey, byte[] recipientPublicKey)
		{
			return EncryptMessage(
				new FileMessage(uploadResult.BlobId,
					encryptResult.Secret,
					mimeType,
					fileName,
					fileSize,
					uploadResultThumbnail != null ? uploadResultThumbnail.BlobId : null),
				senderPrivateKey,
				recipientPublicKey);
		}

		private static EncryptResult EncryptMessage(ThreemaMessage threemaMessage, byte[] privateKey, byte[] publicKey)
		{
			// determine random amount of PKCS7 padding
			int padbytes = new Random().Next(254) + 1;

			byte[] messageBytes;
			try
			{
				messageBytes = threemaMessage.GetData();
			}
			catch
			{
				return null;
			}

			// prepend type byte (0x02) to message data
			byte[] data = new byte[1 + messageBytes.Length + padbytes];
			data[0] = (byte)threemaMessage.GetTypeCode();

			messageBytes.CopyTo(data, 1);

			// append padding
			for (int i = 0; i < padbytes; i++)
			{
				data[i + 1 + messageBytes.Length] = (byte)padbytes;
			}

			return Encrypt(data, privateKey, publicKey);
		}

		/// <summary>
		/// Decrypt an NaCl box using the recipient's private key and the sender's public key.
		/// </summary>
		/// <param name="box">The box to be decrypted</param>
		/// <param name="privateKey">The private key of the recipient</param>
		/// <param name="publicKey">The public key of the sender</param>
		/// <param name="nonce">The nonce that was used for encryption</param>
		/// <returns>The decrypted data, or null if decryption failed</returns>
		public static byte[] Decrypt(byte[] box, byte[] privateKey, byte[] publicKey, byte[] nonce)
		{
			return Sodium.PublicKeyBox.Open(box, nonce, privateKey, publicKey);
		}

		/// <summary>
		/// Decrypt symmetrically encrypted file data.
		/// </summary>
		/// <param name="fileData">The encrypted file data</param>
		/// <param name="secret">The symmetric key that was used for encryption</param>
		/// <returns>The decrypted file data, or null if decryption failed</returns>
		public static byte[] DecryptFileData(byte[] fileData, byte[] secret)
		{
			byte[] box = new byte[fileData.Length + 16];
			fileData.CopyTo(box, 16);
			return Sodium.SecretBox.Open(box, FILE_NONCE, secret);
		}

		/// <summary>
		/// Decrypt symmetrically encrypted file thumbnail data.
		/// </summary>
		/// <param name="fileData">The encrypted thumbnail data</param>
		/// <param name="secret">The symmetric key that was used for encryption</param>
		/// <returns>The decrypted thumbnail data, or null if decryption failed</returns>
		public static byte[] DecryptFileThumbnailData(byte[] fileData, byte[] secret)
		{
			byte[] box = new byte[fileData.Length + 16];
			fileData.CopyTo(box, 16);
			return Sodium.SecretBox.Open(box, FILE_THUMBNAIL_NONCE, secret);
		}

		/// <summary>
		/// Decrypt a message.
		/// </summary>
		/// <param name="box">the box to be decrypted</param>
		/// <param name="recipientPrivateKey">the private key of the receiving ID</param>
		/// <param name="senderPublicKey">the public key of the sending ID</param>
		/// <param name="nonce">the nonce that was used for the encryption</param>
		/// <returns>decrypted message (text or delivery receipt)</returns>
		public static ThreemaMessage DecryptMessage(byte[] box,  byte[] recipientPrivateKey, byte[] senderPublicKey, byte[] nonce)
		{
			byte[] data = Decrypt(box, recipientPrivateKey, senderPublicKey, nonce);
			if (data == null)
			{
				throw new DecryptionFailedException();
			}

			// remove padding
			int padbytes = data[data.Length - 1] & 0xFF;
			int realDataLength = data.Length - padbytes;
			if (realDataLength < 1)
			{
				// Bad message padding
				throw new BadMessageException();
			}

			// first byte of data is type
			int type = data[0] & 0xFF;

			switch (type)
			{
				case TextMessage.TYPE_CODE:
					// Text message
					if (realDataLength < 2)
					{
						throw new BadMessageException();
					}

					return new TextMessage(Encoding.UTF8.GetString(data.Skip(1).Take(realDataLength - 1).ToArray()));

				case DeliveryReceipt.TYPE_CODE:
					/* Delivery receipt */
					if (realDataLength < MessageId.MESSAGE_ID_LEN + 2 || ((realDataLength - 2) % MessageId.MESSAGE_ID_LEN) != 0)
					{
						throw new BadMessageException();
					}

					DeliveryReceipt.Type receiptType = (DeliveryReceipt.Type)Enum.Parse(typeof(DeliveryReceipt.Type), Convert.ToString((int)data[1] & 0xFF));
					if (receiptType == null)
					{
						throw new BadMessageException();
					}

					IEnumerable<MessageId> messageIds = new LinkedList<MessageId>();

					int numMsgIds = ((realDataLength - 2) / MessageId.MESSAGE_ID_LEN);
					for (int i = 0; i < numMsgIds; i++)
					{
						messageIds.ToList().Add(new MessageId(data, 2 + i*MessageId.MESSAGE_ID_LEN));
					}

					return new DeliveryReceipt(receiptType, messageIds.ToList());

				case ImageMessage.TYPE_CODE:
					if(realDataLength != (1 + ThreemaMessage.BLOB_ID_LEN + 4 + ThreemaMessage.NONCEBYTES))
					{
						throw new BadMessageException();
					}
					byte[] blobId = new byte[ThreemaMessage.BLOB_ID_LEN];
					data.Skip(1).Take(ThreemaMessage.BLOB_ID_LEN).ToArray().CopyTo(blobId, 0);
					int size = ReadSwappedInteger(data, 1 + ThreemaMessage.BLOB_ID_LEN);
					byte[] fileNonce = new byte[ThreemaMessage.NONCEBYTES];
					data.Skip(1 + ThreemaMessage.BLOB_ID_LEN + 4).Take(ThreemaMessage.NONCEBYTES).ToArray().CopyTo(fileNonce, 0);

					return new ImageMessage(blobId, size, fileNonce);

				case FileMessage.TYPE_CODE:
					ASCIIEncoding encoding = new ASCIIEncoding();
					return FileMessage.FromString(encoding.GetString(data.Skip(1).Take(realDataLength - 1).ToArray()));

				default:
					throw new UnsupportedMessageTypeException();
			}
		}

		private static int ReadSwappedInteger(byte[] data, int offset)
		{
			return ((data[offset + 0] & 255) << 0) + ((data[offset + 1] & 255) << 8) + ((data[offset + 2] & 255) << 16) + ((data[offset + 3] & 255) << 24);
		}


		/// <summary>
		/// Generate a new key pair.
		/// </summary>
		/// <param name="privateKey">is used to return the generated private key (length must be SealedPublicKeyBox.RecipientSecretKeyBytes)</param>
		/// <param name="publicKey">is used to return the generated public key (length must be SealedPublicKeyBox.RecipientPublicKeyBytes)</param>
		public static void GenerateKeyPair(ref byte[] privateKey, ref byte[] publicKey)
		{
			if (publicKey.Length != Sodium.SealedPublicKeyBox.RecipientPublicKeyBytes || privateKey.Length != Sodium.SealedPublicKeyBox.RecipientSecretKeyBytes)
			{
				throw new ArgumentException("Wrong key length");
			}

			Sodium.KeyPair keyPair = Sodium.PublicKeyBox.GenerateKeyPair();
			privateKey = keyPair.PrivateKey;
			publicKey = keyPair.PublicKey;
		}

		/// <summary>
		/// Encrypt data using NaCl asymmetric ("box") encryption.
		/// </summary>
		/// <param name="data">the data to be encrypted</param>
		/// <param name="privateKey">is used to return the generated private key (length must be SealedPublicKeyBox.RecipientSecretKeyBytes)</param>
		/// <param name="publicKey">is used to return the generated public key (length must be SealedPublicKeyBox.RecipientPublicKeyBytes)</param>
		/// <returns></returns>
		public static EncryptResult Encrypt(byte[] data,byte[] privateKey, byte[] publicKey)
		{
			if (publicKey.Length != Sodium.SealedPublicKeyBox.RecipientPublicKeyBytes || privateKey.Length != Sodium.SealedPublicKeyBox.RecipientSecretKeyBytes)
			{
				throw new ArgumentException("Wrong key length");
			}

			byte[] nonce = RandomNonce();
			byte[] box = Sodium.PublicKeyBox.Create(data, nonce, privateKey, publicKey);
			return new EncryptResult(box, null, nonce);
		}

		/// <summary>
		/// Encrypt file data using NaCl symmetric encryption with a random key.
		/// </summary>
		/// <param name="data">the file contents to be encrypted</param>
		/// <returns>the encryption result including the random key</returns>
		public static EncryptResult EncryptFileData(byte[] data)
		{
			//create random key
			Random rnd = new Random();
			byte[] encryptionKey = new byte[CryptTool.SYMMKEYBYTES];
			rnd.NextBytes(encryptionKey);

			//encrypt file data in-place
			data = Sodium.SecretBox.Create(data, FILE_NONCE, encryptionKey);

			//skip first temp 16 bytes for encryption
			return new EncryptResult(data.Skip(16).ToArray(), encryptionKey, FILE_NONCE);
		}

		/// <summary>
		/// Encrypt file thumbnail data using NaCl symmetric encryption with a random key.
		/// </summary>
		/// <param name="data">data the file contents to be encrypted</param>
		/// <param name="encryptionKey"></param>
		/// <returns>the encryption result including the random key</returns>
		public static EncryptResult encryptFileThumbnailData(byte[] data, byte[] encryptionKey)
		{
			// encrypt file data in-place
			data = Sodium.SecretBox.Create(data, FILE_THUMBNAIL_NONCE, encryptionKey);

			return new EncryptResult(data, encryptionKey, FILE_THUMBNAIL_NONCE);
		}

		/// <summary>
		/// Hashes an email address for identity lookup.
		/// </summary>
		/// <param name="email">email the email address</param>
		/// <returns>the raw hash</returns>
		public static byte[] HashEmail(string email)
		{
			try
			{
				ASCIIEncoding encoding = new ASCIIEncoding();
				var hmac = new HMACSHA256(EMAIL_HMAC_KEY);
				return hmac.ComputeHash(encoding.GetBytes(email.Trim())).ToArray();
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error in HashEmail(): {0}", ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Hashes a phone number for identity lookup.
		/// </summary>
		/// <param name="phoneNo">phoneNo the phone number</param>
		/// <returns>the raw hash</returns>
		public static byte[] HashPhoneNo(string phoneNo)
		{
			try
			{
				ASCIIEncoding encoding = new ASCIIEncoding();
				var hmac = new HMACSHA256(PHONENO_HMAC_KEY);
				return hmac.ComputeHash(encoding.GetBytes(Regex.Replace(phoneNo, "[^0-9]", "")));
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Error in HashPhoneNo(): {0}", ex.Message);
				return null;
			}
		}

		/// <summary>
		/// Generate a random nonce.
		/// </summary>
		/// <returns>random nonce</returns>
		public static byte[] RandomNonce()
		{
			byte[] nonce = new byte[ThreemaMessage.NONCEBYTES];
			new Random().NextBytes(nonce);
			return nonce;
		}

		/// <summary>
		/// Return the public key that corresponds with a given private key.
		/// </summary>
		/// <param name="privateKey">The private key whose public key should be derived</param>
		/// <returns>The corresponding public key.</returns>
		public static byte[] DerivePublicKey(byte[] privateKey) 
		{
			Sodium.KeyPair keyPair = Sodium.PublicKeyBox.GenerateKeyPair(privateKey);
			return keyPair.PublicKey;
		}
	}
}
