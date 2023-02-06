using Sonrai.ExtRSAuth;

namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        AuthenticationExtension authExt = new ();

        [TestMethod]
        public void VerifyValidStringPasswordSucceeds()
        {
            Assert.IsTrue(AuthenticationUtilities.VerifyPassword("some1", "some2"));
        }

        [TestMethod]
        public void VerifyNullRefPasswordSucceeds()
        {
            Assert.IsTrue(AuthenticationUtilities.VerifyPassword(null, null));
        }

        [TestMethod]
        public void VerifyUserStringSucceeds()
        {
            Assert.IsTrue(AuthenticationExtension.VerifyUser("some1"));
        }

        [TestMethod]
        public void VerifyNullUserSucceeds()
        {
            Assert.IsTrue(AuthenticationExtension.VerifyUser(null));
        }

        [TestMethod]
        public void VerifyAdminName()
        {
            Assert.IsTrue(authExt.LocalizedName == "ExtRSAuth");
        }

        [TestMethod]
        public void VerifyLogonUserSucceeds()
        {
            Assert.IsTrue(authExt.LogonUser("ExtRSAuth", "pwd1", "msft"));
        }

        [TestMethod]
        public void VerifyLogonUserSucceeds2()
        {
            Assert.IsTrue(authExt.LogonUser(null, null, null));
        }
    }
}
