# This script configures everything needed for ExtRSAuth to work; run 1x and configure your ExtRSAuth project build to update .
$SQLServer = "."
$db3 = "ReportServer"
$qcd = @' USE [ReportServer]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[SetLastModified]
@Path nvarchar (425),
@ModifiedBySid varbinary (85) = NULL,
@ModifiedByName nvarchar(260) = ''ADMIN'',
@AuthType int,
@ModifiedDate DateTime
AS
DECLARE @ModifiedByID uniqueidentifier
EXEC GetUserID @ModifiedBySid, @ModifiedByName, @AuthType, @ModifiedByID OUTPUT
UPDATE Catalog
SET ModifiedByID = @ModifiedByID, ModifiedDate = @ModifiedDate
WHERE Path = @Path
GO;

ALTER PROCEDURE [dbo].[SetAllProperties]
@Path nvarchar (425),
@EditSessionID varchar(32) = NULL,
@Property ntext,
@Description ntext = NULL,
@Hidden bit = NULL,
@ModifiedBySid varbinary (85) = NULL,
@ModifiedByName nvarchar(260) = ''ADMIN'',
@AuthType int,
@ModifiedDate DateTime
AS

IF(@EditSessionID is null)
BEGIN
DECLARE @ModifiedByID uniqueidentifier
EXEC GetUserID @ModifiedBySid, @ModifiedByName, @AuthType, @ModifiedByID OUTPUT

UPDATE Catalog
SET Property = @Property, Description = @Description, Hidden = @Hidden, ModifiedByID = @ModifiedByID, ModifiedDate = @ModifiedDate
WHERE Path = @Path
END
ELSE
BEGIN
    UPDATE [ReportServerTempDB].dbo.TempCatalog
    SET Property = @Property, Description = @Description
    WHERE ContextPath = @Path and EditSessionID = @EditSessionID
END
GO;
'@
 
Write-Host "ALTER necessary SSRS SPs to work with ExtRSAuth custom authentction" -ForegroundColor Lime
Invoke-Sqlcmd -ServerInstance $SQLServer -Database $db3 -Query $qcd

$isRS = Test-Path -Path "C:\Program Files\Microsoft SQL Server Reporting Services\SSRS\ReportServer"
$rsDir

if($isRS)
{
  $rsDir = "C:\Program Files\Microsoft SQL Server Reporting Services\SSRS"
}
else
{
  $rsDir = "C:\Program Files\Microsoft Power BI Report Server\PBIRS"
}

Write-Host "Copying Logon.aspx page `n" -ForegroundColor Lime
Copy-Item -Path Logon.aspx -Destination $rsDir

Write-Host "Copying Sonrai.ExtRSAuth.dll `n" -ForegroundColor Lime
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination $rsDir + "\ReportServer\Bin"
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination $rsDir + "\Portal"
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination $rsDir + "\PowerBi"

Write-Host "Copying Microsoft.Samples.ReportingServices.CustomSecurity.dll.config `n" -ForegroundColor Lime
Copy-Item -Path Sonrai.ExtRSAuth.dll.config -Destination $rsDir + "\ReportServer\Bin"
Copy-Item -Path Sonrai.ExtRSAuth.dll.config -Destination $rsDir + "\Portal"
Copy-Item -Path Sonrai.ExtRSAuth.dll.config -Destination $rsDir + "\PowerBi"

Write-Host "Copying Microsoft.Samples.ReportingServices.CustomSecurity.pdb `n" -ForegroundColor Lime
Copy-Item -Path Sonrai.ExtRSAuth.pdb -Destination $rsDir + "\ReportServer\Bin\"
Copy-Item -Path Sonrai.ExtRSAuth.pdb -Destination $rsDir + "\Portal\"
Copy-Item -Path Sonrai.ExtRSAuth.pdb -Destination $rsDir + "\PowerBi"

Write-Host "Updating rsreportserver.config `n" -ForegroundColor Lime
$rsConfigFilePath = $rsDir + "\ReportServer\rsreportserver.config"
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
Write-Host "Copy of the original config file in $rsConfigFilePath.backup"
$rsConfigFile.Save("$rsConfigFilePath.backup")
$rsConfigFile.Configuration.Authentication.AuthenticationTypes.InnerXml = "<Custom />"

