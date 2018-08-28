using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using IcgSoftware.Threema.CoreMsgApi.Exceptions;
using IcgSoftware.Threema.CoreMsgApi.Results;
using Microsoft.Extensions.Configuration;

namespace IcgSoftware.Threema.CoreMsgApi
{
	/// <summary>
	/// Facilitates HTTPS communication with the Threema Message API.
	/// </summary>
	public class APIConnector
	{
		public const string DEFAULTAPIURL = "https://msgapi.threema.ch/";

		private readonly string apiUrl;
		private readonly PublicKeyStore publicKeyStore;
		private readonly string apiIdentity;
		private readonly string secret;

		public APIConnector(string apiIdentity, string secret, PublicKeyStore publicKeyStore) :
			this(apiIdentity, secret, APIConnector.DEFAULTAPIURL, publicKeyStore)
		{
		}
		public APIConnector(string apiIdentity, string secret, string apiUrl, PublicKeyStore publicKeyStore)
		{
			this.apiIdentity = apiIdentity;
			this.secret = secret;
			this.apiUrl = apiUrl;

			if (publicKeyStore != null)
			{
				this.publicKeyStore = publicKeyStore;
			}
			else
			{
				this.publicKeyStore = this.GetSQLiteDbProvider();
			}
		}

		/// <summary>
		/// Lookup credits for an ID.
		/// </summary>
		/// <returns>credits or null</returns>
		public int? LookupCredits()
		{
			string res = DoGet(new Uri(this.apiUrl + "credits"),
					MakeRequestParams());
			if(res != null)
			{
				int credits;
				int.TryParse(res, out credits);
				return credits;
			}
			return null;
		}

