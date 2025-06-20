# extRSAuth for custom SSRS security
This assembly, forked from the [Microsoft Custom Security Sample](https://github.com/Microsoft/Reporting-Services/tree/master/CustomSecuritySample) extends and improves custom authentication to allow for mechanisms other than user/pwd credential check and to offer a seamless pass-thru of the Login page if something present in the HttpRequest (ie. an encrypted queryString value) can verify that a user a user is allowed to be authenticated in RS due some other prior authentication. For instance, the user already has an app token from an app that communicates with the report server and you require the communications with the report server to not involve any intermediate screen or login UI. The user just wants to auth as fast as possible and get to their report, right?

A .ps1 script has been added to the project (ToggleAuth.ps1, which toggles RS Authentication between Windows Auth and Custom Forms (extRSAuth implementation) via: .\ToggleAuth), which can be executed after installation of extRSAuth via InitalizeExtRSAuth.ps1. Adjust the ToggleAuth.ps1 script as needed if any paths or other settings do not match your SSRS installation. See the short screen recording demonstrating this feature [here](https://www.youtube.com/watch?v=5L1wRfP8A-k).

# What does extRSAuth do to authenticate SSRS user connections?
The default here is to allow local connections, which grants Admin rights for any local requests. If the SSRS request is external, a fallback option accepts an AES 256-bit encrypted querystring from the calling app, and the application, if decryption works, is authenticated and allowed to communicate using a read-only SSRS user; any exception thrown indicates the request is neither from a local connection nor a secure request from the external app.

This is but one of many approaches we can take with extRSAuth in an SSRS-connected application or business environment. Any type and granularity of custom authentication and level of authorization is possible. The only ingredient needed is a .NET developer or developers willing to customize a pretty basic .NET security model.

# Real-world applications
This Custom Auth assembly has been tested with (1) several .NET Framework 4.8 and .NET 5, 6, 7, 8 and 9 applications, (2) with the SSRS API and all its operations, (3) with the SSRS /ReportServer and the /Reports management web interface as well as (4) Visual Studio 2022 Reporting Services projects (report designers can seamlessly deploy Report Server projects from VS to the Report Server with extRSAuth).

# Demonstration
This [YouTube walkthrough](https://www.youtube.com/watch?v=8E-ESx3kSXc) provides a step-by-step guide to implementation of extRSAuth. This [YouTube explainer video](https://www.youtube.com/watch?v=B49b_y42vNA) describes the SSRS external user authentication problem that extRSAuth addresses. Or for the TL;DR; there is [this short screen capture](https://www.youtube.com/watch?v=0NmlrADXvZo) demonstrating how extRSAuth works.

# Requirements
This plug-in has been tested to work with SSRS 2022 v16.0.9276.19198 running on SQL Server 2019, SQL Server 2022 and SQL Server 2025.

-In the Settings resource, replace the default values for "cle" and "passphrase" with your symetric encryption key and desired pass phrase, respectively. Clients can encrypt SSRS URL access querystring with `Sonrai.ExtRSAuth.Encryption.Encrypt()` or a similiar 256-bit AES encryption implementation, or modify Encrypt() with another encryption algorithm implementation.

This package includes the following components:
- ExtRSAuth.dll

# Related SSRS Tools
- [extRS](https://github.com/sonrai-LLC/extRS) for extending the capabilities of SSRS
