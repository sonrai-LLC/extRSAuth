# ExtRSAuth for custom SSRS security
This assembly, forked from the [Microsoft Custom Security Sample](https://github.com/Microsoft/Reporting-Services/tree/master/CustomSecuritySample) extends and improves custom authentication to allow for mechanisms other than user/pwd credential check and to offer a seamless pass-thru of the Login page if something present in the HttpRequest verifies that user is already authenticated. For instance, the user already has an app token from an app that communicates with the report server and you require the communications with the report server to not involve any intermediate screen or login UI. The user just wants to auth as fast as possible and get to their report, right?

# What does ExtRSAuth do to authenticate SSRS user connections?
The default here is to allow local connections, which grants Admin rights for any local requests. If the SSRS request is external, a fallback option accepts an AES 128-bit encrypted querystring from the calling app, and the application, if decryption works, is authenticated and allowed to communicate using a read-only SSRS user; any exception thrown indicates the request is neither from a local connection nor a secure request from the external app.

This is but one of many approaches we can take with ExtRSAuth in an SSRS-connected application or business environment. Any type and granularity of custom authentication and level of authorization is possible. The only ingredient needed is a .NET developer or developers willing to customize a pretty basic .NET security model.

# Real-world applications
This Custom Auth assembly has been tested with (1) several .NET Framework 4.8 and .NET 5, 6 and 7 applications, (2) with the SSRS API and all its operations, (3) with the SSRS /ReportServer and the /Reports management web interface as well as (4) Visual Studio 2022 Reporting Services projects (report designers can seamlessly deploy Report Server projects from VS to the Report Server with ExtRSAuth).

# Demonstration
This [YouTube explainer video](https://www.youtube.com/watch?v=B49b_y42vNA) describes the SSRS external user authentication problem that ExtRSAuth addresses. Or for the TL;DR; there is [this short screen capture](https://www.youtube.com/watch?v=0NmlrADXvZo) demonstrating how ExtRSAuth works.

# Requirements
This plug-in relies on SSRS (2016 or later), and a report server configuration as described in [Microsoft's Reporting Services Custom Security Sample](https://github.com/Microsoft/Reporting-Services/tree/master/CustomSecuritySample)

-Replace **[your_sym_encr_key]** with your symetric encryption key. Clients can encrypt SSRS URL access querystring with `Sonrai.ExtRSAuth.Excryption.Encrypt()` or a similiar 128-bit AES encryption implementation, or modify Encrypt() with any encryption algorithm and key and block sizes.

This package includes the following components:
- ExtRSAuth.dll

# Related SSRS Tools
- [ExtRS](https://github.com/sonrai-LLC/ExtRS) for extending the capabilities of SSRS: deploying, rendering, managing and monitoring
