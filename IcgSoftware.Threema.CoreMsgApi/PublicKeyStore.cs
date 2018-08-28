using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi
{
	/// <summary>
	/// Stores and caches public keys for Threema users. Extend this class to provide your
	/// own storage implementation, e.g. in a file or database.
	/// </summary>
	public abstract class PublicKeyStore
	{
		private readonly static object lockCache = new object();
		private readonly Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();

		/// <summary>
		/// Get the public key for a given Threema ID. The cache is checked first; if it
		/// is not found in the cache, fetchPublicKey() is called.
		/// </summary>
		/// <param name="threemaId">The Threema ID whose public key should be obtained</param>
		/// <returns>The public key, or null if not found</returns>
		public byte[] GetPublicKey(string threemaId)
		{
			lock (lockCache)
			{
				byte[] pk = null;

				if (this.cache.Keys.Contains(threemaId))
				{
					pk = this.cache[threemaId];
				}
				else
				{
					pk = this.FetchPublicKey(threemaId);
					if (pk != null)
					{
						this.cache.Add(threemaId, pk);
					}
				}

				return pk;
			}
		}

		/// <summary>
		/// Store the public key for a given Threema ID in the cache, and the underlying store.
		/// </summary>
		/// <param name="threemaId">The Threema ID whose public key should be stored</param>
		/// <param name="publicKey">The corresponding public key</param>
		public void SetPublicKey(string threemaId, byte[] publicKey)
		{
			if(publicKey != null)
			{
				lock (lockCache)
				{
					this.cache.Add(threemaId, publicKey);
					this.Save(threemaId, publicKey);
				}
			}
		}

		/// <summary>
		/// Fetch the public key for the given Threema ID from the store. Override to provide
		/// your own implementation to read from the store.
		/// </summary>
		/// <param name="threemaId">The Threema ID whose public key should be obtained</param>
		/// <returns>The public key, or null if not found</returns>
		abstract protected byte[] FetchPublicKey(string threemaId);

		/// <summary>
		/// Save the public key for a given Threema ID in the store. Override to provide
		/// your own implementation to write to the store.
		/// </summary>
		/// <param name="threemaId">The Threema ID whose public key should be stored</param>
		/// <param name="publicKey">The corresponding public key</param>
		abstract protected void Save(string threemaId, byte[] publicKey);
	}
}
