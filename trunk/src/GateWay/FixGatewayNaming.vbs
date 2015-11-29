Option Explicit

If wscript.Arguments.Count<>4 Then
    MsgBox "I need 4 arguments: CurrentClassFileName CurrentInterfaceFileName CurrentClassName NewClassName"
    wscript.Quit
End If

Dim currentClassFile, currentInterfaceFile, currentClassName, newClassName, path
currentClassFile = wscript.Arguments(0)
currentInterfaceFile  = wscript.Arguments(1)
currentClassName  = wscript.Arguments(2)
newClassName = wscript.Arguments(3)
path = GetPath(currentClassFile)

' Load content
Dim currentClassFileContent, currentInterfaceFileContent
currentClassFileContent = ReadFile(currentClassFile)
currentInterfaceFileContent  = ReadFile(currentInterfaceFile)
'Delete files
call DeleteFile (currentClassFile)
call DeleteFile (currentInterfaceFile)
'Replace
currentClassFileContent = Replace(currentClassFileContent,"class " & currentClassName, "class " & newClassName)
currentClassFileContent = Replace(currentClassFileContent,"public " + currentClassName + "()", "public " & newClassName + "()")
currentClassFileContent = Replace(currentClassFileContent, "I" & currentClassName, "I" & newClassName)
currentInterfaceFileContent = Replace(currentInterfaceFileContent, "I" + currentClassName, "I" + newClassName)
'Save
call SaveFile(path + "\" + newclassName + ".cs", currentClassFileContent)
call SaveFile(path + "\I" + newclassName + ".cs", currentInterfaceFileContent)

'''''''''''''''''''''''''''''''''''''''''''''''''
Function GetPath(fileName)
    Dim p, f, fso
    set fso = CreateObject("Scripting.FileSystemObject")
    
    set f = fso.GetFile(fileName)
    p = f.ParentFolder
    GetPath = p    
End Function

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

Sub DeleteFile(fileName)
    With CreateObject("Scripting.FileSystemObject")
	    .DeleteFile(fileName)
    End With
End Sub
    