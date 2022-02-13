# ExtRSAuth for custom SSRS Security
This assembly extends the Microsoft Custom Security Sample to allow for a pass-thru of the Login page if something present in the HttpRequest verifies that user is already authenticated. For instance, the user already has an app token from an app that communicates with the report server and you require the communications with the report server to not involve any intermediate screen or login UI. You just want to auth and get to the report, right?

The default here is to allow local connections, which grants Admin rights for any local requests. If the SSRS request is external, a fallback option accepts an AES 128-bit encrypted querystring from the calling app, and the application, if decryption works, is authenticated and allowed to communicate using a read-only SSRS user; any exception thrown indicates the request neither local nor a secure request from the external app.

This is only how the code curently works to demonstrate one of many approaches. Any type of custom authentication and level of authorization is possible.

# Requirements
This plug-in relies on SSRS (2016 or later), and a report server configuration as described in Microsoft's Reporting Services Custom Security Sample: https://github.com/Microsoft/Reporting-Services/tree/master/CustomSecuritySample

-Replace **[your_sym_encr_key]** with your symetric encryption key. Clients can encrypt SSRS URL access querystring with `Sonrai.ExtRSAuth.Excryption.Encrypt()` or a similiar 128-bit AES encryption implementation, or modify Encrypt() with any encryption algorithm and size that meets your requirements.

This package includes the following components:
- ExtRSAuth.dll

# Related SSRS Tools
- [ExtRS](https://github.com/sonrai-LLC/ExtRS) for extending the capabilities of the SSRS including report rendering, management tools and realtime monitoring.
