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

namespace Sonrai.ExtRSAuth
{
    public class AuthenticationUtilities
    {
        public const string ExtRsUser = "ExtRSAuth";
        public const string ReadOnlyUser = @"BUILTIN\Everyone";
        public const string AdminUser = @"BUILTIN\Administrator";
        public const string MSBIToolsUser = "ReportingServicesTools";
        public const string ReportExecution2005SOAP = "https://localhost/reportserver/ReportExecution2005.asmx";
        public const string ReportService2010SOAP = "https://localhost/ReportServer/ReportService2010.asmx";
        
        public static bool VerifyPassword(string username, string password)
        {
            return true; //already auth'd
        }
    }
}
