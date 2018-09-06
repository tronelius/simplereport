# Rollback the latest version. Can be called repeatedly to rollback several versions.

. .\Config.ps1

& $toolPath --verbose=true -a ../../bin/Debug/Worker.Common.dll -db sqlserver -conn $connString -t=rollback