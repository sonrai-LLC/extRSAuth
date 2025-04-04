#region
// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)
/*============================================================================
  File:     Authorization.cs

  Summary:  Demonstrates an implementation of an authorization 
            extension.
------------------------------------------------------------------------------
  This file is part of Microsoft SQL Server Code Samples.
    
 This source code is intended only as a supplement to Microsoft
 Development Tools and/or on-line documentation. See these other
 materials for detailed information regarding Microsoft code 
 samples.

 THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF 
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 PARTICULAR PURPOSE.
===========================================================================*/
#endregion

using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.ReportingServices.Interfaces;
using System.Xml;
using System.Web;
using System.Diagnostics.Eventing.Reader;

namespace Sonrai.ExtRSAuth
{
    public class Authorization : IAuthorizationExtension
    {
        private static string m_adminUserName = AuthenticationUtilities.ExtRsUser;
        static Authorization()
        {
            InitializeMaps();
        }

        public byte[] CreateSecurityDescriptor(AceCollection acl, SecurityItemType itemType, out string stringSecDesc)
        {
            // Creates a memory stream and serializes the ACL for storage.
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream result = new MemoryStream())
            {
                bf.Serialize(result, acl);
                stringSecDesc = null;
                return result.GetBuffer();
            }
        }

        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelItemOperation modelItemOperation = ModelItemOperation.ReadProperties)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (ModelItemOperation aclOperation in ace.ModelItemOperations)
                    {
                        if (aclOperation == modelItemOperation)
                            return true;
                    }
                }
            }

            return false;
        }

        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ModelOperation modelOperation = ModelOperation.ReadProperties)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (ModelOperation aclOperation in ace.ModelOperations)
                        if (aclOperation == modelOperation)
                            return true;
                }
            }

            return false;
        }

        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, CatalogOperation requiredOperation = CatalogOperation.ReadRoleProperties)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (CatalogOperation aclOperation in ace.CatalogOperations)
                        if (aclOperation == requiredOperation)
                            return true;
                }
            }

            return false;
        }

        // Overload for array of Catalog operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, CatalogOperation[] requiredOperations = null)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            foreach (CatalogOperation operation in requiredOperations)
                if (!CheckAccess(userName, userToken, secDesc, operation))
                    return false;

            return true;
        }

        // Overload for Report operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ReportOperation requiredOperation = ReportOperation.ReadProperties)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (ReportOperation aclOperation in ace.ReportOperations)
                        if (aclOperation == requiredOperation)
                            return true;
                }
            }

            return false;
        }

        public bool CheckUserToken(IntPtr token, string userName)
        {
            return token != null && userName != null;
        }

        // Overload for Folder operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, FolderOperation requiredOperation = FolderOperation.ReadAuthorizationPolicy)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (FolderOperation aclOperation in ace.FolderOperations)
                        if (aclOperation == requiredOperation)
                            return true;
                }
            }

            return false;
        }

        // Overload for an array of Folder operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, FolderOperation[] requiredOperations = null)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            foreach (FolderOperation operation in requiredOperations)
            if (!CheckAccess(userName, userToken, secDesc, operation))
                    return false;

            return true;
        }

        // Overload for Resource operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ResourceOperation requiredOperation = ResourceOperation.ReadAuthorizationPolicy)
        {
#if DEBUG
            if (AuthenticationUtilities.NoAuth)
            {
                return true;
            }
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (ResourceOperation aclOperation in ace.ResourceOperations)
                        if (aclOperation == requiredOperation)
                            return true;
                }
            }

            return false;
        }

        // Overload for an array of Resource operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, ResourceOperation[] requiredOperations = null)
        {
            //  return true;

            foreach (ResourceOperation operation in requiredOperations)
                if (!CheckAccess(userName, userToken, secDesc, operation))
                    return false;

            return true;
        }

        // Overload for Datasource operations
        public bool CheckAccess(string userName, IntPtr userToken = new IntPtr(), byte[] secDesc = null, DatasourceOperation requiredOperation = DatasourceOperation.ReadAuthorizationPolicy)
        {
#if DEBUG
            if(AuthenticationUtilities.NoAuth)
            {
                return true;
            }          
#endif
            AceCollection acl = DeserializeAcl(secDesc);
            foreach (AceStruct ace in acl)
            {
                if (0 == string.Compare(userName, ace.PrincipalName, true, CultureInfo.CurrentCulture))
                {
                    foreach (DatasourceOperation aclOperation in ace.DatasourceOperations)
                    {
                        if (aclOperation == requiredOperation)
                            return true;
                    }
                }
            }

            return false;
        }

        public StringCollection GetPermissions(string userName = AuthenticationUtilities.ExtRsUser, IntPtr userToken = new IntPtr(), SecurityItemType itemType = SecurityItemType.Unknown, byte[] secDesc = null)
        {
            StringCollection permissions = new StringCollection();
            if (IsAdmin(userName))
            {
                permissions.AddRange(_fullPermissions.ToArray());
            }
            else
            {
                AceCollection acl = DeserializeAcl(secDesc);
                foreach (AceStruct ace in acl)
                {
                    if (0 == String.Compare(userName, ace.PrincipalName, true,
                          CultureInfo.CurrentCulture))
                    {
                        foreach (ModelItemOperation aclOperation in ace.ModelItemOperations)
                        {
                            if (!permissions.Contains(_modelItemOperNames[aclOperation]))
                                permissions.Add(_modelItemOperNames[aclOperation]);
                        }
                        foreach (ModelOperation aclOperation in ace.ModelOperations)
                        {
                            if (!permissions.Contains(_modelOperNames[aclOperation]))
                                permissions.Add(_modelOperNames[aclOperation]);
                        }
                        foreach (CatalogOperation aclOperation in ace.CatalogOperations)
                        {
                            if (!permissions.Contains(_catalogOperationNames[aclOperation]))
                                permissions.Add(_catalogOperationNames[aclOperation]);
                        }
                        foreach (ReportOperation aclOperation in ace.ReportOperations)
                        {
                            if (!permissions.Contains(_reportOperationNames[aclOperation]))
                                permissions.Add(_reportOperationNames[aclOperation]);
                        }
                        foreach (FolderOperation aclOperation in ace.FolderOperations)
                        {
                            if (!permissions.Contains(_folderOperationNames[aclOperation]))
                                permissions.Add(_folderOperationNames[aclOperation]);
                        }
                        foreach (ResourceOperation aclOperation in ace.ResourceOperations)
                        {
                            if (!permissions.Contains(_resourceOperationNames[aclOperation]))
                                permissions.Add(_resourceOperationNames[aclOperation]);
                        }
                        foreach (DatasourceOperation aclOperation in ace.DatasourceOperations)
                        {
                            if (!permissions.Contains(_dataSourceOperationNames[aclOperation]))
                                permissions.Add(_dataSourceOperationNames[aclOperation]);
                        }
                    }
                }
            }

            return permissions;
        }

        // Used to deserialize the ACL that is stored by the report server.
        private AceCollection DeserializeAcl(byte[] secDesc = null)
        {
            AceCollection acl = new AceCollection();
            if (secDesc != null)
            {
                BinaryFormatter bf = new BinaryFormatter();
                using (MemoryStream sdStream = new MemoryStream(secDesc))
                {
                    acl = (AceCollection)bf.Deserialize(sdStream);
                }
            }

            return acl;
        }

        private static readonly Dictionary<ModelItemOperation, string> _modelItemOperNames = new Dictionary<ModelItemOperation, string>();
        private static readonly Dictionary<ModelOperation, string> _modelOperNames = new Dictionary<ModelOperation, string>();
        private static readonly Dictionary<CatalogOperation, string> _catalogOperationNames = new Dictionary<CatalogOperation, string>();
        private static readonly Dictionary<FolderOperation, string> _folderOperationNames = new Dictionary<FolderOperation, string>();
        private static readonly Dictionary<ReportOperation, string> _reportOperationNames = new Dictionary<ReportOperation, string>();
        private static readonly Dictionary<ResourceOperation, string> _resourceOperationNames = new Dictionary<ResourceOperation, string>();
        private static readonly Dictionary<DatasourceOperation, string> _dataSourceOperationNames = new Dictionary<DatasourceOperation, string>();
        private static readonly List<string> _fullPermissions = new List<string>();

        public static void InitializeMaps()
        {
            // create model operation names data
            if (!_modelItemOperNames.ContainsValue(OperationNames.OperReadProperties))
                _modelItemOperNames.Add(ModelItemOperation.ReadProperties, OperationNames.OperReadProperties);

            if (_modelItemOperNames.Count != Enum.GetValues(typeof(ModelItemOperation)).Length)
                throw new Exception("Model item name mismatch");

            // create model operation names data
            if (!_modelOperNames.ContainsValue(OperationNames.OperDelete))
            {
                _modelOperNames.Add(ModelOperation.Delete, OperationNames.OperDelete);
                _modelOperNames.Add(ModelOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
                _modelOperNames.Add(ModelOperation.ReadContent, OperationNames.OperReadContent);
                _modelOperNames.Add(ModelOperation.ReadDatasource, OperationNames.OperReadDatasources);
                _modelOperNames.Add(ModelOperation.ReadModelItemAuthorizationPolicies, OperationNames.OperReadModelItemSecurityPolicies);
                _modelOperNames.Add(ModelOperation.ReadProperties, OperationNames.OperReadProperties);
                _modelOperNames.Add(ModelOperation.UpdateContent, OperationNames.OperUpdateContent);
                _modelOperNames.Add(ModelOperation.UpdateDatasource, OperationNames.OperUpdateDatasources);
                _modelOperNames.Add(ModelOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
                _modelOperNames.Add(ModelOperation.UpdateModelItemAuthorizationPolicies, OperationNames.OperUpdateModelItemSecurityPolicies);
                _modelOperNames.Add(ModelOperation.UpdateProperties, OperationNames.OperUpdatePolicy);
            }

            if (_modelOperNames.Count != Enum.GetValues(typeof(ModelOperation)).Length)
                throw new Exception("Model name mismatch");

            // create operation names data
            _catalogOperationNames.Add(CatalogOperation.CreateRoles, OperationNames.OperCreateRoles);
            _catalogOperationNames.Add(CatalogOperation.DeleteRoles, OperationNames.OperDeleteRoles);
            _catalogOperationNames.Add(CatalogOperation.ReadRoleProperties, OperationNames.OperReadRoleProperties);
            _catalogOperationNames.Add(CatalogOperation.UpdateRoleProperties, OperationNames.OperUpdateRoleProperties);
            _catalogOperationNames.Add(CatalogOperation.ReadSystemProperties, OperationNames.OperReadSystemProperties);
            _catalogOperationNames.Add(CatalogOperation.UpdateSystemProperties, OperationNames.OperUpdateSystemProperties);
            _catalogOperationNames.Add(CatalogOperation.GenerateEvents, OperationNames.OperGenerateEvents);
            _catalogOperationNames.Add(CatalogOperation.ReadSystemSecurityPolicy, OperationNames.OperReadSystemSecurityPolicy);
            _catalogOperationNames.Add(CatalogOperation.UpdateSystemSecurityPolicy, OperationNames.OperUpdateSystemSecurityPolicy);
            _catalogOperationNames.Add(CatalogOperation.CreateSchedules, OperationNames.OperCreateSchedules);
            _catalogOperationNames.Add(CatalogOperation.DeleteSchedules, OperationNames.OperDeleteSchedules);
            _catalogOperationNames.Add(CatalogOperation.ReadSchedules, OperationNames.OperReadSchedules);
            _catalogOperationNames.Add(CatalogOperation.UpdateSchedules, OperationNames.OperUpdateSchedules);
            _catalogOperationNames.Add(CatalogOperation.ListJobs, OperationNames.OperListJobs);
            _catalogOperationNames.Add(CatalogOperation.CancelJobs, OperationNames.OperCancelJobs);
            _catalogOperationNames.Add(CatalogOperation.ExecuteReportDefinition, OperationNames.ExecuteReportDefinition);

            if (_catalogOperationNames.Count != Enum.GetValues(typeof(CatalogOperation)).Length)
                //Catalog name mismatch
                throw new Exception("Catalog name mismatch");

            _folderOperationNames.Add(FolderOperation.CreateFolder, OperationNames.OperCreateFolder);
            _folderOperationNames.Add(FolderOperation.Delete, OperationNames.OperDelete);
            _folderOperationNames.Add(FolderOperation.ReadProperties, OperationNames.OperReadProperties);
            _folderOperationNames.Add(FolderOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _folderOperationNames.Add(FolderOperation.CreateReport, OperationNames.OperCreateReport);
            _folderOperationNames.Add(FolderOperation.CreateResource, OperationNames.OperCreateResource);
            _folderOperationNames.Add(FolderOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _folderOperationNames.Add(FolderOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _folderOperationNames.Add(FolderOperation.CreateDatasource, OperationNames.OperCreateDatasource);
            _folderOperationNames.Add(FolderOperation.CreateModel, OperationNames.OperCreateModel);
            if (_folderOperationNames.Count != Enum.GetValues(typeof(FolderOperation)).Length)
                //Folder name mismatch
                throw new Exception("Folder name mismatch");

            _reportOperationNames.Add(ReportOperation.Delete, OperationNames.OperDelete);
            _reportOperationNames.Add(ReportOperation.ReadProperties, OperationNames.OperReadProperties);
            _reportOperationNames.Add(ReportOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _reportOperationNames.Add(ReportOperation.UpdateParameters, OperationNames.OperUpdateParameters);
            _reportOperationNames.Add(ReportOperation.ReadDatasource, OperationNames.OperReadDatasources);
            _reportOperationNames.Add(ReportOperation.UpdateDatasource, OperationNames.OperUpdateDatasources);
            _reportOperationNames.Add(ReportOperation.ReadReportDefinition, OperationNames.OperReadReportDefinition);
            _reportOperationNames.Add(ReportOperation.UpdateReportDefinition, OperationNames.OperUpdateReportDefinition);
            _reportOperationNames.Add(ReportOperation.CreateSubscription, OperationNames.OperCreateSubscription);
            _reportOperationNames.Add(ReportOperation.DeleteSubscription, OperationNames.OperDeleteSubscription);
            _reportOperationNames.Add(ReportOperation.ReadSubscription, OperationNames.OperReadSubscription);
            _reportOperationNames.Add(ReportOperation.UpdateSubscription, OperationNames.OperUpdateSubscription);
            _reportOperationNames.Add(ReportOperation.CreateAnySubscription, OperationNames.OperCreateAnySubscription);
            _reportOperationNames.Add(ReportOperation.DeleteAnySubscription, OperationNames.OperDeleteAnySubscription);
            _reportOperationNames.Add(ReportOperation.ReadAnySubscription, OperationNames.OperReadAnySubscription);
            _reportOperationNames.Add(ReportOperation.UpdateAnySubscription, OperationNames.OperUpdateAnySubscription);
            _reportOperationNames.Add(ReportOperation.UpdatePolicy, OperationNames.OperUpdatePolicy);
            _reportOperationNames.Add(ReportOperation.ReadPolicy, OperationNames.OperReadPolicy);
            _reportOperationNames.Add(ReportOperation.DeleteHistory, OperationNames.OperDeleteHistory);
            _reportOperationNames.Add(ReportOperation.ListHistory, OperationNames.OperListHistory);
            _reportOperationNames.Add(ReportOperation.ExecuteAndView, OperationNames.OperExecuteAndView);
            _reportOperationNames.Add(ReportOperation.CreateResource, OperationNames.OperCreateResource);
            _reportOperationNames.Add(ReportOperation.CreateSnapshot, OperationNames.OperCreateSnapshot);
            _reportOperationNames.Add(ReportOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _reportOperationNames.Add(ReportOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _reportOperationNames.Add(ReportOperation.Execute, OperationNames.OperExecute);
            _reportOperationNames.Add(ReportOperation.CreateLink, OperationNames.OperCreateLink);
            _reportOperationNames.Add(ReportOperation.Comment, OperationNames.OperComment);
            _reportOperationNames.Add(ReportOperation.ManageComments, OperationNames.OperManageComments);
            if (_reportOperationNames.Count != Enum.GetValues(typeof(ReportOperation)).Length)
                throw new Exception("Report name mismatch");

            _resourceOperationNames.Add(ResourceOperation.Delete, OperationNames.OperDelete);
            _resourceOperationNames.Add(ResourceOperation.ReadProperties, OperationNames.OperReadProperties);
            _resourceOperationNames.Add(ResourceOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _resourceOperationNames.Add(ResourceOperation.ReadContent, OperationNames.OperReadContent);
            _resourceOperationNames.Add(ResourceOperation.UpdateContent, OperationNames.OperUpdateContent);
            _resourceOperationNames.Add(ResourceOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _resourceOperationNames.Add(ResourceOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            _resourceOperationNames.Add(ResourceOperation.Comment, OperationNames.OperComment);
            _resourceOperationNames.Add(ResourceOperation.ManageComments, OperationNames.OperManageComments);
            if (_resourceOperationNames.Count != Enum.GetValues(typeof(ResourceOperation)).Length)
                throw new Exception("Resource name mismatch");

            _dataSourceOperationNames.Add(DatasourceOperation.Delete, OperationNames.OperDelete);
            _dataSourceOperationNames.Add(DatasourceOperation.ReadProperties, OperationNames.OperReadProperties);
            _dataSourceOperationNames.Add(DatasourceOperation.UpdateProperties, OperationNames.OperUpdateProperties);
            _dataSourceOperationNames.Add(DatasourceOperation.ReadContent, OperationNames.OperReadContent);
            _dataSourceOperationNames.Add(DatasourceOperation.UpdateContent, OperationNames.OperUpdateContent);
            _dataSourceOperationNames.Add(DatasourceOperation.ReadAuthorizationPolicy, OperationNames.OperReadAuthorizationPolicy);
            _dataSourceOperationNames.Add(DatasourceOperation.UpdateDeleteAuthorizationPolicy, OperationNames.OperUpdateDeleteAuthorizationPolicy);
            if (_dataSourceOperationNames.Count != Enum.GetValues(typeof(DatasourceOperation)).Length)
                throw new Exception("Datasource name mismatch");

            foreach (CatalogOperation oper in _catalogOperationNames.Keys)
                if (!_fullPermissions.Contains(_catalogOperationNames[oper]))
                    _fullPermissions.Add(_catalogOperationNames[oper]);

            foreach (ModelItemOperation oper in _modelItemOperNames.Keys)
                if (!_fullPermissions.Contains(_modelItemOperNames[oper]))
                    _fullPermissions.Add(_modelItemOperNames[oper]);

            foreach (ModelOperation oper in _modelOperNames.Keys)
                if (!_fullPermissions.Contains(_modelOperNames[oper]))
                    _fullPermissions.Add(_modelOperNames[oper]);

            foreach (CatalogOperation oper in _catalogOperationNames.Keys)
                if (!_fullPermissions.Contains(_catalogOperationNames[oper]))
                    _fullPermissions.Add(_catalogOperationNames[oper]);

            foreach (ReportOperation oper in _reportOperationNames.Keys)
                if (!_fullPermissions.Contains(_reportOperationNames[oper]))
                    _fullPermissions.Add(_reportOperationNames[oper]);

            foreach (FolderOperation oper in _folderOperationNames.Keys)
                if (!_fullPermissions.Contains(_folderOperationNames[oper]))
                    _fullPermissions.Add(_folderOperationNames[oper]);

            foreach (ResourceOperation oper in _resourceOperationNames.Keys)
                if (!_fullPermissions.Contains(_resourceOperationNames[oper]))
                    _fullPermissions.Add(_resourceOperationNames[oper]);

            foreach (DatasourceOperation oper in _dataSourceOperationNames.Keys)
                if (!_fullPermissions.Contains(_dataSourceOperationNames[oper]))
                    _fullPermissions.Add(_dataSourceOperationNames[oper]);
        }

        private bool IsAdmin(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            if (userName.Equals(m_adminUserName, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public void SetConfiguration(string configuration)
        {
            if (HttpContext.Current != null && !HttpContext.Current.Request.IsLocal && HttpContext.Current.Request.Url.AbsolutePath.Contains("/ReportServer/ReportService2010.asmx"))
            {
                throw new Exception("Cannot access internal report server operations from external machines");
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(configuration);
            if (doc.DocumentElement.Name == "AdminConfiguration")
            {
                foreach (XmlNode child in doc.DocumentElement.ChildNodes)
                    if (child.Name == "UserName")
                        m_adminUserName = child.InnerText;
                    else
                        throw new Exception(string.Format(CultureInfo.InvariantCulture, CustomSecurity.UnrecognizedElement));
            }
            else
                throw new Exception(string.Format(CultureInfo.InvariantCulture, CustomSecurity.AdminConfiguration));
        }

        public string LocalizedName
        {
            get
            {
                // Return a localized name for this extension
                return AuthenticationUtilities.ExtRsUser;
            }
        }
    }
}
