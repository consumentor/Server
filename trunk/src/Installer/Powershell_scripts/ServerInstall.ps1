#Install script

#Get current folder
$currentFolder = [System.IO.Path]::GetDirectoryName($myinvocation.mycommand.path)

IF (Test-Path $currentFolder\DontInstall.txt)
{
	Remove-Item $currentFolder\ToInstall\*.*
}
ELSE #Install
{
	#Find exe
	$exec = ForEach-Object { (Get-ChildItem $currentFolder\ToInstall).Name} 
	ForEach-Object { (Write-Host $currentFolder\ToInstall\$exec) }

	#Find db parameter
	$databaseServer = ".\SqlExpress"
	IF (Test-Path $currentFolder\DatabaseServer.txt)
	{
		$databaseServer = Get-Content $currentFolder\DatabaseServer.txt
	}

	#Run installer(s)
	#ForEach-Object { [System.Diagnostics.Process]::Start("$currentFolder\ToInstall\$exec", "/S /CreateDatabase /CreateServiceUser /DatabaseServer:$databaseServer").WaitForExit() }
	ForEach-Object { [System.Diagnostics.Process]::Start("$currentFolder\ToInstall\$exec", "/S /CreateServiceUser /DatabaseServer:$databaseServer").WaitForExit() }

	#Move file(s)	
	ForEach-Object { Move-Item -force $currentFolder\ToInstall\$exec $currentFolder\Installed\$exec } 
}