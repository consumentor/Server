#Uninstall script

#Get current folder
$fullCurrentFolder = [System.IO.Path]::GetDirectoryName($myinvocation.mycommand.path)
$currentFolder = $fullCurrentFolder.Substring($fullCurrentFolder.LastIndexOf("\")+1)

IF (Test-Path $fullCurrentFolder\DontInstall.txt)
{
    #Dont uninstall either
}
ELSE #UnInstall
{
    #Get unistall path via registry 
    ls HKLM:\Software\Microsoft\Windows\CurrentVersion\UnInstall |	
		    Where-Object {$_.Name -like "*Consumentor*" -and $_.Name.Contains($currentFolder)} | 
		    ForEach-Object { (get-ItemProperty -path registry::$_).UninstallString }
    $programToUninstall = ls HKLM:\Software\Microsoft\Windows\CurrentVersion\UnInstall | 
				    Where-Object {$_.Name -like "*Consumentor*" -and $_.Name.Contains($currentFolder)} 

    #Run uninstaller
    ForEach-Object { [Diagnostics.Process]::Start((get-ItemProperty -path registry::$programToUninstall).UninstallString, "/S").WaitForExit() }

    Move-Item -force "$fullCurrentFolder\Installed\*.exe" "$fullCurrentFolder\UnInstalled\"
}