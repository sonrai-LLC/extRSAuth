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
            Assert.IsTrue(AuthenticationUtilities.VerifyPassword("ExtRSAuth", "This_IS_a_simpl_Passphrase"));
        }

        [TestMethod]
        public void VerifyNullRefPasswordFails()
        {
            Assert.IsFalse(AuthenticationUtilities.VerifyPassword("", ""));
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
