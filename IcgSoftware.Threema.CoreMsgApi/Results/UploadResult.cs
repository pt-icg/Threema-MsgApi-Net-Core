using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Results
{
	public class UploadResult
	{
		private readonly int responseCode;
		private readonly byte[] blobId;

		public UploadResult(int responseCode, byte[] blobId)
		{
			this.responseCode = responseCode;
			this.blobId = blobId;
		}

		/// <summary>
		/// the blob ID that has been created
		/// </summary>
		public byte[] BlobId
		{
			get { return this.blobId; }
		}

		/// <summary>
		/// whether the upload succeeded
		/// </summary>
		public bool IsSuccess
		{
			get { return this.responseCode == 200; }
		}

		/// <summary>
		/// the response code of the upload
		/// </summary>
		public int ResponseCode
		{
			get { return this.responseCode; }
		}
	}
}
