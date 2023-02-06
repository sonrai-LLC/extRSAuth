using Sonrai.ExtRSAuth;

namespace ExtRSAuth.Tests
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void EncryptUrlSucceeds()
        {
            Assert.IsTrue(Encryption.Encrypt("some clear text", "secr3tk3y") == "EjFbXU9jMPxq5+87S1opiqiCNi6JRBcbe7K7AlfD2Fo=");
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void EncryptUrlFails()
        {
            Encryption.Encrypt(null, null);
        }

        [TestMethod]
        public void DecryptUrlSucceeds()
        {
            Assert.IsTrue(Encryption.Decrypt("EjFbXU9jMPxq5+87S1opiqiCNi6JRBcbe7K7AlfD2Fo=", "secr3tk3y") == "some clear text");
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void DecryptUrlFails()
        {
            Encryption.Decrypt(null, null);
        }
    }
}
