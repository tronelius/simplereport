# Configuration for the migration scripts
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$toolPath = "$scriptPath\..\..\..\packages\FluentMigrator.1.5.1.0\tools\Migrate.exe"
$connString = "Server=.;Database=SimpleReport;Trusted_Connection=True;multipleactiveresultsets=True;"
