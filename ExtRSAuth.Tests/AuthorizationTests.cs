using Microsoft.ReportingServices.Interfaces;
using Sonrai.ExtRSAuth;
using System.Collections.Specialized;

namespace ExtRSAuth.Tests
{
    [TestClass]
    public class AuthorizationTests
    {
        private readonly Authorization _auth = new();
        private readonly AceCollection _aces = new();
        private readonly SecurityItemType _itemType = SecurityItemType.Folder;
        private byte[] _bytes = [];

        [TestInitialize]
        public void Initialize()
        {
            AceStruct ace = new("extRSAuth");
            ace.ModelOperations.Add(ModelOperation.ReadProperties);
            ace.ModelItemOperations.Add(ModelItemOperation.ReadProperties);
            ace.CatalogOperations.Add(CatalogOperation.ReadSchedules);
            ace.ReportOperations.Add(ReportOperation.CreateSnapshot);
            ace.ResourceOperations.Add(ResourceOperation.ReadContent);
            ace.ResourceOperations.Add(ResourceOperation.Delete);
            ace.ResourceOperations.Add(ResourceOperation.Comment);
            ace.FolderOperations.Add(FolderOperation.CreateFolder);
            ace.FolderOperations.Add(FolderOperation.Delete);
            ace.DatasourceOperations.Add(DatasourceOperation.UpdateContent);
            ace.CatalogOperations.Add(CatalogOperation.CreateSchedules);

            _aces.Add(ace);
            _bytes = _auth.CreateSecurityDescriptor(_aces, _itemType, out string desc);
        }

        [TestMethod]
        public void CreateSecurityDescriptorSucceeds()
        {
            AceStruct ace = new("extRSAuth");
            _aces.Add(ace);
            byte[] bytes = _auth.CreateSecurityDescriptor(_aces, _itemType, out string desc);
            Assert.IsTrue(bytes.Length > 0);
        }

        [TestMethod]
        public void CheckAccessModelOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, ModelOperation.ReadProperties));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, ModelOperation.ReadProperties));
        }

        [TestMethod]
        public void CheckAccessModelItemOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, ModelItemOperation.ReadProperties));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, ModelItemOperation.ReadProperties));
        }

        [TestMethod]
        public void CheckAccessCatalogOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, CatalogOperation.CreateSchedules));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, CatalogOperation.CreateSchedules));
        }

        [TestMethod]
        public void CheckAccessReportOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, ReportOperation.CreateSnapshot));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, ReportOperation.CreateSnapshot));
        }

        [TestMethod]
        public void CheckAccessFolderOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, FolderOperation.CreateFolder));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, FolderOperation.CreateFolder));
        }

        [TestMethod]
        public void CheckAccessFolderArrayOperation()
        {
            FolderOperation[] folderOps = new FolderOperation[2];
            folderOps[0] = FolderOperation.CreateFolder;
            folderOps[1] = FolderOperation.Delete;
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, folderOps));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, folderOps));
        }

        [TestMethod]
        public void CheckAccessResourceOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, ResourceOperation.Comment));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, ResourceOperation.Comment));
        }

        [TestMethod]
        public void CheckAccessResourceArrayOperation()
        {
            {
                ResourceOperation[] folderOps = new ResourceOperation[2];
                folderOps[0] = ResourceOperation.ReadContent;
                folderOps[1] = ResourceOperation.Delete;
                Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, folderOps));
                Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, folderOps));
            }
        }

        [TestMethod]
        public void CheckAccessDataSourceOperation()
        {
            Assert.IsTrue(_auth.CheckAccess(AuthenticationUtilities.ExtRsUser, new IntPtr(), _bytes, DatasourceOperation.UpdateContent));
            Assert.IsFalse(_auth.CheckAccess(null, new IntPtr(), null, DatasourceOperation.UpdateContent));
        }

        [TestMethod]
        public void CheckPermissionsForUserSucceeds()
        {
            StringCollection perms = _auth.GetPermissions(AuthenticationUtilities.ExtRsUser, new IntPtr(), SecurityItemType.Report);
            Assert.IsTrue(perms.Count > 50);
        }

        [TestMethod]
        public void SetConfigurationSucceeds()
        {
            _auth.SetConfiguration(
           @"<AdminConfiguration>
                <UserName>extRSAuth</UserName>
            </AdminConfiguration>");
        }
    }
}
