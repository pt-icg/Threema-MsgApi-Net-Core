using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi
{
	public interface IProgressListener
	{
		/// <summary>
		/// Update the progress of an upload/download process.
		/// </summary>
		/// <param name="progress">in percent (0..100)</param>
		void updateProgress(int progress);
	}
}
