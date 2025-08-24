using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common;

namespace FrameWork.Tests
{
    [TestClass]
    public class FastRandomTests
    {
        [TestMethod]
        public void RandomInt_ReturnsWithinRange()
        {
            var rand = new FastRandom(12345);
            const int range = 100;
            for (int i = 0; i < 1000; i++)
            {
                int value = rand.randomInt(range);
                Assert.IsTrue(value >= 0 && value < range, $"Value {value} out of range");
            }
        }

        [TestMethod]
        public void SRandomInt_ReturnsWithinRange()
        {
            var rand = new SFastRandom(54321);
            const int range = 100;
            for (int i = 0; i < 1000; i++)
            {
                int value = rand.randomInt(range);
                Assert.IsTrue(value >= 0 && value < range, $"Value {value} out of range");
            }
        }
    }
}
