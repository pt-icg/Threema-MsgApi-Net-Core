using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Helpers;
using IcgSoftware.Threema.CoreMsgApi.Results;

namespace IcgSoftware.Threema.CoreMsgApi.Com
{
	[ComVisible(true)]
	[ProgId("Threema.MsgApi.Com.MessageTool")]
	[Guid("097F1D26-B38E-4501-845D-6DE7DBFB5EAA")]
	public class MessageToolWrapper : IMessageToolWrapper
	{
		
		/// <summary>
		/// Wrapper to send simple message <see cref="Threema.MsgApi.APIConnector.SendTextMessageSimple"/>
		/// </summary>
		/// <param name="to">Recipient id</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender sercret</param>
		/// <param name="text">Text message</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>Message id</returns>
		public string SendTextMessageSimple(string to, string from, string secret, string text, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			APIConnector apiConnector = this.CreateConnector(from, secret, apiUrl);
			return apiConnector.SendTextMessageSimple(to, DataUtils.Utf8Endcode(text));
		}

		/// <summary>
		/// Wrapper to send text message E2E <see cref="Threema.MsgApi.E2EHelper.SendTextMessage"/>
		/// </summary>
		/// <param name="to">Recipient id</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender sercret</param>
		/// <param name="privateKey">Sender private key</param>
		/// <param name="text">Text message</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>Message id</returns>
		public string SendTextMessage(string to, string from, string secret, string privateKey, string text, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			byte[] privateKeyBytes = GetKey(privateKey, Key.KeyType.PRIVATE);

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret, apiUrl), privateKeyBytes);
			return e2EHelper.SendTextMessage(to, DataUtils.Utf8Endcode(text));
		}

		/// <summary>
		/// Wrapper to send image message E2E <see cref="Threema.MsgApi.E2EHelper.SendImageMessage"/>
		/// </summary>
		/// <param name="to">Recipient id</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender sercret</param>
		/// <param name="privateKey">Sender private key</param>
		/// <param name="imageFilePath">File path to image</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>Message id</returns>
		public string SendImageMessage(string to, string from, string secret, string privateKey, string imageFilePath, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			byte[] privateKeyBytes = GetKey(privateKey, Key.KeyType.PRIVATE);

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret, apiUrl), privateKeyBytes);
			return e2EHelper.SendImageMessage(to, imageFilePath);
		}

		/// <summary>
		/// Wrapper to send file message E2E <see cref="Threema.MsgApi.E2EHelper.SendFileMessage"/>
		/// </summary>
		/// <param name="to">Recipient id</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender sercret</param>
		/// <param name="privateKey">Sender private key</param>
		/// <param name="file">File path to file</param>
		/// <param name="thumbnail">File path to thumbnail</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>Message id</returns>
		public string SendFileMessage(string to, string from, string secret, string privateKey, string file, string thumbnail = null, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			byte[] privateKeyBytes = GetKey(privateKey, Key.KeyType.PRIVATE);
			FileInfo fileInfo = file != null ? new FileInfo(file) : null;
			FileInfo thumbnailInfo = thumbnail != null ? new FileInfo(thumbnail) : null;

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret, apiUrl), privateKeyBytes);
			return e2EHelper.SendFileMessage(to, fileInfo, thumbnailInfo);
		}

		/// <summary>
		/// Wrapper to id lookup via email <see cref="Threema.MsgApi.APIConnector.LookupEmail"/>
		/// </summary>
		/// <param name="email">Email for lookup</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender secret</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>id</returns>
		public string LookupEmail(string email, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			APIConnector apiConnector = this.CreateConnector(from, secret, apiUrl);
			return apiConnector.LookupEmail(email);
		}

		/// <summary>
		/// Wrapper to id lookup via phone number <see cref="Threema.MsgApi.APIConnector.LookupPhone"/>
		/// </summary>
		/// <param name="phoneNo">Phone number for lookup</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender secret</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>id</returns>
		public string LookupPhone(string phoneNo, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			APIConnector apiConnector = this.CreateConnector(from, secret, apiUrl);
			return apiConnector.LookupPhone(phoneNo);
		}

		/// <summary>
		/// Wrapper to lookup/fetch public key <see cref="Threema.MsgApi.APIConnector.LookupKey"/>
		/// </summary>
		/// <param name="threemaId">Id for lookup</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender secret</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>public key has hex-string</returns>
		public string LookupKey(string threemaId, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			APIConnector apiConnector = this.CreateConnector(from, secret, apiUrl);
			byte[] publicKey = apiConnector.LookupKey(threemaId);
			if (publicKey != null)
			{
				return new Key(Key.KeyType.PUBLIC, publicKey).Encode();
			}
			return null;
		}

		/// <summary>
		/// Wrapper to lookup capabilities <see cref="Threema.MsgApi.APIConnector.LookupKeyCapability"/>
		/// </summary>
		/// <param name="threemaId">Id for lookup</param>
		/// <param name="from">Sender id</param>
		/// <param name="secret">Sender secret</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>Array with capatilities</returns>
		public System.Collections.ArrayList LookupKeyCapability(string threemaId, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			CapabilityResult capabilities = this.CreateConnector(from, secret, apiUrl)
					.LookupKeyCapability(threemaId);

			var result = new ArrayList();
			capabilities.Capabilities.ToList().ForEach(c => result.Add(c));
			return result;
		}

		/// <summary>
		/// Wrapper to lookup credits <see cref="Threema.MsgApi.APIConnector.LookupCredits"/>
		/// </summary>
		/// <param name="from">From id</param>
		/// <param name="secret">From secret</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>credits or null</returns>
		public int? LookupCredits(string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			return this.CreateConnector(from, secret, apiUrl).LookupCredits();
		}

		/// <summary>
		/// Wrapper to receive message and download files <see cref="Threema.MsgApi.Helpers.E2EHelper.ReceiveMessage"/>
		/// </summary>
		/// <param name="id">Sender id</param>
		/// <param name="from">From id</param>
		/// <param name="secret">From secret</param>
		/// <param name="privateKey">From private key</param>
		/// <param name="messageId">Message id</param>
		/// <param name="nonce">Nonce as hex-string</param>
		/// <param name="box">Box message as hex-string</param>
		/// <param name="outputFolder">Optional path to output folder</param>
		/// <param name="apiUrl">Optional api url</param>
		/// <returns>Array with message-type, message-id and message</returns>
		public ArrayList ReceiveMessage(string id, string from, string secret,  string privateKey, string messageId, string nonce, string box, string outputFolder = null, string apiUrl = APIConnector.DEFAULTAPIURL)
		{
			byte[] privateKeyBytes =  GetKey(privateKey, Key.KeyType.PRIVATE);
			byte[] nonceBytes = DataUtils.HexStringToByteArray(nonce);

			E2EHelper e2EHelper = new E2EHelper(this.CreateConnector(from, secret, apiUrl), privateKeyBytes);

			byte[] boxBytes = DataUtils.HexStringToByteArray(box);

			ReceiveMessageResult res = e2EHelper.ReceiveMessage(id, messageId, boxBytes, nonceBytes, outputFolder);

			ArrayList result = new ArrayList();
			result.Add(res.Message.GetTypeCode().ToString());
			result.Add(res.MessageId);
			result.Add(res.Message.ToString());
			return result;
		}

		private APIConnector CreateConnector(string gatewayId, string secret, string apiUrl)
		{
			return new APIConnector(gatewayId, secret, apiUrl, new PublicKeyStoreNone());
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
