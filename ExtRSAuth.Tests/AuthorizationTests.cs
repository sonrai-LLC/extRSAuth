using Microsoft.ReportingServices.Interfaces;
using Sonrai.ExtRSAuth;


namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        readonly Authorization auth = new ();

        [TestMethod]
        public void CreateSecurityDescriptorSucceeds()
        {
            //public byte[] CreateSecurityDescriptor(AceCollection acl, SecurityItemType itemType, out string stringSecDesc)
        }


        [TestMethod]
        public void CheckAccessWithSecDescriptorSucceeds()
        {
            //CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelItemOperation modelItemOperation = ModelItemOperation.ReadProperties)
            //CreateSecurityDescriptor
        }

        [TestMethod]
        public void CheckAccessWithSecDescriptorFails()
        {
            //CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelItemOperation modelItemOperation = ModelItemOperation.ReadProperties)
            //CreateSecurityDescriptor
        }

        [TestMethod]
        public void CheckAccessModelItemOperationSucceeds()
        {
            //   public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelItemOperation modelItemOperation = ModelItemOperation.ReadProperties)
        }

        [TestMethod]
        public void CheckAccessModelItemOperationFails()
        {
            //   public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelItemOperation modelItemOperation = ModelItemOperation.ReadProperties)
        }

        [TestMethod]
        public void CheckAccessModelOperationSucceeds()
        {
            // public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelOperation modelOperation = ModelOperation.ReadProperties)
        }

        [TestMethod]
        public void CheckAccessModelOperationFails()
        {
            // public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelOperation modelOperation = ModelOperation.ReadProperties)
        }

        [TestMethod]
        public void CheckAccessCatalogOpSucceeds()
        {
            //CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, CatalogOperation[] requiredOperations = null)
        }

        [TestMethod]
        public void CheckAccessCatalogOpFails()
        {
            //CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, CatalogOperation[] requiredOperations = null)
        }

        [TestMethod]
        public void CheckAccessReportOpSucceeds()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ReportOperation requiredOperation = ReportOperation.ReadProperties)
        }

        [TestMethod]
        public void CheckAccessReportOpFails()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ReportOperation requiredOperation = ReportOperation.ReadProperties)
        }

        [TestMethod]
        public void CheckAccessFolderOpSucceeds()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, FolderOperation requiredOperation = FolderOperation.ReadAuthorizationPolicy)
        }

        [TestMethod]
        public void CheckAccessFolderOpFails()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, FolderOperation requiredOperation = FolderOperation.ReadAuthorizationPolicy)
        }

        [TestMethod]
        public void CheckAccessArrayFolderOpSucceeds()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, FolderOperation[] requiredOperations = null)
        }

        [TestMethod]
        public void CheckAccessArrayFolderOpFails()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, FolderOperation[] requiredOperations = null)
        }

        [TestMethod]
        public void CheckAccessResourceOpSucceeds()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ResourceOperation requiredOperation = ResourceOperation.ReadAuthorizationPolicy)
        }

        [TestMethod]
        public void CheckAccessResourceOpFails()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ResourceOperation requiredOperation = ResourceOperation.ReadAuthorizationPolicy)
        }

        [TestMethod]
        public void CheckAccessArrayResourceOpSucceeds()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ResourceOperation[] requiredOperations = null)
        }

        [TestMethod]
        public void CheckAccessArrayResourceOpFails()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ResourceOperation[] requiredOperations = null)
        }

        [TestMethod]
        public void CheckAccessDataSourceOpSucceeds()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, DatasourceOperation requiredOperation = DatasourceOperation.ReadAuthorizationPolicy)
        }

        [TestMethod]
        public void CheckAccessDataSourceOpFails()
        {
            //public bool CheckAccess(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), byte[] secDesc = null, DatasourceOperation requiredOperation = DatasourceOperation.ReadAuthorizationPolicy)
        }

        [TestMethod]
        public void CheckPermissionsForUserSucceeds()
        {
            //public StringCollection GetPermissions(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), SecurityItemType itemType = SecurityItemType.Unknown, byte[] secDesc = null)
        }

        [TestMethod]
        public void CheckPermissionsForUserFails()
        {
            //public StringCollection GetPermissions(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), SecurityItemType itemType = SecurityItemType.Unknown, byte[] secDesc = null)
        }

        [TestMethod]
        public void DeserializeAclSucceeds()
        {
            //public StringCollection GetPermissions(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), SecurityItemType itemType = SecurityItemType.Unknown, byte[] secDesc = null)
        }

        [TestMethod]
        public void DeserializeAclFails()
        {
            //public StringCollection GetPermissions(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), SecurityItemType itemType = SecurityItemType.Unknown, byte[] secDesc = null)
        }

        [TestMethod]
        public void SetConfigurationSucceeds()
        {
            auth.SetConfiguration(
           @"<AdminConfiguration>
                <UserName>ExtRSAuth</UserName>
            </AdminConfiguration>");
        }

        [TestMethod]
        public void LocalizedNameIsExtRSAdminUserSucceeds()
        {
            Assert.IsTrue(auth.LocalizedName == "ExtRSAuth");
        }
    }
}
