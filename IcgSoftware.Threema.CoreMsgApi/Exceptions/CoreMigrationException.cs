using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Exceptions
{
	public class CoreMigrationException : Exception
	{
        public CoreMigrationException(string message) :
            base(message)
        {
        }

    }
}
