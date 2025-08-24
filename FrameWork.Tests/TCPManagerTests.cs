using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.Threading;

namespace FrameWork.Tests
{
    [TestClass]
    public class TCPManagerTests
    {
        private static ReaderWriterLockSlim GetLock(TCPManager manager)
        {
            var field = typeof(TCPManager).GetField("ClientRWLock", BindingFlags.NonPublic | BindingFlags.Instance);
            return (ReaderWriterLockSlim)field.GetValue(manager);
        }

        [TestMethod]
        public void GetClient_ValidIndex_ReleasesLock()
        {
            var manager = new TCPManager();
            var rwLock = GetLock(manager);

            var result = manager.GetClient(0);
            Assert.IsNull(result);
            Assert.IsFalse(rwLock.IsReadLockHeld);
            Assert.AreEqual(0, rwLock.CurrentReadCount);
        }

        [TestMethod]
        public void GetClient_InvalidIndex_ReleasesLock()
        {
            var manager = new TCPManager();
            var rwLock = GetLock(manager);

            var result = manager.GetClient(-1);
            Assert.IsNull(result);
            Assert.IsFalse(rwLock.IsReadLockHeld);
            Assert.AreEqual(0, rwLock.CurrentReadCount);
        }
    }
}
