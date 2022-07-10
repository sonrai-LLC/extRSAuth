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

using System;
using System.Web;
using System.Security.Principal;
using System.Xml;
using Microsoft.ReportingServices.Interfaces;
using System.Web.Security;

namespace Sonrai.ExtRSAuth
{

    public class AuthenticationExtension : IAuthenticationExtension2, IExtension
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void SetConfiguration(string configuration)
        {
            if (!string.IsNullOrEmpty(configuration))
            {
                var doc = new XmlDocument();
                doc.LoadXml(configuration);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public string LocalizedName
        {
            get
            {
                return @"Daylite";
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public bool LogonUser(string userName, string password, string authority)
        {
            return AuthenticationUtilities.VerifyPassword(userName, password);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public void GetUserInfo(out IIdentity userIdentity, out IntPtr userId)
        {
            if (HttpContext.Current.Items["OriginalUrl"].ToString() == "https://localhost/ReportServer/ReportService2010.asmx")
            {
                FormsAuthentication.SetAuthCookie(AuthenticationUtilities.ExtRsUser, true);
                userIdentity = new GenericIdentity("ReportingServicesTools");
            }
            else
            {
                if (HttpContext.Current.Request.IsLocal)
                {
                    FormsAuthentication.SetAuthCookie(AuthenticationUtilities.ExtRsUser, true);
                    userIdentity = HttpContext.Current.User.Identity;
                }
                else
                    userIdentity = new GenericIdentity(@"BUILTIN\Everyone");
            }

            // initialize a pointer to the current user id to zero
            userId = IntPtr.Zero;
        }

        //adding new GetUserInfo method for IAuthenticationExtension2
        public void GetUserInfo(IRSRequestContext requestContext, out IIdentity userIdentity, out IntPtr userId)
        {
            userIdentity = null;
            if (requestContext.User != null && requestContext.User.Name == @"Daylite")
                userIdentity = requestContext.User;

            // initialize a pointer to the current user id to zero
            userId = IntPtr.Zero;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public bool IsValidPrincipalName(string principalName)
        {
            return VerifyUser(principalName);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope")]
        public static bool VerifyUser(string userName)
        {
            return true; //already auth'd
        }
    }
}
