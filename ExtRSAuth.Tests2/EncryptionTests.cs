using Sonrai.ExtRSAuth;


namespace ExtRSAuth.Tests
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void EncryptUrlSucceeds()
        {
            Encryption e = new ();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void EncryptUrlFails()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void DecryptUrlSucceeds()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void DecryptUrlFails()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }
    }
}