		/// <summary>
		/// Lookup an ID by email address. The email address will be hashed before
		/// being sent to the server.
		/// </summary>
		/// <param name="email">the email address</param>
		/// <returns>the ID, or null if not found</returns>
		public string LookupEmail(string email)
		{
			try
			{
				Dictionary<string, string> getParams = MakeRequestParams();

				byte[] emailHash = CryptTool.HashEmail(email);

				return DoGet(new Uri(this.apiUrl + "lookup/email_hash/" + DataUtils.ByteArrayToHexString(emailHash)), getParams);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}

		/// <summary>
		/// Lookup a public key by ID.
		/// </summary>
		/// <param name="id">The ID whose public key is desired</param>
		/// <returns>The corresponding public key, or null if not found</returns>
		public byte[] LookupKey(string id)
		{
			byte[] key = this.publicKeyStore.GetPublicKey(id);
			if (key == null)
			{
				try
				{
					Dictionary<string, string> getParams = MakeRequestParams();
					string pubkeyHex = DoGet(new Uri(this.apiUrl + "pubkeys/" + id), getParams);
					key = DataUtils.HexStringToByteArray(pubkeyHex);

					this.publicKeyStore.SetPublicKey(id, key);
				}
				catch (FileNotFoundException)
				{
					return null;
				}
			}
			return key;
		}

		/// <summary>
		/// Lookup the capabilities of a ID
		/// </summary>
		/// <param name="threemaId">The ID whose capabilities should be checked</param>
		/// <returns>The capabilities, or null if not found</returns>
		public CapabilityResult LookupKeyCapability(string threemaId)
		{
			string res = DoGet(new Uri(this.apiUrl + "capabilities/" + threemaId),
					MakeRequestParams());
			if (res != null)
			{
				return new CapabilityResult(threemaId, res.Split(','));
			}
			return null;
		}

		/// <summary>
		/// Lookup an ID by phone number. The phone number will be hashed before
		/// being sent to the server.
		/// </summary>
		/// <param name="phoneNumber">the phone number in E.164 format</param>
		/// <returns>the ID, or null if not found</returns>
		public string LookupPhone(string phoneNumber)
		{
			try
			{
				Dictionary<string, string> getParams = MakeRequestParams();

				byte[] phoneHash = CryptTool.HashPhoneNo(phoneNumber);

				return DoGet(new Uri(this.apiUrl + "lookup/phone_hash/" + DataUtils.ByteArrayToHexString(phoneHash)), getParams);
			}
			catch (FileNotFoundException)
			{
				return null;
			}
		}

		/// <summary>
		/// Download a file given its blob ID.
		/// </summary>
		/// <param name="blobId">The blob ID of the file</param>
		/// <returns>Encrypted file data</returns>
		public byte[] DownloadFile(byte[] blobId)
		{
			return this.DownloadFile(blobId, null);
		}

		/// <summary>
		/// Download a file given its blob ID.
		/// </summary>
		/// <param name="blobId">The blob ID of the file</param>
		/// <param name="progressListener">An object that will receive progress information, or null</param>
		/// <returns>Encrypted file data</returns>
		public byte[] DownloadFile(byte[] blobId, IProgressListener progressListener)
		{
			string queryString = MakeUrlEncoded(MakeRequestParams());
			Uri blobUrl = new Uri(string.Format(this.apiUrl + "blobs/{0}?{1}",
					DataUtils.ByteArrayToHexString(blobId), queryString));

			byte[] blob;

			WebRequest request = WebRequest.CreateHttp(blobUrl);
			request.Method = "GET";
			request.Timeout = 20 * 1000;

			using (WebResponse response = request.GetResponse())
			using (Stream stream = response.GetResponseStream())
			{
				blob = DataUtils.StreamToBytes(stream, progressListener);

				stream.Close();
				response.Close();
			}

			return blob;
		}

		/// <summary>
		/// Upload a file.
		/// </summary>
		/// <param name="fileEncryptionResult">The result of the file encryption (i.e. encrypted file data)</param>
		/// <returns>the result of the upload</returns>
		public UploadResult UploadFile(EncryptResult fileEncryptionResult)
		{
			string attachmentName = "blob";
			string attachmentFileName = "blob.file";
			string crlf = "\r\n";
			string twoHyphens = "--";

			char[] chars = "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
			string boundary = string.Empty;
			Random rand = new Random();
			int count = rand.Next(11) + 30;
			for (int i = 0; i < count; i++)
			{
				boundary += chars[rand.Next(chars.Length)];
			}

			byte[] header = Encoding.UTF8.GetBytes(twoHyphens + boundary + crlf +
					"Content-Disposition: form-data; name=\"" + attachmentName + "\";filename=\"" + attachmentFileName + "\"" + crlf + crlf
				);
			byte[] footer = Encoding.UTF8.GetBytes(crlf + twoHyphens + boundary + twoHyphens + crlf);
			byte[] postData = new byte[header.Length + fileEncryptionResult.Result.Length + footer.Length];

			header.CopyTo(postData, 0);
			fileEncryptionResult.Result.CopyTo(postData, header.Length);
			footer.CopyTo(postData, header.Length + fileEncryptionResult.Result.Length);

			string queryString = MakeUrlEncoded(MakeRequestParams());
			Uri url = new Uri(this.apiUrl + "upload_blob?" + queryString);

			WebRequest request = WebRequest.CreateHttp(url);
			request.Method = "POST";

			if (!WebHeaderCollection.IsRestricted("Connection"))
			{
				request.Headers.Set(HttpRequestHeader.Connection, "Keep-Alive");
			}
			if (!WebHeaderCollection.IsRestricted("CacheControl"))
			{
				request.Headers.Add(HttpRequestHeader.CacheControl, "no-cache");
			}

			request.ContentType = "multipart/form-data;boundary=" + boundary;
			request.ContentLength = postData.Length;

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(postData, 0, postData.Length);
				stream.Close();
			}

			string responseData;
			HttpStatusCode responseCode = GetResponse(request, out responseData);

			return new UploadResult((int)responseCode, responseData != null ? DataUtils.HexStringToByteArray(responseData) : null);
		}

