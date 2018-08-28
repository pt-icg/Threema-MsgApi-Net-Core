using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IcgSoftware.Threema.CoreMsgApi.Test
{
	[TestClass]
	public class DataUtilsTest
	{
		private const string randomNonceBase64Test = "UW9PFWLdoHBKe66Jl88LNUxpgBgRUqwy";

		[TestMethod]
		public void TestHexStringToByteArray()
		{
			byte[] result = DataUtils.HexStringToByteArray(Common.randomNonce);

			Assert.IsNotNull(result);
			Assert.AreEqual(Convert.ToBase64String(result), randomNonceBase64Test);
		}

		[TestMethod]
		public void TestByteArrayToHexString()
		{
			string result = DataUtils.ByteArrayToHexString(Convert.FromBase64String(randomNonceBase64Test));

			Assert.IsNotNull(result);
			Assert.AreEqual(result.ToUpper(), Common.randomNonce.ToUpper());
		}

		[TestMethod]
		public void TestSeekableStreamToBytes()
		{
			byte[] result;

			ASCIIEncoding encoding = new ASCIIEncoding();
			using (MemoryStream stream = new MemoryStream(encoding.GetBytes("test")))
			{
				result = DataUtils.StreamToBytes(stream, null);
				stream.Close();
			}

			Assert.IsNotNull(result);
			CollectionAssert.AreEquivalent(encoding.GetBytes("test").ToList(), result.ToList());
		}
	}
}
