using Microsoft.ReportingServices.Interfaces;
using Sonrai.ExtRSAuth;
using System.Collections.Specialized;

namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        readonly Authorization auth = new();
        readonly AceCollection aces = new();
        readonly SecurityItemType itemType = SecurityItemType.Folder;

        [Obsolete("This method's AceCollection serialization has been deprecated in .NET 6+")]
        [Ignore]
        [TestMethod]
        public void CreateSecurityDescriptorSucceeds()
        {
            AceStruct ace = new("Everyone");
            aces.Add(ace);
            byte[] bytes = auth.CreateSecurityDescriptor(aces, itemType, out string desc);
            Assert.IsTrue(bytes.Length > 0);
        }

        [TestMethod]
        public void CheckAccessModelOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, ModelOperation.ReadProperties));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, ModelOperation.ReadProperties));
        }

        [TestMethod]
        public void CheckAccessModelItemOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, ModelItemOperation.ReadProperties));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, ModelItemOperation.ReadProperties));
        }

        [TestMethod]
        public void CheckAccessCatalogOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, CatalogOperation.CreateSchedules));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, CatalogOperation.CreateSchedules));
        }

        [TestMethod]
        public void CheckAccessReportOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, ReportOperation.CreateSnapshot));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, ReportOperation.CreateSnapshot));
        }

        [TestMethod]
        public void CheckAccessFolderOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, FolderOperation.CreateFolder));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, FolderOperation.CreateFolder));
        }

        [TestMethod]
        public void CheckAccessFolderArrayOperation()
        {
            FolderOperation[] folderOps = new FolderOperation[2];
            folderOps[0] = FolderOperation.CreateFolder;
            folderOps[1] = FolderOperation.Delete;
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, folderOps));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, folderOps));
        }

        [TestMethod]
        public void CheckAccessResourceOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, ResourceOperation.Comment));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, ResourceOperation.Comment));
        }

        [TestMethod]
        public void CheckAccessResourceArrayOperation()
        {
            {
                ResourceOperation[] folderOps = new ResourceOperation[2];
                folderOps[0] = ResourceOperation.ReadContent;
                folderOps[1] = ResourceOperation.Delete;
                Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, folderOps));
                Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, folderOps));
            }
        }

        [TestMethod]
        public void CheckAccessDataSourceOperation()
        {
            Assert.IsTrue(auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), null, DatasourceOperation.UpdateContent));
            Assert.IsFalse(auth.CheckAccess(null, new IntPtr(), null, DatasourceOperation.UpdateContent));
        }

        [TestMethod]
        public void CheckPermissionsForUserSucceeds()
        {
            StringCollection perms = auth.GetPermissions(AuthenticationUtilities.ExtRsUser, new IntPtr(), SecurityItemType.Report);
            Assert.IsTrue(perms.Count > 50);
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
