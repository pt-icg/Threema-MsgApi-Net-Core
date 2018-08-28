using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Results
{
	/// <summary>
	/// Result of a capability lookup
	/// </summary>
	public class CapabilityResult
	{
		private readonly string key;
		private readonly string[] capabilities;

		public CapabilityResult(string key, string[] capabilities)
		{
			this.key = key;
			this.capabilities = capabilities;
		}

		public string Key
		{
			get { return key; }
		}

		/// <summary>
		/// Get all capabilities as a string array.
		/// </summary>
		public string[] Capabilities
		{
			get { return capabilities; }
		}

		/// <summary>
		/// Check whether the Threema ID can receive text
		/// </summary>
		public bool CanText
		{
			get { return this.Can("text"); }
		}

		/// <summary>
		/// Check whether the Threema ID can receive images
		/// </summary>
		public bool CanImage
		{
			get { return this.Can("image"); }
		}

		/// <summary>
		/// Check whether the Threema ID can receive videos
		/// </summary>
		public bool CanVideo
		{
			get { return this.Can("video"); }
		}

		/// <summary>
		/// Check whether the Threema ID can receive audio
		/// </summary>
		public bool CanAudio
		{
			get { return this.Can("audio"); }
		}

		/// <summary>
		/// Check whether the Threema ID can receive files
		/// </summary>
		public bool CanFile
		{
			get { return this.Can("file"); }
		}

		public override string ToString()
		{
			StringBuilder b = new StringBuilder();
			b.Append(this.key).Append(": ");
			for (int n = 0; n < this.capabilities.Length; n++) {
				if (n > 0)
				{
					b.Append(",");
				}
				b.Append(this.capabilities[n]);
			}
			return b.ToString();
		}

		private bool Can(string key)
		{
			return this.capabilities.Any(k => k.Equals(key));
		}
	}
}
