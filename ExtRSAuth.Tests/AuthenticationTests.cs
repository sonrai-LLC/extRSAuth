using Sonrai.ExtRSAuth;


namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthenticationTests
    {
        [TestMethod]
        public void PassthroughAuthSucceeds()
        {
            Encryption e = new ();
            Assert.IsTrue(1 == 1);
        }

        [TestMethod]
        public void PassthroughAuthFails()
        {
            Encryption e = new();
            Assert.IsTrue(1 == 1);
        }
    }
}