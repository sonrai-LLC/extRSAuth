#region
// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Modified (c) 2022 cfitzg. All Rights Forfitted.
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
using System.Configuration;

namespace Sonrai.ExtRSAuth
{
   public class Logon : System.Web.UI.Page
   {
        public System.Web.UI.WebControls.Button BtnLogon;

        private void Page_Init(object sender, EventArgs e)
        {
            var isLocalConn = System.Web.HttpContext.Current.Request.IsLocal;
            if (isLocalConn)
            {
                FormsAuthentication.SetAuthCookie(@"BUILTIN\Administrators", true);
                var returnUrl = System.Web.HttpContext.Current.Request.Url.Query;
                Response.Redirect("https://localhost" + (returnUrl.ToLower().EndsWith("reports") ? "/reports" : "/reportserver"));
            }
            else
            {
                try
                {
                    var decryptUri = Encryption.Decrypt(ExtractEncQs(System.Web.HttpContext.Current.Request.Url.PathAndQuery), ConfigurationManager.AppSettings["Cle"]);
                    if (!decryptUri.Contains("ReportServer?"))
                    {
                        FormsAuthentication.SetAuthCookie(@"\Everyone", false);
                        Response.Redirect(decryptUri);
                    }
                }
                catch (Exception)
                {
                    FormsAuthentication.SignOut();
                }
            }
        }

        public string ExtractEncQs(string uri)
        {
            var tmp = Server.UrlDecode(Server.UrlDecode(uri));
            return tmp.Substring(tmp.IndexOf("Qs=") + 3);
        }
           
        #region Required Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            Init += new EventHandler(this.Page_Init);
        }
        #endregion
   }
}
