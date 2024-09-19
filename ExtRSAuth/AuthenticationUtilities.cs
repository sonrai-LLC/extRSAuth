#region
// Copyright (c) 2016 Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License (MIT)
/*============================================================================
   File:      AuthenticationStore.cs

  Summary:  Demonstrates how to create and maintain a user store for
            a security extension. 
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

using System.Data.SqlClient;
using System.Web;

namespace Sonrai.ExtRSAuth
{
    public class AuthenticationUtilities
    {
        public const string ExtRsUser = "extRSAuth";
		public const string ExtRsReadOnlyUser = "extRS.Portal";
        public const string ReadOnlyUser = "BUILTIN\\Everyone";
        public const string MSBIToolsUser = "ReportingServicesTools";
        public const string ReportExecution2005SOAP = "https://localhost/reportserver/ReportExecution2005.asmx";
        public const string ReportService2010SOAP = "https://localhost/ReportServer/ReportService2010.asmx";

        // API auth uses this method
        public static bool VerifyPassword(string password)
        {
            if (password == Properties.Settings.Default.passphrase)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string ExtractEncQs(string uri)
        {
            var tmp = HttpUtility.UrlDecode(HttpUtility.UrlDecode(uri));
            return tmp.Substring(tmp.IndexOf("Qs=") + 3);
        }

        public static string ExtractRSUserName(string uri)
        {
            var tmp = HttpUtility.UrlDecode(HttpUtility.UrlDecode(uri));
            return tmp.Substring(tmp.IndexOf("UserName=") + 9);
        }

        public static bool RSUserExists(string userName)
        {
            SqlConnection sqlConnection = new SqlConnection("Data Source=.;Initial Catalog=ReportServer;Integrated Security=True");
            sqlConnection.Open();
            SqlCommand sqlCommand = sqlConnection.CreateCommand();
            sqlCommand.CommandText = string.Format("SELECT COUNT(*) FROM [ReportServer].[dbo].[Users] WHERE UserName = '{0}'", userName);
            int userCount = (int)sqlCommand.ExecuteScalar();

            return userCount > 0;
        }
    }
}
