;Command line options when running the installer
;/ServiceUser:<account> - service account to use
;/ServicePassword:<password> - password for service account
;/CreateServiceUser - creates a local service account
;/ServiceDomain:<domain> - the service account is in the domain specified. If not specified an account on the local computer is assumed 
;/CreateDatabase - create database, will drop existing database!
;/DatabaseServer:. (or .\SqlExpress if you use sqlexpress or an instance name)

!ifndef APPNAME
	!define APPNAME "Shopgun.Server"
!endif	
!ifndef VERSION
	!define VERSION "1.0.0.0"
!endif	
!ifndef FILESPATH
	!define FILESPATH "..\ApplicationService.Server\bin\debug"
!endif
!define FULLNAME "Consumentor.${APPNAME} v${VERSION}"
!define WINSERVICENAME "Consumentor.${APPNAME}.ConsoleProgram v${VERSION}"
;--------------------------------
; The name of the installer
Name "${FULLNAME}"

; The files to write
Outfile "..\..\Build\\Installers\Consumentor.${APPNAME}.v${VERSION}.exe"

; The default installation directory
InstallDir $PROGRAMFILES\Consumentor\${APPNAME}\v${VERSION}

; Registry key to check for directory (so if you install again, it will  overwrite the old one automatically)
InstallDirRegKey HKLM "Software\Consumentor\$FULLNAME" "Install_Dir"

;--------------------------------
!include ".\ShopgunFunctions.nsh"
;--------------------------------
VIAddVersionKey "ProductName" "${FULLNAME}"
VIAddVersionKey "Comments" ""
VIAddVersionKey "CompanyName" "Consumentor Ek. Förening"
VIAddVersionKey "LegalTrademarks" "${FULLNAME} is a trademark of Consumentor Ek. Förening"
VIAddVersionKey "LegalCopyright" "© Consumentor Ek. Förening"
VIAddVersionKey "FileDescription" "${FULLNAME}"
VIAddVersionKey "FileVersion" "${VERSION}"
VIProductVersion "${VERSION}"
;--------------------------------
; Pages

;Page components
;Page directory
Page instfiles

;UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------
; The stuff to install
Section "Application Files (required)" ;No components page, name is not important
	
	SectionIn RO

	; Set output path to the installation directory.
	SetOutPath $INSTDIR 
	; Put file there
	File /x *.vshost.exe "${FILESPATH}\*.exe"	
	File "${FILESPATH}\*.dll"
	File "${FILESPATH}\*.config"

	; Write the installation path into the registry
	WriteRegStr HKLM "SOFTWARE\Consumentor\${FULLNAME}" "Install_Dir" "$INSTDIR"
	; Write the uninstall keys for Windows
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FULLNAME}" "DisplayName" "${FULLNAME}"
	WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FULLNAME}" "UninstallString" '"$INSTDIR\uninstall.exe"'
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FULLNAME}" "NoModify" 1
	WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FULLNAME}" "NoRepair" 1
	WriteUninstaller "uninstall.exe"
	
	Call .SetExecutable
    Call .CreateUser
    Call .FixSecurity
    Call .UpdateConnectionStringsConfig
    Call .UpdateAppConfig
    ; Call .DatabaseStuff
	Call .InstallUtil
    Call .ReserveUriForUser
	Call .StartService
SectionEnd
;--------------------------------

; Uninstaller
Section "Uninstall"
	; Stopp Service
	ExecWait '"net" stop "${WINSERVICENAME}"'	
	Call Un.InstallUtil

    ; Remove registry keys
    DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\${FULLNAME}"
    DeleteRegKey HKLM "SOFTWARE\Consumentor\${FULLNAME}"
    ; Remove files and uninstaller
    Delete $INSTDIR\*.config
    Delete $INSTDIR\*.dll
    Delete $INSTDIR\*.InstallLog
    Delete $INSTDIR\*.log
    Delete $INSTDIR\*Shopgun*.exe
    Delete $INSTDIR\uninstall.exe

    ; Remove directories used
    RMDir /r /REBOOTOK "$INSTDIR"
SectionEnd
