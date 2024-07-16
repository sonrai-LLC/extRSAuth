#region
// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)
/*============================================================================
  File:      AuthenticationExtension.cs

  Summary:  Demonstrates an implementation of an authentication 
            extension.
--------------------------------------------------------------------
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

using Microsoft.ReportingServices.Interfaces;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Security;
using System.Xml;

namespace Sonrai.ExtRSAuth
{

    public class AuthenticationExtension : IAuthenticationExtension2, IExtension
    {
        public void SetConfiguration(string configuration)
        {
            if (!string.IsNullOrEmpty(configuration))
            {
                var doc = new XmlDocument();
                doc.LoadXml(configuration);
            }
        }

        public string LocalizedName
        {
            get
            {
                return AuthenticationUtilities.ExtRsUser;
            }
        }

        public bool LogonUser(string userName, string password, string authority)
        {
            return AuthenticationUtilities.VerifyPassword(password);
        }

        public void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
        {
            if (HttpContext.Current.Request.IsLocal
                && HttpContext.Current.Items["OriginalUrl"].ToString() == AuthenticationUtilities.ReportExecution2005SOAP
                || (HttpContext.Current.Items["OriginalUrl"].ToString() == AuthenticationUtilities.ReportService2010SOAP))
            {
                FormsAuthentication.SetAuthCookie(AuthenticationUtilities.ExtRsUser, true);
                userIdentity = new GenericIdentity(AuthenticationUtilities.MSBIToolsUser);
            }
            if (HttpContext.Current.User != null) //ttpContext.Current.Request.IsLocal && 
            {
                FormsAuthentication.SetAuthCookie(HttpContext.Current.User.Identity.Name, true);
                userIdentity = HttpContext.Current.User.Identity;
            }
            else
                userIdentity = new GenericIdentity(HttpContext.Current.User.Identity.Name); //make user account

            // initialize a pointer to the current user id to zero
            userId = IntPtr.Zero;
        }

        //adding new GetUserInfo method for IAuthenticationExtension2
        public void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
        {
            userIdentity = null;
            if (requestContext.User != null) // && requestContext.User.Name == AuthenticationUtilities.ExtRsUser)
                userIdentity = requestContext.User;

            // initialize a pointer to the current user id to zero
            userId = IntPtr.Zero;
        }

        public bool IsValidPrincipalName(string principalName)
        {
            return VerifyUser(principalName);
        }

        public static bool VerifyUser(string userName)
        {
            return true; //already auth'd
        }
    }
}
