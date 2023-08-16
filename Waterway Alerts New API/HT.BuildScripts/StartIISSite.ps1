param(
	[string]$SiteName, 
	[string]$AppPoolName,
	[string]$HealthCheckEndpoint)
	
if(-not($SiteName)) { Throw "You must supply a value for -SiteName" }
if(-not($AppPoolName)) { Throw "You must supply a value for -AppPoolName" }

cd $env:windir\System32\inetsrv;
.\appcmd start site $SiteName;
.\appcmd start apppool $AppPoolName