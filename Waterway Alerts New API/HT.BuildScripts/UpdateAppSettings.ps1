Param(
	[string]$applicationSettingFile,
	[string]$connectionString,
	[string]$portalURL,
	[string]$env
)

$Utf8NoBomEncoding = New-Object System.Text.UTF8Encoding $False

Write-Output "Updating Application Settings file $applicationSettingFile"
$applicationSettingsFileContents = (Get-Content $applicationSettingFile) 
$applicationSettingsFileContents = $applicationSettingsFileContents -replace '"DefaultConnection": ".*"', "`"DefaultConnection`": `"$connectionString`""
$applicationSettingsFileContents = $applicationSettingsFileContents -replace '"PortalURL": ".*"', "`"PortalURL`": `"$portalURL`""
$applicationSettingsFileContents = $applicationSettingsFileContents -replace '"Environment": ".*"', "`"Environment`": `"$env`""

[System.IO.File]::WriteAllLines($applicationSettingFile, $applicationSettingsFileContents, $Utf8NoBomEncoding)