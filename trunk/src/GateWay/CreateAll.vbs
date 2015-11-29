'call GenerateGateWay("..\P2Simulator\bin\debug", "TetraPak.ICiC.DMS.P2Simulator.exe StartServices", "P2Simulator\CreateGateways.cmd")

call GenerateGateWay("..\ApplicationService.Server\bin\debug", "Consumentor.ShopGun.ApplicationService.Server.exe StartServices", "Server\CreateGateways.cmd")

'call GenerateGateWay("..\ApplicationService.PrintingPress\bin\debug", "TetraPak.ICiC.DMS.ApplicationService.PrintingPress.exe StartServices", "CallbackWebService\CreateGateway.cmd")

''''''''''''''''''''''''''''''''''''
' Allow servers to quit gracefully
wscript.Sleep 100
''''''''''''''''''''''''''''''''''''
Sub GenerateGateWay(exeServerFolder, exeServer, cmdScript)
    Dim currentDirectory 
	
	Set shell = WScript.CreateObject("WScript.Shell")
    Set shellServer = WScript.CreateObject("WScript.Shell")
	currentDirectory = shellServer.CurrentDirectory
	
	shellServer.CurrentDirectory = exeServerFolder
    shellServer.Run exeServer, 1, false
    wscript.Sleep 1500

    shell.CurrentDirectory = currentDirectory
	shell.Run cmdScript, 1, true
    wscript.Sleep 50
    shellServer.SendKeys "{ENTER}"

End Sub

Sub WaitForText(stdOut)
    Dim strLine 
    Do
        strLine = stdOut.ReadLine
    Loop Until InStr(strLine,"[Enter]")
End Sub

Sub WaitToQuit(theShell)
    Do Until theShell.Status<>0
        wscript.Sleep 10
    Loop
End Sub