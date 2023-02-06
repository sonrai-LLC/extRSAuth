using Microsoft.ReportingServices.Interfaces;
using Moq;
using Sonrai.ExtRSAuth;
using System.Security.Principal;

namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        AuthenticationExtension authExt = new ();
        private IIdentity? userIdentity;
        private IntPtr userId;
        private Mock<IRSRequestContext> request = new ();

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
        public void VerifyLogonUserStringSucceeds()
        {
            Assert.IsTrue(authExt.LogonUser("ExtRSAuth", "pwd1", "msft"));
        }

        [TestMethod]
        public void VerifyLogonUserNullSucceeds()
        {
            Assert.IsTrue(authExt.LogonUser(null, null, null));
        }

        [TestMethod]
        public void VerifyGetUserInfoSucceeds()
        {
            authExt.GetUserInfo(request.Object, out userIdentity, out userId);
        }

        [ExpectedException(typeof(NullReferenceException))]
        [TestMethod]
        public void VerifyGetUserInfoFails()
        {
            authExt.GetUserInfo(null, out userIdentity, out userId);
        }
    }
}
