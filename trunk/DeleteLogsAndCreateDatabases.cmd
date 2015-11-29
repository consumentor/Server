del /S *DMS*.log
del /S *DMS*.log.*

@ECHO OFF
src\P2Simulator\bin\debug\TetraPak.ICIC.DMS.P2Simulator.exe CreateDatabase >CreateDatabase.log
src\ApplicationService.Server\bin\debug\TetraPak.ICIC.DMS.ApplicationService.Server.exe CreateDatabase >>CreateDatabase.log
src\ApplicationService.PrintingPress\bin\debug\TetraPak.ICIC.DMS.ApplicationService.PrintingPress.exe CreateDatabase >>CreateDatabase.log
src\ApplicationService.Laminator\bin\debug\TetraPak.ICIC.DMS.ApplicationService.Laminator.exe CreateDatabase >>CreateDatabase.log
src\ApplicationService.Slitter\bin\debug\TetraPak.ICIC.DMS.ApplicationService.Slitter.exe CreateDatabase >>CreateDatabase.log

sqlcmd -S .\SqlExpress -d Dms.P2Simulator -Q "ALTER AUTHORIZATION ON DATABASE::[Dms.P2Simulator] TO sa"
