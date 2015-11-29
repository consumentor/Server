!include "Locate.nsh"
!include "FileFunc.nsh"
!include "xml.nsh"
!include "StrFunc.nsh"
!include "LogicLib.nsh"

!insertmacro GetParameters
!insertmacro GetOptions
!insertmacro DirState


# Declare used functions
${StrRep}
${StrLoc}

XPStyle on
; Request application privileges for Windows Vista
RequestExecutionLevel admin

Var Executable

;--------------------------------
Function .SetExecutable
	${Locate::Open} "$INSTDIR" "/L=F /G=0 /M=Consumentor.Shopgun.*.EXE" $0
	; Find File(s) (/L=F), Not in subdirs (/G=0), with name matching (/M=....)
	${locate::Find} $0 $1 $2 $3 $4 $5 $6
    StrCpy $Executable $1
	${Locate::Close} $0	
FunctionEnd

Function .FixSecurity

    Call .GetServiceDomain
    Call .GetServiceUser

    DetailPrint "FixSecurity for user: $R5\$R3"     

    ; Create Logs-directory and set read and write permissions
    CreateDirectory "$INSTDIR\Logs"
    AccessControl::GrantOnFile "$INSTDIR\Logs" "$R5\$R3" "FullAccess"
    DetailPrint "Setting FullAccess on $INSTDIR\Logs"
FunctionEnd

Function .GetDatabaseServer
    ${GetOptions} $R0 "/DatabaseServer:" $R2
	IfErrors 0 +2
    StrCpy $R2 ".\SqlExpress" ; DB server
FunctionEnd

Function .GetServiceUser
    ${GetOptions} $R0 "/ServiceUser:" $R3
    IfErrors 0 +2
    StrCpy $R3 "${APPNAME}" ; Set default user	
FunctionEnd

Function .GetServicePassword
    ${GetOptions} $R0 "/ServicePassword:" $R4
    IfErrors 0 +2
    StrCpy $R4 "qEtUo_12358" ; Set default password
FunctionEnd

Function .GetServiceDomain
	${GetOptions} $R0 "/ServiceDomain:" $R5
    IfErrors 0 +2
	ReadRegStr $R5 HKLM "SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName" "ComputerName"
FunctionEnd

Function .GetServer
	${GetOptions} $R0 "/DmsServer:" $R6
    IfErrors 0 +2
    StrCpy $R6 "localhost" 
FunctionEnd

Function .GetComputerName
	ReadRegStr $R7 HKLM "SYSTEM\CurrentControlSet\Control\ComputerName\ComputerName" "ComputerName"
FunctionEnd

Function .UpdateAppConfig
    ; Call .UpdateP2SimAppConfig
    ; Call .UpdatePrintingPressAppConfig
    ; Call .UpdateLaminatorAppConfig
    ; Call .UpdatePlcAppConfig

    Call .GetServer
    Call .GetComputerName
    Call .SetExecutable

	${xml::LoadFile} "$Executable.config" $0
    ${xml::GotoPath} "/configuration/appSettings/add" $0
	

    ${xml::SaveFile} "$Executable.config" $0
	#Used only for unload plugin
	${xml::NodeHandle} $0

	${xml::Unload}
FunctionEnd

Function .UpdateConnectionStringsConfig
    ${GetParameters} $R0
    Call .GetDatabaseServer
    ;$R2 has database
	${xml::LoadFile} "ConnectionStrings.config" $0
    ${xml::GotoPath} "/connectionStrings/add[1]" $0
    ${xml::GetAttribute} "connectionString" $0 $1 
    DetailPrint "connectionString: $1"
    ${StrRep} $1 $0 "Server=.\SqlExpress" "Server=$R2"
    DetailPrint "New connectionString: $1"
	${xml::SetAttribute} "connectionString" $1 $0
    ${xml::SaveFile} "ConnectionStrings.config" $0
	${xml::Unload}
FunctionEnd

;Debug tip: Change -Q to -q then you can se the error result of the query. type quit in command window to continue.
Function .ReserveUriForUser
    ExecWait '"$Executable" ReserveUri $R5\$R3'
FunctionEnd

Function .GrantDBAccess
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "CREATE USER [$R5\$R3] FOR LOGIN [$R5\$R3] WITH DEFAULT_SCHEMA=[DMS_USER]"'
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "GRANT CONNECT TO [$R5\$R3]"'			
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "CREATE LOGIN [$R5\$R3] FROM WINDOWS WITH DEFAULT_DATABASE=[${APPNAME}], DEFAULT_LANGUAGE=[us_english]"'
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "CREATE SCHEMA [DMS_USER] AUTHORIZATION [$R5\$R3]"'
	StrCpy $R9 "SP_ADDROLEMEMBER 'db_datareader', [$R5\$R3]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'		
	StrCpy $R9 "SP_ADDROLEMEMBER 'db_datawriter', [$R5\$R3]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	
	${If} ${APPNAME} != "DMS.Server"
		Call .GrantDBAccessClientSpecific
	${EndIf}
FunctionEnd

