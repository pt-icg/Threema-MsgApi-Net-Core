using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Exceptions
{
	public class InvalidKeyException : Exception
	{
		public InvalidKeyException(string message) :
			base(message)
		{
		}

		public InvalidKeyException(string message, Exception innerException) :
			base(message, innerException)
		{
		}
	}
}
