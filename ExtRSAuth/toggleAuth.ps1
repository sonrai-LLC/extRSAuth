# Create function to inspect the config files for existing entries
Function Get-StrPattern{
    Param ($filePath, $string)
    $found = Get-Content $filePath `
                | %{ $res = $false } `
                   { $res = $res -or $_.Contains($string) } `
                   { return $res }

   return $found
}

# Init RS variables
$rsSrvDir = "C:\Program Files\Microsoft SQL Server Reporting Services"
$extRSAuthDir = ".\bin\Debug"
$rsConfigFilePath = ($rsSrvDir + "\SSRS\ReportServer\rsreportserver.config")
[xml]$rsConfigFile = (Get-Content $rsConfigFilePath)
$webConfigFilePath = ($rsSrvDir + "\SSRS\ReportServer\web.config")
[xml]$webConfig = (Get-Content $webConfigFilePath)

Write-Host ":::::::::::::::::::::::::::::::::"
# Check if ExtRSAuth is installed
If(-Not(Test-Path ($rsSrvDir + "\SSRS\ReportServer\bin\Sonrai.ExtRSAuth.dll")))
{
	Write-Host "Please install ExtRSAuth via: .\InitalizeExtRSAuth" -ForegroundColor Magenta
	Write-Host "EXITING..." -ForegroundColor Magenta
	Return
}

# Stop the RS Server
Stop-Service SQLServerReportingServices

If(Get-StrPattern ($rsSrvDir + "\SSRS\ReportServer\web.config")  'sqlAuthCookie')
{
	Write-Host ":::::::TOGGLE to Windows Auth:::::::"
	Write-Host ":::::::::::::::::::::::::::::::::"
	Write-Host "Updating rsreportserver.config `n" -ForegroundColor Cyan
	Write-Host "Copy reportserver.config file in $rsConfigFilePath.backup" -ForegroundColor Cyan
	$rsConfigFile.Save("$rsConfigFilePath.backup")
	$rsConfigFile.Configuration.Authentication.AuthenticationTypes.InnerXml = "<RSWindowsNTLM />"
	$extension = $rsConfigFile.CreateElement("Extension")
	$extension.SetAttribute("Name","Windows")
	$extension.SetAttribute("Type","Microsoft.ReportingServices.Authorization.WindowsAuthorization, Microsoft.ReportingServices.Authorization")
	$configuration =$rsConfigFile.CreateElement("Configuration")
	$configuration.InnerXml="<AdminConfiguration>`n<UserName>Administrator</UserName>`n</AdminConfiguration>"
	$extension.AppendChild($configuration)
	$rsConfigFile.Configuration.Extensions.Security.AppendChild($extension)
	$rsConfigFile.Configuration.Extensions.Security.RemoveChild($rsConfigFile.Configuration.Extensions.Security.FirstChild)
	$rsConfigFile.Save($rsConfigFilePath)
	$rsConfigFile.Configuration.Extensions.Authentication.Extension.Name ="Windows"
	$rsConfigFile.Configuration.Extensions.Authentication.Extension.Type ="Microsoft.ReportingServices.Authentication.WindowsAuthentication, Microsoft.ReportingServices.Authorization"
	$rsConfigFile.Save($rsConfigFilePath)

	Write-Host "Configuring web app Identity Authentication `n" -ForegroundColor Cyan
    Write-Host "Copy web.config file in $webConfigFilePath.backup" -ForegroundColor Cyan
	$webConfig.Save("$webConfigFilePath.backup")
	$webConfig.Configuration.'System.Web'.Identity.Impersonate="true"
	$webConfig.Configuration.'System.Web'.Authentication.Mode="Windows"
	$webConfig.Configuration.'System.Web'.Authentication.InnerXml=""
	$webConfig.Save($webConfigFilePath)
}
Else
{
	Write-Host ":::::::TOGGLE to ExtRSAuth:::::::"
	Write-Host ":::::::::::::::::::::::::::::::::"
	Write-Host "Updating rsreportserver.config `n" -ForegroundColor Cyan
	Write-Host "Copy reportserver.config file in $rsConfigFilePath.backup" -ForegroundColor Cyan
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

	Write-Host "Configuring web app Identity Authentication `n" -ForegroundColor Cyan
	Write-Host "Copy web.config file in $webConfigFilePath.backup" -ForegroundColor Cyan
	$webConfig.Save("$webConfigFilePath.backup")
	$webConfig.Configuration.'System.Web'.Identity.Impersonate="true"
	$webConfig.Configuration.'System.Web'.Authentication.Mode="Forms"
	$webConfig.Configuration.'System.Web'.Authentication.InnerXml="<forms loginUrl=""logon.aspx"" name=""sqlAuthCookie"" timeout=""60"" path=""/""></forms>"
	$webConfig.Save($webConfigFilePath)
}

# Start the RS Server
Start-Service SQLServerReportingServices
