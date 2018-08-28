using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Results
{
	public class EncryptResult
	{
		private readonly byte[] result;
		private readonly byte[] secret;
		private readonly byte[] nonce;

		public EncryptResult(byte[] result, byte[] secret, byte[] nonce)
		{
			this.result = result;
			this.secret = secret;
			this.nonce = nonce;
		}

		/// <summary>
		/// the encrypted data
		/// </summary>
		public byte[] Result
		{
			get { return this.result; }
		}

		/// <summary>
		/// the size (in bytes) of the encrypted data
		/// </summary>
		public int Size
		{
			get { return this.result.Length; }
		}

		/// <summary>
		/// the nonce that was used for encryption
		/// </summary>
		public byte[] Nonce
		{
			get { return this.nonce; }
		}

		/// <summary>
		/// the secret that was used for encryption (only for symmetric encryption, e.g. files)
		/// </summary>
		public byte[] Secret
		{
			get { return secret; }
		}
	}
}
