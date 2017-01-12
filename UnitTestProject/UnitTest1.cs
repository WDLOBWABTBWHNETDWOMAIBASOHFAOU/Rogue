using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Wink;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Level level = new Level();
            Debug.Write(level.ToString());
        }
    }
}
