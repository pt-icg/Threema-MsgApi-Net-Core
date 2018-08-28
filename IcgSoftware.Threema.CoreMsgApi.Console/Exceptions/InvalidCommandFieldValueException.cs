using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Console.Exceptions
{
	class InvalidCommandFieldValueException : Exception
	{
		public InvalidCommandFieldValueException(string message) :
			base(message)
		{
		}
	}
}
