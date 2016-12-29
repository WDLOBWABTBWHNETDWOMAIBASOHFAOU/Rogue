using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using Wink;

namespace UnitTestProject
{
    [TestClass]
    public class UnitTest1
    {
        [TestInitialize()]
        public void Initialize()
        {
            Treehugger th = Treehugger.Instance;
        }

        [TestMethod]
        public void TestLevelGeneration()
        {
            Level level = new Level();
            Debug.Write(level.ToString());
        }
    }
}
