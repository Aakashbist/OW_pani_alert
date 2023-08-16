param([string]$SiteName, [string]$AppPoolName)
if(-not($SiteName)) { Throw "You must supply a value for -SiteName" }
if(-not($AppPoolName)) { Throw "You must supply a value for -AppPoolName" }

cd $env:windir\System32\inetsrv;
.\appcmd stop site $SiteName;
.\appcmd stop apppool $AppPoolName;