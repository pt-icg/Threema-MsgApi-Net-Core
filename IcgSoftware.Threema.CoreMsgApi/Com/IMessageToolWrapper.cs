using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Com
{
	[ComVisible(true)]
	[Guid("E9E32936-CCAB-4297-BAB6-7F3B5939F3D4")]
	public interface IMessageToolWrapper
	{
		string SendTextMessageSimple(string to, string from, string secret, string text, string apiUrl = APIConnector.DEFAULTAPIURL);
		string SendTextMessage(string to, string from, string secret, string privateKey, string text, string apiUrl = APIConnector.DEFAULTAPIURL);
		string SendImageMessage(string to, string from, string secret, string privateKey, string imageFilePath, string apiUrl = APIConnector.DEFAULTAPIURL);
		string SendFileMessage(string to, string from, string secret, string privateKey, string file, string thumbnail = null, string apiUrl = APIConnector.DEFAULTAPIURL);
		string LookupEmail(string email, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL);
		string LookupPhone(string phoneNo, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL);
		string LookupKey(string threemaId, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL);
		ArrayList LookupKeyCapability(string threemaId, string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL);
		int? LookupCredits(string from, string secret, string apiUrl = APIConnector.DEFAULTAPIURL);
		ArrayList ReceiveMessage(string id, string from, string secret, string privateKey, string messageId, string nonce, string box, string outputFolder = null, string apiUrl = APIConnector.DEFAULTAPIURL);
	}
}
