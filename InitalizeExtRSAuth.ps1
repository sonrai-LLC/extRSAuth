Write-Host "Creating the User Store Database"
& sqlcmd -S "." -i "Setup\CreateUserStore.Sql"

Write-Host "Copying Logon.aspx page `n" -ForegroundColor Green
Copy-Item -Path Logon.aspx -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\"

Write-Host "Copying Sonrai.ExtRSAuth.dll `n" -ForegroundColor Green
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\Bin\"
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\Portal\"
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\PowerBi"

Write-Host "Copying Sonrai.ExtRSAuth.onfig `n" -ForegroundColor Green
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\Bin\"
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\Portal\"
Copy-Item -Path Sonrai.ExtRSAuth.dll -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\PowerBi"

Write-Host "Copying Sonrai.ExtRSAuth.pdb `n" -ForegroundColor Green
Copy-Item -Path Sonrai.ExtRSAuth.pdb -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\Bin\"
Copy-Item -Path Sonrai.ExtRSAuth.pdb -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\Portal\"
Copy-Item -Path Sonrai.ExtRSAuth.pdb -Destination "C:\Program Files\Microsoft Power BI Report Server\PBIRS\PowerBi"

Write-Host "Updating rsreportserver.config `n" -ForegroundColor Green
$rsConfigFilePath = "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\rsreportserver.config"
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
Write-Host "Copy of the original config file in $rsConfigFilePath.backup"
$rsConfigFile.Save("$rsConfigFilePath.backup")
$rsConfigFile.Configuration.Authentication.AuthenticationTypes.InnerXml = "<Custom />"

$extension = $rsConfigFile.CreateElement("Extension")
$extension.SetAttribute("Name","Forms")
$extension.SetAttribute("Type"," Sonrai.ExtRSAuth.Authorization,  Sonrai.ExtRSAuth")
$configuration =$rsConfigFile.CreateElement("Configuration")
$configuration.InnerXml="<AdminConfiguration>`n<UserName>AdminUser</UserName>`n</AdminConfiguration>"
$extension.AppendChild($configuration)
$rsConfigFile.Configuration.Extensions.Security.AppendChild($extension)
$rsConfigFile.Configuration.Extensions.Authentication.Extension.Name ="Forms"
$rsConfigFile.Configuration.Extensions.Authentication.Extension.Type ="Sonrai.ExtRSAuth.AuthenticationExtension, Sonrai.ExtRSAuth"

$rsConfigFile.Save($rsConfigFilePath)

Write-Host "Updating RSSrvPolicy.config `n" -ForegroundColor Green
$rsPolicyFilePath = "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\rssrvpolicy.config"
[xml]$rsPolicy = (Get-Content $rsPolicyFilePath)
Write-Host "Copy of the original config file in $rsPolicyFilePath.backup"
$rsPolicy.Save("$rsPolicyFilePath.backup")

$codeGroup = $rsPolicy.CreateElement("CodeGroup")
$codeGroup.SetAttribute("class","UnionCodeGroup")
$codeGroup.SetAttribute("version","1")
$codeGroup.SetAttribute("Name","SecurityExtensionCodeGroup")
$codeGroup.SetAttribute("Description","Code group for the ExtRS security extension")
$codeGroup.SetAttribute("PermissionSetName","FullTrust")
$codeGroup.InnerXml ="<IMembershipCondition class=""UrlMembershipCondition"" version=""1"" Url=""C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\bin\Sonrai.ExtRSAuth.dll""/>"
$rsPolicy.Configuration.mscorlib.security.policy.policylevel.CodeGroup.CodeGroup.AppendChild($codeGroup)
$rsPolicy.Save($rsPolicyFilePath)


Write-Host "Updating web.config `n" -ForegroundColor Green
$webConfigFilePath = "C:\Program Files\Microsoft Power BI Report Server\PBIRS\ReportServer\web.config"
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


Write-Host "Adding Machine Keys to $rsConfigFilePath `n" -ForegroundColor Green
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
$machineKey = $rsConfigFile.CreateElement("MachineKey")
$machineKey.SetAttribute("ValidationKey","6A883FF722BC07963704B124939B9E584673875C744CC2BDA0A076C8668AA8335FC0F5696CFFE5A56FA5E32BC00E0104710BA386FBFF8DCAA3BF3EE1A9B288A5")
$machineKey.SetAttribute("DecryptionKey","AF017F3FFDBDD11B813FCDFBB8D4CEB32FA0CB999954A53E")
$machineKey.SetAttribute("Validation","AES")
$machineKey.SetAttribute("Decryption","AES")
$rsConfigFile.Configuration.AppendChild($machineKey)
$rsConfigFile.Save($rsConfigFilePath)


Write-Host "Configuring Passthrough cookies `n" -ForegroundColor Green
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
$customUI = $rsConfigFile.CreateElement("CustomAuthenticationUI")
$customUI.InnerXml ="<PassThroughCookies><PassThroughCookie>sqlAuthCookie</PassThroughCookie></PassThroughCookies>"
$rsConfigFile.Configuration.UI.AppendChild($customUI)
$rsConfigFile.Save($rsConfigFilePath)









