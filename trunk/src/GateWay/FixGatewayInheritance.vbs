Option Explicit
If wscript.Arguments.Count<>1 Then
    MsgBox "I need 1 arguments: className"
    wscript.Quit
End If

Dim currentClassName
currentClassName = wscript.Arguments(0)



Dim fileName

fileName = currentClassName + ".cs"
Call ReplaceInFile(fileName, ": System.Web.Services.Protocols.SoapHttpClientProtocol", ": Consumentor.ShopGun.Gateway.WebServiceBase")

fileName = "I" + currentClassName + ".cs"
Call ReplaceInFile(fileName, "I" + currentClassName, "I" + currentClassName + " : Consumentor.ShopGun.Gateway.IWebServiceSettings")


''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''''
Sub ReplaceInFile(fileName, replaceThis, withThis)
    Dim text
    
    text = ReadFile(fileName)
    text = Replace(text, replaceThis, withThis)
    SaveFile fileName, text
End Sub

Function ReadFile(fileName)
    Dim text

    With CreateObject("Scripting.FileSystemObject")
	    With .OpenTextFile(fileName)
		    text = .ReadAll()
		    .Close()
	    End With
    End With
    ReadFile = text
End Function    

Sub SaveFile(fileName, content)
    With CreateObject("Scripting.FileSystemObject")
	    With .CreateTextFile(fileName, true)
		    .Write(content)
		    .Close()
	    End With
    End With
End Sub