		/// <summary>
		/// Send an end-to-end encrypted message.
		/// </summary>
		/// <param name="to">recipient ID</param>
		/// <param name="nonce">nonce used for encryption (24 bytes)</param>
		/// <param name="box">encrypted message data (max. 4000 bytes)</param>
		/// <returns>message ID</returns>
		public string SendE2EMessage(string to, byte[] nonce, byte[] box)
		{
			Dictionary<string, string> postParams = MakeRequestParams();
			postParams.Add("to", to);
			postParams.Add("nonce", DataUtils.ByteArrayToHexString(nonce));
			postParams.Add("box", DataUtils.ByteArrayToHexString(box));

			return DoPost(new Uri(this.apiUrl + "send_e2e"), postParams);
		}

		/// <summary>
		/// Send a text message with server-side encryption.
		/// </summary>
		/// <param name="to">recipient ID</param>
		/// <param name="text">message text (max. 3500 bytes)</param>
		/// <returns>message ID</returns>
		public string SendTextMessageSimple(string to, string text)
		{
			Dictionary<string, string> postParams = MakeRequestParams();
			postParams.Add("to", to);
			postParams.Add("text", text);

			return DoPost(new Uri(this.apiUrl + "send_simple"), postParams);
		}

		private string DoGet(Uri url, Dictionary<string, string> getParams)
		{
			if (getParams != null)
			{
				string queryString = MakeUrlEncoded(getParams);

				url = new Uri(url.ToString() + "?" + queryString);
			}

			WebRequest request = WebRequest.CreateHttp(url);
			request.Method = "GET";

			string responseData;
			HttpStatusCode responseCode = GetResponse(request, out responseData);
			if (responseCode != HttpStatusCode.OK)
			{
				throw new HttpListenerException((int)responseCode);
			}

			return responseData;
		}

		private string DoPost(Uri url, Dictionary<string, string> postParams)
		{
			ASCIIEncoding encoding = new ASCIIEncoding();
			byte[] postData = encoding.GetBytes(MakeUrlEncoded(postParams));

			WebRequest request = WebRequest.CreateHttp(url);
			request.Method = "POST";
			request.Headers.Add(HttpRequestHeader.AcceptCharset, "utf-8");
			request.ContentType = "application/x-www-form-urlencoded";
			request.ContentLength = postData.Length;

			using (Stream stream = request.GetRequestStream())
			{
				stream.Write(postData, 0, postData.Length);
				stream.Close();
			}

			string responseData;
			HttpStatusCode responseCode = GetResponse(request, out responseData);
			if (responseCode != HttpStatusCode.OK)
			{
				throw new HttpListenerException((int)responseCode);
			}

			return responseData;
		}

		private HttpStatusCode GetResponse(WebRequest request, out string responseData)
		{
			responseData = null;

			WebResponse response = request.GetResponse();
			HttpStatusCode status = ((HttpWebResponse)response).StatusCode;
			if (status == HttpStatusCode.OK)
			{
				using (Stream stream = response.GetResponseStream())
				using (StreamReader reader = new StreamReader(stream))
				{
					responseData = reader.ReadToEnd();

					reader.Close();
					stream.Close();
				}
			}
			response.Close();

			return status;
		}

		private Dictionary<string,string> MakeRequestParams()
		{
			Dictionary<string, string> postParams = new Dictionary<string, string>();
			postParams.Add("from", apiIdentity);
			postParams.Add("secret", secret);
			return postParams;
		}

		private String MakeUrlEncoded(Dictionary<string, string> parameters)
		{
			StringBuilder s = new StringBuilder();

			foreach (KeyValuePair<String,String> param in parameters)
			{
				if (s.Length > 0)
				{
					s.Append('&');
				}

				s.Append(param.Key);
				s.Append('=');
				s.Append(DataUtils.Utf8Endcode(WebUtility.UrlEncode(param.Value)));
			}

			return s.ToString();
		}

		private PublicKeyStore GetSQLiteDbProvider()
		{
			PublicKeyStore store = null;

            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfigurationRoot configuration = builder.Build();
            string connectionString = configuration.GetConnectionString("SQLiteConnectionString");            
            if (!String.IsNullOrWhiteSpace(connectionString))
            {
                store = new PublicKeyStoreDb(connectionString);
            }
            else
            {
                store = new PublicKeyStoreNone();
            }

            return store;
		}
	}
}