Function .GrantDBAccessClientSpecific
	StrCpy $R9 "ALTER DATABASE ${PLCDATABASE} SET ENABLE_BROKER"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "GRANT CONNECT TO [$R5\$R3]"'
	
	StrCpy $R9 "SP_ADDROLEMEMBER 'db_datareader', [$R5\$R3]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	
	StrCpy $R9 "SP_ADDROLEMEMBER 'db_datawriter', [$R5\$R3]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	
	StrCpy $R9 "CREATE ROLE db_executor"	
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'	
	StrCpy $R9 "GRANT EXECUTE ON ${PLCTRUNCATEPROCEDURE} TO db_executor"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'		
	StrCpy $R9 "EXEC SP_ADDROLEMEMBER db_executor, [$R5\$R3]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	
	StrCpy $R9 "EXEC SP_ADDROLE 'sql_dependency_subscriber'"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	StrCpy $R9 "GRANT CREATE PROCEDURE TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	StrCpy $R9 "GRANT CREATE QUEUE TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	StrCpy $R9 "GRANT CREATE SERVICE TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	StrCpy $R9 "GRANT SUBSCRIBE QUERY NOTIFICATIONS TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	StrCpy $R9 "GRANT REFERENCES on CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] to [sql_dependency_subscriber]"
    ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
    StrCpy $R9 "GRANT RECEIVE ON QueryNotificationErrorsQueue TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
	StrCpy $R9 "EXEC SP_ADDROLEMEMBER 'sql_dependency_subscriber', [$R5\$R3]"
    ExecWait '"sqlcmd" -S $R2 -d ${PLCDATABASE} -Q "$R9"'
FunctionEnd

Function .DatabaseStuff
    ${GetParameters} $R0
    Call .GetDatabaseServer
	Call .GetServiceUser
	Call .GetServiceDomain
    ${GetOptions} $R0 "/CreateDatabase" $R1
    IfErrors end

	; Create database
	ExecWait '"$Executable" CreateDatabase'
	
	Call .GrantDBAccess

    ;Enable SqlDependency
    ;SQL Notifications
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "ALTER AUTHORIZATION ON DATABASE::[${APPNAME}] TO sa"'
	ExecWait '"sqlcmd" -S $R2 -d master -Q "ALTER DATABASE [${APPNAME}] SET ENABLE_BROKER"'
    
    StrCpy $R9 "EXEC sp_addrole 'sql_dependency_subscriber'"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
    StrCpy $R9 "CREATE ROLE [sql_dependency_user]"
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'

    ;Permissions needed for [sql_dependency_subscriber]
	StrCpy $R9 "GRANT CREATE PROCEDURE TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	StrCpy $R9 "GRANT CREATE QUEUE TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	StrCpy $R9 "GRANT CREATE SERVICE TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
    StrCpy $R9 "GRANT REFERENCES on CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] to [sql_dependency_subscriber]"
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
    StrCpy $R9 "GRANT VIEW DEFINITION TO [sql_dependency_subscriber]"        
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	StrCpy $R9 "GRANT SELECT TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	StrCpy $R9 "GRANT SUBSCRIBE QUERY NOTIFICATIONS TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	StrCpy $R9 "GRANT RECEIVE ON QueryNotificationErrorsQueue TO [sql_dependency_subscriber]"
	ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'	
    
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "GRANT REFERENCES on CONTRACT::[http://schemas.microsoft.com/SQL/Notifications/PostQueryNotification] to [$R5\$R3]"'
	StrCpy $R9 "EXEC sp_addrolemember 'sql_dependency_subscriber', [$R5\$R3]"
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'
	StrCpy $R9 "EXEC sp_addrolemember 'sql_dependency_user', [$R5\$R3]"
    ExecWait '"sqlcmd" -S $R2 -d ${APPNAME} -Q "$R9"'

    end:
FunctionEnd

Function .CreateUser
    ${GetParameters} $R0
    ${GetOptions} $R0 "/CreateServiceUser" $R3
    IfErrors end

	Call .GetServiceUser
	Call .GetServicePassword
	
	; Get domain
	${GetOptions} $R0 "/ServiceDomain:" $R5
    IfErrors +2 0
	StrCpy $5 "/DOMAIN"
	; Create user
	ExecWait '"net" user $R3 $R4 /ADD /passwordchg:no /expires:never /fullname:" Consumentor ${APPNAME}" $R5' $0
	end:
FunctionEnd

Function .StartService
	; Start Service
	ExecWait '"net" start "${WINSERVICENAME}"'
FunctionEnd

Function .InstallUtil
	Call .GetServiceUser
	Call .GetServicePassword
	Call .GetServiceDomain
	
	; Register windows service
	${Locate::Open} "$INSTDIR" "/L=F /G=0 /M=Consumentor.Shopgun.*.EXE" $0
	; Find File(s) (/L=F), Not in subdirs (/G=0), with name matching (/M=....)
	${locate::Find} $0 $1 $2 $3 $4 $5 $6
    StrCpy $Executable $1
	ExecWait '"$WINDIR\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" /ServiceUser=$R3 /ServicePassword=$R4 /ServiceDomain=$R5 "$1"' $0	
	${Locate::Close} $0	
FunctionEnd

Function Un.InstallUtil
; Un-Register windows service
    ReadRegStr $9 HKLM "SOFTWARE\Consumentor\${FULLNAME}" "Install_Dir"
  	${locate::Open} "$9" "/L=F /G=0 /M=Consumentor.Shopgun.*.EXE" $0
	; Find File(s) (/L=F), Not in subdirs (/G=0), with name matching (/M=....)
	${locate::Find} $0 $1 $2 $3 $4 $5 $6
	ExecWait '"$WINDIR\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe" /u "$1"' $0
	${locate::Close} $0
FunctionEnd