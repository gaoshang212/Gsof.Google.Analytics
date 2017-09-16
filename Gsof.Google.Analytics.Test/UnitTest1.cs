using System;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Gsof.Google.Analytics.Test
{
    [TestClass]
    public class UnitTest1
    {
        private string tid = Environment.GetEnvironmentVariable("GOOGLE_TRACKINGID");
        [TestMethod]
        public async Task TestMethod1()
        {
            var ga = new Analytics(tid, "A22DA03C-BDF7-11E2-B50E-BC563DC62100", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Electron/1.7.6 Safari/537.36", true);
            var result = await ga.Screenview("test-csharp", "0.0.1", "com.gsof.test", "com.gsof.test", "APP").Append(new { sr = "1920*1080", ul = "zh-CN" }).Send();
            Assert.AreEqual(result, true);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var ga = new Analytics(tid, "A22DA03C-BDF7-11E2-B50E-BC563DC62100", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/58.0.3029.110 Electron/1.7.6 Safari/537.36", true);
            ga.Screenview("test-csharp", "0.0.1", "com.gsof.test", "com.gsof.test", "APP").Append(new { sr = "1920*1080", ul = "zh-CN" });
            var result = ga.GetBody();

            Assert.AreNotEqual(result, null);
        }
    }
}
