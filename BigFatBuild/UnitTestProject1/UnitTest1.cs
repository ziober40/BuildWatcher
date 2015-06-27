using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            const int minute = 60000;
            Thread.Sleep(0*minute);  
            Assert.AreEqual(1,1);
        }
    }
}
