# extRSAuth for custom SSRS security
This assembly, forked from the [Microsoft Custom Security Sample](https://github.com/Microsoft/Reporting-Services/tree/master/CustomSecuritySample) extends and improves custom authentication to allow for mechanisms other than user/pwd credential check and to offer seamless pass-thru authentication.

A .ps1 script has been added to the project which can be executed after installation of extRSAuth via InitalizeExtRSAuth.ps1. Adjust the ToggleAuth.ps1 script as needed if any paths or other settings do not match your SSRS installation. See the short screen recording demonstrating this feature [here](https://www.youtube.com/watch?v=5L1wRfP8A-k).

# What does extRSAuth do to authenticate SSRS user connections?
The default here is to allow local connections, which grants Admin rights for any local requests. If the SSRS request is external, a fallback option accepts an AES 256-bit encrypted querystring from the calling app with user credentials, and the application, if decryption works, is authenticated as the requested user. If decryption fails, then the user is not authenticated.

# Real-world applications
This Custom Auth assembly has been tested with (1) several .NET Framework 4.8 and .NET 5-10 applications, (2) with the SSRS API and all its operations, (3) with the SSRS /ReportServer and the /Reports management web interface, (4) with Visual Studio 2022 Reporting Services projects, (5) with Report Builder and (6) with PBIRS.

# Demonstration
This [walkthrough](https://www.youtube.com/watch?v=8E-ESx3kSXc) provides a step-by-step guide to implementation of extRSAuth. Or for the TL;DR; there is [this short screen capture](https://www.youtube.com/watch?v=0NmlrADXvZo) demonstrating how extRSAuth works.

# Requirements
This plug-in has been tested to work with Reporting Services 2022 v16.0 and Power BI Reporting Services v1.23 running on SQL Server 2022 and SQL Server 2025.

This package includes the following components:
- ExtRSAuth.dll

# Related SSRS Tools
- [extRS](https://github.com/sonrai-LLC/extRS) for extending the capabilities of SSRS
