using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Uaeglp.Services;
using Uaeglp.Services.Extensions;
using Uaeglp.Utilities;

namespace Uaeglp.Tests
{
	[TestClass]
	public class UnitTestUtilities
	{
		[TestMethod]
		public void TestPasswordHashing()
		{
			var hashing = new PasswordHashing();
			var hashedPassword = hashing.CreateHash("test");
			var result = hashing.ValidatePassword("test", hashedPassword);
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void TestMongo()
        {
            var tt = new RijndaelEncryption(new Random());

            var mm = tt.Encrypt(9.ToString());

            var de = tt.Decrypt<int>("9XQsOnz0EkOYnJVC40KqSOAHs5cjl1ZbsS8sB88mPss");

            var nn = "";
        }
	}
}
