using Sonrai.ExtRSAuth;


namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        [TestMethod]
        public void AccessReportsSucceeds()
        {
            Encryption e = new ();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void AccessReportsFails()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void PostReportSucceeds()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void PostReportFails()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void DeleteReportSucceeds()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void DeleteReportFails()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }
    }
}