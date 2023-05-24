#region
// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Modified (c) 2022 sonrai LLC. All Rights Forfeited.
// Licensed under the MIT License (MIT)
/*============================================================================
  File:     Logon.aspx.cs
  Summary:  The code-behind for a logon page that supports Forms
            Authentication in a custom security extension    
--------------------------------------------------------------------
  This file is based on Microsoft SQL Server Code Samples.
    
 This source code is intended only as a supplement to Microsoft
 Development Tools and/or on-line documentation. See these other
 materials for detailed information regarding Microsoft code 
 samples.

 THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF 
 ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
 THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
 PARTICULAR PURPOSE.

(BUT YOU CAN DO SOME PRETTY COOL STUFF WITH SSRS CUSTOM AUTH)
===========================================================================*/
#endregion

using System;
using System.Web.Security;

namespace Sonrai.ExtRSAuth
{
   public class Logon : System.Web.UI.Page
   {
        public System.Web.UI.WebControls.Button BtnLogon;

        private void Page_Init(object sender, EventArgs e)
        {
            try        
            {
                var isLocalConn = System.Web.HttpContext.Current.Request.IsLocal;
                if (isLocalConn)
                {
                    FormsAuthentication.RedirectFromLoginPage(AuthenticationUtilities.ExtRsUser, true);
                }
                else
                {
                    var decryptUri = Encryption.Decrypt(ExtractEncQs(System.Web.HttpContext.Current.Request.Url.PathAndQuery), Properties.Settings.Default.cle);
                    FormsAuthentication.RedirectFromLoginPage(AuthenticationUtilities.ReadOnlyUser, true);
                }
            }
            catch (Exception ex)
            {
                FormsAuthentication.SignOut();
            }
        }
        
        //TODO: Add #i18n #l10n
        public string ExtractEncQs(string uri)
        {
            var tmp = Server.UrlDecode(Server.UrlDecode(uri));
            return tmp.Substring(tmp.IndexOf("Qs=") + 3);
        }

        // The below 2 methods are required .NET Framework web form designer code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            Init += new EventHandler(this.Page_Init);
        }
    }
}
