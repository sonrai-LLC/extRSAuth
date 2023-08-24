using Sonrai.ExtRSAuth;

namespace ExtRSAuth.Tests
{
    [TestClass]
    public class EncryptionTests
    {
        [TestMethod]
        public void EncryptUrlSucceeds()
        {
            Assert.IsTrue(Encryption.Encrypt("some clear text", "secr3tk3y") == "nNVA3kA4w+Imz4fyhK7/qsF7IUSLMZ/bsa42vAPkFPk=");
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
            Assert.IsTrue(Encryption.Decrypt("nNVA3kA4w+Imz4fyhK7/qsF7IUSLMZ/bsa42vAPkFPk=", "secr3tk3y") == "some clear text");
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void DecryptUrlFails()
        {
            Encryption.Decrypt(null, null);
        }
    }
}
