# Configuration for the migration scripts
$scriptPath = split-path -parent $MyInvocation.MyCommand.Definition
$toolPath = "$scriptPath\..\..\..\packages\FluentMigrator.1.6.2\tools\Migrate.exe"
$connString = "Server=(LocalDb)\MSSQLLocalDB;Database=SimpleReport;Trusted_Connection=True;multipleactiveresultsets=True;"
