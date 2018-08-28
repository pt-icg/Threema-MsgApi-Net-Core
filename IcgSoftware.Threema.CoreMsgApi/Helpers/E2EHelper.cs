#if CoreWinOnly
using Microsoft.Win32;
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;
using IcgSoftware.Threema.CoreMsgApi.Messages;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Helpers
{
	/// <summary>
	/// Helper to handle Threema end-to-end encryption.
	/// </summary>
	public class E2EHelper
	{
		private readonly APIConnector apiConnector;
		private readonly byte[] privateKey;

		public E2EHelper(APIConnector apiConnector, byte[] privateKey)
		{
			this.apiConnector = apiConnector;
			this.privateKey = privateKey;
		}

		/// <summary>
		/// Decrypt a Message and download the blobs of the Message (e.g. image or file)
		/// </summary>
		/// <param name="threemaId">Threema ID of the sender</param>
		/// <param name="messageId">Message ID</param>
		/// <param name="box">Encrypted box data of the file/image message</param>
		/// <param name="nonce">Nonce that was used for message encryption</param>
		/// <param name="outputFolder">Output folder for storing decrypted images/files</param>
		/// <returns>Result of message reception</returns>
		public ReceiveMessageResult ReceiveMessage(string threemaId, string messageId, byte[] box, byte[] nonce, string outputFolder)
		{
			//fetch public key
			byte[] publicKey = this.apiConnector.LookupKey(threemaId);

			if(publicKey == null)
			{
				throw new InvalidKeyException("invalid threema id");
			}

			ThreemaMessage message = CryptTool.DecryptMessage(box, this.privateKey, publicKey, nonce);
			if(message == null)
			{
				return null;
			}

			ReceiveMessageResult result = new ReceiveMessageResult(messageId, message);

			if (message.GetType() == typeof(ImageMessage))
			{
				//download image
				ImageMessage imageMessage = (ImageMessage)message;
				byte[] fileData = this.apiConnector.DownloadFile(imageMessage.BlobId);

				if(fileData == null)
				{
					throw new MessageParseException();
				}

				byte[] decryptedFileContent = CryptTool.Decrypt(fileData, privateKey, publicKey, imageMessage.Nonce);
				FileInfo imageFile = new FileInfo(outputFolder + "/" + messageId + ".jpg");

				using (FileStream stream = File.OpenWrite(imageFile.FullName))
				{
					stream.Write(decryptedFileContent, 0, decryptedFileContent.Length);
					stream.Close();
				}

				result.Files.Add(imageFile);
			}
			else if (message.GetType() == typeof(FileMessage))
			{
				//download file
				FileMessage fileMessage = (FileMessage)message;
				byte[] fileData = this.apiConnector.DownloadFile(fileMessage.BlobId);

				byte[] decryptedFileData = CryptTool.DecryptFileData(fileData, fileMessage.EncryptionKey);
				FileInfo file = new FileInfo(outputFolder + "/" + messageId + "-" + fileMessage.FileName);
				
				using (FileStream stream = File.OpenWrite(file.FullName))
				{
					stream.Write(decryptedFileData, 0, decryptedFileData.Length);
					stream.Close();
				}

				result.Files.Add(file);

				if(fileMessage.ThumbnailBlobId != null)
				{
					byte[] thumbnailData = this.apiConnector.DownloadFile(fileMessage.ThumbnailBlobId);

					byte[] decryptedThumbnailData = CryptTool.DecryptFileThumbnailData(thumbnailData, fileMessage.EncryptionKey);
					FileInfo thumbnailFile = new FileInfo(outputFolder + "/" + messageId + "-thumbnail.jpg");
					using (FileStream stream = File.OpenWrite(thumbnailFile.FullName))
					{
						stream.Write(decryptedThumbnailData, 0, decryptedThumbnailData.Length);
						stream.Close();
					}

					result.Files.Add(thumbnailFile);
				}
			}

			return result;
		}

		/// <summary>
		/// Encrypt a file message and send it to the given recipient.
		/// The thumbnailMessagePath can be null.
		/// </summary>
		/// <param name="threemaId">target Threema ID</param>
		/// <param name="fileMessageFile">the file to be sent</param>
		/// <param name="thumbnailMessageFile">file for thumbnail; if not set, no thumbnail will be sent</param>
		/// <returns>generated message ID</returns>
		public string SendFileMessage(string threemaId, FileInfo fileMessageFile, FileInfo thumbnailMessageFile)
		{
			//fetch public key
			byte[] publicKey = this.apiConnector.LookupKey(threemaId);

			if (publicKey == null)
			{
				throw new InvalidKeyException("invalid threema id");
			}

			//check capability of a key
			CapabilityResult capabilityResult = this.apiConnector.LookupKeyCapability(threemaId);
			if (capabilityResult == null || !capabilityResult.CanImage)
			{
				throw new NotAllowedException();
			}

			if (fileMessageFile == null)
			{
				throw new ArgumentException("fileMessageFile must not be null.");
			}

			if (!fileMessageFile.Exists)
			{
				throw new FileNotFoundException(fileMessageFile.FullName);
			}

			byte[] fileData;
			using (Stream stream = File.OpenRead(fileMessageFile.FullName))
			{
				fileData = new byte[stream.Length];
				stream.Read(fileData, 0, (int)stream.Length);
				stream.Close();
			}

			if (fileData == null)
			{
				throw new IOException("invalid file");
			}

			//encrypt the image
			EncryptResult encryptResult = CryptTool.EncryptFileData(fileData);

			//upload the image
			UploadResult uploadResult = apiConnector.UploadFile(encryptResult);

			if(!uploadResult.IsSuccess)
			{
				throw new IOException("could not upload file (upload response " + uploadResult.ResponseCode + ")");
			}

			UploadResult uploadResultThumbnail = null;

			if (thumbnailMessageFile != null && thumbnailMessageFile.Exists)
			{
				byte[] thumbnailData;
				using (Stream stream = File.OpenRead(thumbnailMessageFile.FullName))
				{
					thumbnailData = new byte[stream.Length];
					stream.Read(thumbnailData, 0, (int)stream.Length);
					stream.Close();
				}

				if (thumbnailData == null)
				{
					throw new IOException("invalid thumbnail file");
				}

				//encrypt the thumbnail
				EncryptResult encryptResultThumbnail = CryptTool.encryptFileThumbnailData(fileData, encryptResult.Secret);

				//upload the thumbnail
				uploadResultThumbnail = this.apiConnector.UploadFile(encryptResultThumbnail);
			}

			//send it
			EncryptResult fileMessage = CryptTool.EncryptFileMessage(
					encryptResult,
					uploadResult,
					GetMIMEType(fileMessageFile),
					fileMessageFile.Name,
					(int) fileMessageFile.Length,
					uploadResultThumbnail,
					privateKey, publicKey);

			return this.apiConnector.SendE2EMessage(
					threemaId,
					fileMessage.Nonce,
					fileMessage.Result);
		}

		/// <summary>
		/// Encrypt an image message and send it to the given recipient.
		/// </summary>
		/// <param name="threemaId">threemaId target Threema ID</param>
		/// <param name="imageFilePath">path to read image data from</param>
		/// <returns>generated message ID</returns>
		public string SendImageMessage(string threemaId, string imageFilePath)
		{
			//fetch public key
			byte[] publicKey = this.apiConnector.LookupKey(threemaId);

			if (publicKey == null)
			{
				throw new InvalidKeyException("invalid threema id");
			}

			//check capability of a key
			CapabilityResult capabilityResult = this.apiConnector.LookupKeyCapability(threemaId);
			if (capabilityResult == null || !capabilityResult.CanImage)
			{
				throw new NotAllowedException();
			}

			byte[] fileData = File.ReadAllBytes(imageFilePath);
			if (fileData == null)
			{
				throw new IOException("invalid file");
			}

			//encrypt the image
			EncryptResult encryptResult = CryptTool.Encrypt(fileData, this.privateKey, publicKey);

			//upload the image
			UploadResult uploadResult = apiConnector.UploadFile(encryptResult);

			if (!uploadResult.IsSuccess)
			{
				throw new IOException("could not upload file (upload response " + uploadResult.ResponseCode + ")");
			}

			//send it
			EncryptResult imageMessage = CryptTool.EncryptImageMessage(encryptResult, uploadResult, this.privateKey, publicKey);

			return apiConnector.SendE2EMessage(
					threemaId,
					imageMessage.Nonce,
					imageMessage.Result);
		}

		/// <summary>
		/// Encrypt a text message and send it to the given recipient.
		/// </summary>
		/// <param name="threemaId">target Threema ID</param>
		/// <param name="text">the text to send</param>
		/// <returns>generated message ID</returns>
		public string SendTextMessage(string threemaId, string text)
		{
			//fetch public key
			byte[] publicKey = this.apiConnector.LookupKey(threemaId);

			if (publicKey == null)
			{
				throw new InvalidKeyException("invalid threema id");
			}
			EncryptResult res = CryptTool.EncryptTextMessage(text, this.privateKey, publicKey);

			return this.apiConnector.SendE2EMessage(threemaId, res.Nonce, res.Result);
		}

		/// <summary>
		/// Get mime type of the file extension via registry.
		/// </summary>
		/// <param name="file">mime type of this file</param>
		/// <returns>mime type</returns>
		private string GetMIMEType(FileInfo file)
		{
#if CoreWinOnly
			if (file == null)
			{
				throw new ArgumentException("file must not be null.");
			}

			string mimeType = "application/unknown";

            //Using Microsoft.Win32.Registry. But you lose portability and can't run the application on Linux and MacOS anymore. Implement GetMIMEType in another way.
            RegistryKey regKey = Registry.ClassesRoot.OpenSubKey(
                    file.Extension.ToLower()
                );

            if (regKey != null)
            {
                object contentType = regKey.GetValue("Content Type");

                if (contentType != null)
                {
                    mimeType = contentType.ToString();
                }
            }

            return mimeType;
#else
            throw new CoreMigrationException("Using Microsoft.Win32.Registry. But you lose portability and can't run the application on Linux and MacOS anymore. Implement GetMIMEType in another way.");
#endif
		}
	}
}