$extension = $rsConfigFile.CreateElement("Extension")
$extension.SetAttribute("Name","Forms")
$extension.SetAttribute("Type","Sonrai.ExtRSAuth.Authorization, Sonrai.ExtRSAuth")
$configuration =$rsConfigFile.CreateElement("Configuration")
$configuration.InnerXml="<AdminConfiguration>`n<UserName>username</UserName>`n</AdminConfiguration>"
$extension.AppendChild($configuration)
$rsConfigFile.Configuration.Extensions.Security.AppendChild($extension)
$rsConfigFile.Configuration.Extensions.Authentication.Extension.Name ="Forms"
$rsConfigFile.Configuration.Extensions.Authentication.Extension.Type ="Sonrai.ExtRSAuth.AuthenticationExtension, Sonrai.ExtRSAuth"

$rsConfigFile.Save($rsConfigFilePath)

Write-Host "Updating RSSrvPolicy.config `n" -ForegroundColor Lime
$rsPolicyFilePath = $rsDir + "\ReportServer\rssrvpolicy.config"
[xml]$rsPolicy = (Get-Content $rsPolicyFilePath)
Write-Host "Copy of the original config file in $rsPolicyFilePath.backup"
$rsPolicy.Save("$rsPolicyFilePath.backup")

$codeGroup = $rsPolicy.CreateElement("CodeGroup")
$codeGroup.SetAttribute("class","UnionCodeGroup")
$codeGroup.SetAttribute("version","1")
$codeGroup.SetAttribute("Name","SecurityExtensionCodeGroup")
$codeGroup.SetAttribute("Description","Code group for the sample security extension")
$codeGroup.SetAttribute("PermissionSetName","FullTrust")
$codeGroup.InnerXml ="<IMembershipCondition class=""UrlMembershipCondition"" version=""1"" Url=""" + $rsDir + "\ReportServer\bin\Sonrai.ExtRSAuth.dll""/>"
$rsPolicy.Configuration.mscorlib.security.policy.policylevel.CodeGroup.CodeGroup.AppendChild($codeGroup)
$rsPolicy.Save($rsPolicyFilePath)


Write-Host "Updating web.config `n" -ForegroundColor Lime
$webConfigFilePath = $rsDir + "\ReportServer\web.config"
[xml]$webConfig = (Get-Content $webConfigFilePath)
Write-Host "Copy of the original config file in $webConfigFilePath.backup"
$webConfig.Save("$webConfigFilePath.backup")
$webConfig.configuration.'system.web'.identity.impersonate="false"
$webConfig.configuration.'system.web'.authentication.mode="Forms"
$webConfig.configuration.'system.web'.authentication.InnerXml="<forms loginUrl=""logon.aspx"" name=""sqlAuthCookie"" timeout=""60"" path=""/""></forms>"
$authorization = $webConfig.CreateElement("authorization")
$authorization.InnerXml="<deny users=""?"" />"
$webConfig.configuration.'system.web'.AppendChild($authorization)
$webConfig.Save($webConfigFilePath)


Write-Host "Adding Machine Keys to $rsConfigFilePath `n" -ForegroundColor Lime
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
$machineKey = $rsConfigFile.CreateElement("MachineKey")
$machineKey.SetAttribute("ValidationKey","6A883FF722BC07123704B124939B9E584673875C744CC2BDA0A076CDD68AA8335FC0F5696CFFE5A56FA5E32BC00E010471RFA386FBFF8DCAA3BF3EE1A9B288A5")
$machineKey.SetAttribute("DecryptionKey","AF017F3FF5GDD11B813FC12BB8D4CEB32FA0CB999954A11V")
$machineKey.SetAttribute("Validation","AES")
$machineKey.SetAttribute("Decryption","AES")
$rsConfigFile.Configuration.AppendChild($machineKey)
$rsConfigFile.Save($rsConfigFilePath)


Write-Host "Configuring Passthrough cookies `n" -ForegroundColor Lime
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
$customUI = $rsConfigFile.CreateElement("CustomAuthenticationUI")
$customUI.InnerXml ="<PassThroughCookies><PassThroughCookie>sqlAuthCookie</PassThroughCookie></PassThroughCookies>"
$rsConfigFile.Configuration.UI.AppendChild($customUI)
$rsConfigFile.Save($rsConfigFilePath)

Write-Host "Configuration of ExtRSAuth complete! Press any key to continue...`n" -ForegroundColor Lime