# Migrate db to the latest version

. .\Config.ps1

& $toolPath --verbose=true -a ../../bin/Debug/Worker.Common.dll -db sqlserver -conn $connString