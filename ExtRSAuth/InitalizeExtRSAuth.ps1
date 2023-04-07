# Create function to inspect the config files for existing entries
Function Get-StrPattern{
    Param ($filePath, $string)
    $found = Get-Content $filePath `
                | %{ $res = $false } `
                   { $res = $res -or $_.Contains($string) } `
                   { return $res }

   return $found
}

# This script configures everything needed for ExtRSAuth to work
$SQLServer = "."
$db = "ReportServer"
$sql1 = @'
ALTER PROCEDURE [dbo].[SetLastModified]
@Path nvarchar (425),
@ModifiedBySid varbinary (85) = NULL,
@ModifiedByName nvarchar(260) = 'BUILTIN/\Administrators',
@AuthType int,
@ModifiedDate DateTime
AS
DECLARE @ModifiedByID uniqueidentifier
EXEC GetUserID @ModifiedBySid, @ModifiedByName, @AuthType, @ModifiedByID OUTPUT
UPDATE Catalog
SET ModifiedByID = @ModifiedByID, ModifiedDate = @ModifiedDate
WHERE Path = @Path
'@

$sql2 = @'
ALTER PROCEDURE [dbo].[SetAllProperties]
@Path nvarchar (425),
@EditSessionID varchar(32) = NULL,
@Property ntext,
@Description ntext = NULL,
@Hidden bit = NULL,
@ModifiedBySid varbinary (85) = NULL,
@ModifiedByName nvarchar(260) = 'ExtRSAuth',
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
'@

$sql3 = @'
ALTER PROCEDURE [dbo].[CreateObject]
@ItemID uniqueidentifier,
@Name nvarchar (425),
@Path nvarchar (425),
@ParentID uniqueidentifier,
@Type int,
@Content image = NULL,
@Intermediate uniqueidentifier = NULL,
@LinkSourceID uniqueidentifier = NULL,
@Property ntext = NULL,
@Parameter ntext = NULL,
@Description ntext = NULL,
@Hidden bit = NULL,
@CreatedBySid varbinary(85) = NULL,
@CreatedByName nvarchar(260) = 'ExtRSAuth',
@AuthType int,
@CreationDate datetime,
@ModificationDate datetime,
@MimeType nvarchar (260) = NULL,
@SnapshotLimit int = NULL,
@PolicyRoot int = 0,
@PolicyID uniqueidentifier = NULL,
@ExecutionFlag int = 1, -- allow live execution, don't keep history
@SubType nvarchar(128) = NULL,
@ComponentID uniqueidentifier = NULL
AS

DECLARE @CreatedByID uniqueidentifier
EXEC GetUserID @CreatedBySid, @CreatedByName, @AuthType, @CreatedByID OUTPUT

UPDATE Catalog
SET ModifiedByID = @CreatedByID, ModifiedDate = @ModificationDate
WHERE ItemID = @ParentID

-- If no policyID, use the parent's
IF @PolicyID is NULL BEGIN
   SET @PolicyID = (SELECT PolicyID FROM [dbo].[Catalog] WHERE Catalog.ItemID = @ParentID)
END

-- If there is no policy ID then we are guarenteed not to have a parent
IF @PolicyID is NULL BEGIN
RAISERROR ('Parent Not Found', 16, 1)
return
END

INSERT INTO Catalog (ItemID,  Path,  Name,  ParentID,  Type,  Content, ContentSize,  Intermediate,  LinkSourceID,  Property,  Description,  Hidden,  CreatedByID,  CreationDate,  ModifiedByID,  ModifiedDate,  MimeType,  SnapshotLimit,  [Parameter],  PolicyID,  PolicyRoot, ExecutionFlag, SubType, ComponentID)
VALUES             (@ItemID, @Path, @Name, @ParentID, @Type, @Content, DATALENGTH(@Content), @Intermediate, @LinkSourceID, @Property, @Description, @Hidden, @CreatedByID, @CreationDate, @CreatedByID,  @ModificationDate, @MimeType, @SnapshotLimit, @Parameter, @PolicyID, @PolicyRoot , @ExecutionFlag, @SubType, @ComponentID)

IF @Intermediate IS NOT NULL AND @@ERROR = 0 BEGIN
   UPDATE SnapshotData
   SET PermanentRefcount = PermanentRefcount + 1, TransientRefcount = TransientRefcount - 1
   WHERE SnapshotData.SnapshotDataID = @Intermediate
END
'@

$sql4 = @'
IF NOT EXISTS(SELECT * FROM Users WHERE UserName = 'ExtRSAuth')
          BEGIN
	          INSERT INTO Users (UserID, UserName, UserType, AuthType)
	          VALUES(newid(), 'ExtRSAuth', 0, 3)
          END
'@

$rsSrvDir = "C:\Program Files\Microsoft SQL Server Reporting Services"
$extRSAuthDir = ".\bin\Debug"

Write-Host "ALTER necessary SSRS SPs to work with ExtRSAuth custom authentication `n" -ForegroundColor Cyan
Invoke-Sqlcmd -ServerInstance $SQLServer -Database $db -Query $sql1
Invoke-Sqlcmd -ServerInstance $SQLServer -Database $db -Query $sql2
Invoke-Sqlcmd -ServerInstance $SQLServer -Database $db -Query $sql3
Invoke-Sqlcmd -ServerInstance $SQLServer -Database $db -Query $sql4

    If(-Not(Test-Path ($rsSrvDir + "\SSRS.ORIGINAL\ReportServer")))
    {
        Write-Host "Copy backup of original SSRS config files `n" -ForegroundColor Cyan
        Copy-Item -Path ($rsSrvDir + "\SSRS") -Destination ($rsSrvDir + "\SSRS.ORIGINAL") -Recurse 
    }

    If(-Not(Test-Path ($rsSrvDir + "\SSRS\ReportServer\Logon.aspx")))
    {
        Write-Host "Copying Logon.aspx page `n" -ForegroundColor Cyan
        Copy-Item -Path ($extRSAuthDir + "\Logon.aspx") -Destination ($rsSrvDir + "\SSRS\ReportServer")
    }

    If(-Not(Test-Path ($rsSrvDir + "\SSRS\ReportServer\bin\Sonrai.ExtRSAuth.dll")))
    {
        Write-Host "Copying Sonrai.ExtRSAuth.dll `n" -ForegroundColor Cyan
        Copy-Item -Path ($extRSAuthDir + "\Sonrai.ExtRSAuth.dll") -Destination ($rsSrvDir + "\SSRS\ReportServer\bin")
    }

    If(-Not(Test-Path ($rsSrvDir + "\SSRS\ReportServer\bin\Sonrai.ExtRSAuth.pdb")))
    {
        Write-Host "Copying Sonrai.ExtRSAuth.pdb `n" -ForegroundColor Cyan
        Copy-Item -Path ($extRSAuthDir + "\Sonrai.ExtRSAuth.pdb") -Destination ($rsSrvDir + "\SSRS\ReportServer\bin")
    }

    If(-Not(Test-Path ($rsSrvDir + "\SSRS\Portal\Sonrai.ExtRSAuth.dll")))
    {
        Write-Host "Copying Sonrai.ExtRSAuth.dll `n" -ForegroundColor Cyan
        Copy-Item -Path ($extRSAuthDir + "\Sonrai.ExtRSAuth.dll") -Destination ($rsSrvDir + "\SSRS\Portal")
    }

    If(-Not(Test-Path ($rsSrvDir + "\SSRS\Portal\Sonrai.ExtRSAuth.pdb")))
    {
        Write-Host "Copying Sonrai.ExtRSAuth.pdb `n" -ForegroundColor Cyan
        Copy-Item -Path ($extRSAuthDir + "\Sonrai.ExtRSAuth.pdb") -Destination ($rsSrvDir + "\SSRS\Portal")
    }

    If(-Not(Get-StrPattern ($rsSrvDir + "\SSRS\ReportServer\rsreportserver.config") 'Sonrai.ExtRSAuth.Authorization'))
    {
        Write-Host "Updating rsreportserver.config `n" -ForegroundColor Cyan
        $rsConfigFilePath = ($rsSrvDir + "\SSRS\ReportServer\rsreportserver.config")
        [xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
        Write-Host "Copy of the original config file in $rsConfigFilePath.backup" -ForegroundColor Cyan
        $rsConfigFile.Save("$rsConfigFilePath.backup")
        $rsConfigFile.Configuration.Authentication.AuthenticationTypes.InnerXml = "<Custom />"
        $extension = $rsConfigFile.CreateElement("Extension")
        $extension.SetAttribute("Name","Forms")
        $extension.SetAttribute("Type","Sonrai.ExtRSAuth.Authorization, Sonrai.ExtRSAuth")
        $configuration =$rsConfigFile.CreateElement("Configuration")
        $configuration.InnerXml="<AdminConfiguration>`n<UserName>ExtRSAuth</UserName>`n</AdminConfiguration>"
        $extension.AppendChild($configuration)
        $rsConfigFile.Configuration.Extensions.Security.AppendChild($extension)
        $rsConfigFile.Configuration.Extensions.Security.RemoveChild($rsConfigFile.Configuration.Extensions.Security.FirstChild)
        $rsConfigFile.Save($rsConfigFilePath)
        $rsConfigFile.Configuration.Extensions.Authentication.Extension.Name ="Forms"
        $rsConfigFile.Configuration.Extensions.Authentication.Extension.Type ="Sonrai.ExtRSAuth.AuthenticationExtension, Sonrai.ExtRSAuth"
        $rsConfigFile.Save($rsConfigFilePath)
    }

    If(-Not(Get-StrPattern ($rsSrvDir + "\SSRS\ReportServer\rssrvpolicy.config") '\SSRS\ReportServer\bin\Sonrai.ExtRSAuth.dll'))
    {
            Write-Host "Updating RSSrvPolicy.config `n" -ForegroundColor Cyan
            $rsPolicyFilePath = ($rsSrvDir + "\SSRS\ReportServer\rssrvpolicy.config")
            [xml]$rsPolicy = (Get-Content $rsPolicyFilePath)  
            $codeGroup = $rsPolicy.CreateElement("CodeGroup")
            $codeGroup.SetAttribute("class","UnionCodeGroup")
            $codeGroup.SetAttribute("version","1")
            $codeGroup.SetAttribute("Name","SecurityExtensionCodeGroup")
            $codeGroup.SetAttribute("Description","Code group for ExtRSAuth SSRS Security Extension")
            $codeGroup.SetAttribute("PermissionSetName","FullTrust")
            $codeGroup.InnerXml ="<IMembershipCondition class=""UrlMembershipCondition"" version=""1"" Url=""" + ($rsSrvDir + "\SSRS\ReportServer\bin\Sonrai.ExtRSAuth.dll") + """/>"
            $rsPolicy.Configuration.mscorlib.security.policy.policylevel.CodeGroup.CodeGroup.AppendChild($codeGroup)
            $rsPolicy.Save($rsPolicyFilePath)     
    }

    If(-Not(Get-StrPattern ($rsSrvDir + "\SSRS\ReportServer\web.config")  'sqlAuthCookie'))
    {
        Write-Host "Updating web.config and adding machine keys `n" -ForegroundColor Cyan
        $webConfigFilePath = ($rsSrvDir + "\SSRS\ReportServer\web.config")
        [xml]$webConfig = (Get-Content $webConfigFilePath)
        $webConfig.Configuration.'System.Web'.Identity.Impersonate="false"
        $webConfig.Configuration.'System.Web'.Authentication.Mode="Forms"
        $webConfig.Configuration.'System.Web'.Authentication.InnerXml="<forms loginUrl=""logon.aspx"" name=""sqlAuthCookie"" timeout=""60"" path=""/""></forms>"
        $authorization = $webConfig.CreateElement("authorization")
        $authorization.InnerXml="<deny users=""?"" />"
        $webConfig.Configuration.'System.Web'.AppendChild($authorization)
        $location = $webConfig.CreateElement("location")
        $location.SetAttribute("path", "ReportService2010.asmx")
        $location.InnerXml="<system.web><authorization><allow users=""?"" /></authorization></system.web>"
        $webConfig.Configuration.AppendChild($location)
        $machineKey = $webConfig.CreateElement("machineKey")
        $machineKey.SetAttribute("validationKey","6A883FF722BC07123704B124939B9E584673875C744CC2BDA0A076CDD68AA8335FC0F5696CFFE5A56FA5E32BC00E010471RFA386FBFF8DCAA3BF3EE1A9B288A5")
        $machineKey.SetAttribute("decryptionKey","AF017F3FF5GDD11B813FC12BB8D4CEB32FA0CB999954A11V")
        $machineKey.SetAttribute("validation","HMACSHA256")
        $machineKey.SetAttribute("decryption","AES")
        $webConfig.Configuration.'System.Web'.AppendChild($machineKey)
        $webConfig.Save($webConfigFilePath)

        #rsConfigFile requires MachineKey to be proper-case, no idea why (different product teams?)...
        Write-Host "Adding machine keys to $rsConfigFilePath `n" -ForegroundColor Cyan
        [xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
        $machineKey = $rsConfigFile.CreateElement("MachineKey")
        $machineKey.SetAttribute("ValidationKey","6A883FF722BC07123704B124939B9E584673875C744CC2BDA0A076CDD68AA8335FC0F5696CFFE5A56FA5E32BC00E010471RFA386FBFF8DCAA3BF3EE1A9B288A5")
        $machineKey.SetAttribute("DecryptionKey","AF017F3FF5GDD11B813FC12BB8D4CEB32FA0CB999954A11V")
        $machineKey.SetAttribute("Validation","HMACSHA256")
        $machineKey.SetAttribute("Decryption","AES")
        $rsConfigFile.Configuration.AppendChild($machineKey)
        $rsConfigFile.Save($rsConfigFilePath)

        Write-Host "Configuring Passthrough cookies `n" -ForegroundColor Cyan
        [xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
        $customUI = $rsConfigFile.CreateElement("CustomAuthenticationUI")
        $customUI.InnerXml ="<PassThroughCookies><PassThroughCookie>sqlAuthCookie</PassThroughCookie></PassThroughCookies>"
        $rsConfigFile.Configuration.UI.AppendChild($customUI)
        $rsConfigFile.Save($rsConfigFilePath)
    }
    
    Write-Host "Configuration of ExtRSAuth complete! Happy ExtRSing... :) `n" -ForegroundColor Green
        break;
