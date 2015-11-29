Option Explicit
If wscript.Arguments.Count<>2 Then
    MsgBox "I need 2 arguments: className url"
    wscript.Quit
End If

Dim currentClassName
currentClassName = wscript.Arguments(0)
Dim currentUrl
currentUrl = wscript.Arguments(1)

Dim fileName

fileName = currentClassName + ".cs"
Call ReplaceInFile(fileName, "this.Url = """ + currentUrl + """;", "")

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
