using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Messages
{
	/// <summary>
	/// Abstract base class of messages that can be sent with end-to-end encryption via Threema.
	/// </summary>
	public abstract class ThreemaMessage
	{
		public const int NONCEBYTES = 24;
		public const int BLOB_ID_LEN = 16;

		/// <summary>
		/// Get message's raw content
		/// </summary>
		/// <returns></returns>
		public abstract byte[] GetData();

		/// <summary>
		/// Get message's type code
		/// </summary>
		/// <returns></returns>
		public abstract int GetTypeCode();
	}
}
