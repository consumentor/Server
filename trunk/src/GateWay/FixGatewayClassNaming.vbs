Option Explicit

call Main()

Sub Main()
    dim fileNames, contentInFiles, newContentInFiles, fso, i, dir
    
    If wscript.Arguments.Count>0 Then
        dir = wscript.Arguments(0)
    Else
        dir = "."
    End If

    Set fso = CreateObject("Scripting.FileSystemObject")
    fileNames = GetFileNames(dir)
    contentInFiles = LoadFiles(fileNames)
    newContentInFiles = ReplaceInFiles(contentInFiles, fileNames)
    SaveFiles contentInFiles, newContentInFiles, fileNames
End Sub
'''''''''''''''''''''''''''''''''''''''''''''''''
Function RegExReplace(pattern, content, replaceWith)
    Dim regEx
    Set regEx = New RegExp
    regEx.Pattern = pattern
    RegExReplace = regEx.Replace(content, replaceWith)
End Function
'''''''''''''''''''''''''''''''''''''''''''''''''
Sub SaveFiles(oldContent, newContent, fileNames)
    Dim i
    For i=0 To UBound(fileNames)
        If (oldContent(i)<>newContent(i)) Then
            call SaveFile(fileNames(i), newContent(i))
        End If
    Next
End Sub
'''''''''''''''''''''''''''''''''''''''''''''''''
Function ReplaceInFiles(content, fileNames)
    Dim newContent(), i, publicClassPos, classNames(), fullString, j    
    ReDIM newContent(UBound(fileNames))
    ReDIM classNames(UBound(fileNames))
    
    For i=0 to UBound(fileNames)
        If (Right(LCase(fileNames(i)),10) <> "gateway.cs") AND (Right(LCase(fileNames(i)),10) <> "factory.cs") Then 'Dont Parse the "sender class/interface"
            publicClassPos = InStr(content(i), "public partial class") + 21
            If publicClassPos>21 Then
                classNames(i) = Mid(content(i), publicClassPos, InStr(publicClassPos,content(i), vbCrLf) - publicClassPos)
                IF (InStr(1, classNames(i),":") > 1) Then
                    classNames(i) = Left(classNames(i), InStr(1,classNames(i)," "))
                End If
                classNames(i) = Trim(classNames(i))
                fullString = "class " + classNames(i)
                newContent(i) = Replace(content(i), fullString, fullString + "GWO")
                'constructor
                newContent(i) = Replace(newContent(i), "public " + classNames(i) +"(", "public " + classNames(i) +"GWO(")
                newContent(i) = Replace(newContent(i), "internal " + classNames(i) +"(", "internal " + classNames(i) +"GWO(")
            ELSE
                newContent(i) = content(i)
            End If
        ELSE
            newContent(i) = content(i)
        End If
    Next
    'Update all classNames
    For i=0 To UBound(fileNames)
        For j=0 To UBound(fileNames)
            If Right(LCase(fileNames(i)),10) <> "factory.cs" Then 'Dont change the factory class
                if (Len(classNames(j))>0) Then
                    'types, ex FaultCodeList faultCodes;
                    newContent(i) = Replace(newContent(i), " " + classNames(j) + " ", " " + classNames(j) + "GWO ")
                    'inheritance
                    newContent(i) = Replace(newContent(i), ": " + classNames(j)  + vbCrLf, ": " + classNames(j) + "GWO" + vbCrLf)
                    'generics
                    newContent(i) = Replace(newContent(i), "<" + classNames(j) + ">", "<" + classNames(j) + "GWO>")
                    'Arrays
                    newContent(i) = Replace(newContent(i), classNames(j) + "[]", classNames(j) + "GWO[]")
                    'typeof
                    newContent(i) = Replace(newContent(i), "typeof(" + classNames(j) + ")", "typeof(" + classNames(j) +"GWO"  + ")")
                    'cast
                    newContent(i) = Replace(newContent(i), "(" + classNames(j) + ")", "(" + classNames(j) +"GWO)")
                    'first param in function
                    newContent(i) = Replace(newContent(i), "(" + classNames(j) + " ", "(" + classNames(j) +"GWO ")
                    'new 
                    newContent(i) = Replace(newContent(i), "new " + classNames(j) + "(", "new " + classNames(j) +"GWO(")

                    'And remove one line
                    newContent(i) = RegExReplace("// File time \d{2}-\d{2}-\d{2} \d{2}:\d{2}", newContent(i), "// data objects suffixed with GWO")
                End If
            End If        
        Next
    Next
    ReplaceInFiles = newContent
End function
'''''''''''''''''''''''''''''''''''''''''''''''''
Function LoadFiles(files)
    Dim fileContent(), i
    ReDIM fileContent(UBound(files))
    For i=0 To UBound(files)
        fileContent(i) = ReadFile(files(i))
    Next
    LoadFiles = fileContent
End function
'''''''''''''''''''''''''''''''''''''''''''''''''
Function GetFileNames(path)
    Dim folder, files(), nrOfFiles, i, f, fso
    Set fso = CreateObject("Scripting.FileSystemObject")
    Set folder = fso.GetFolder(path)
    ReDim files(CountFiles(folder.files))
    i=0
    For Each f in folder.files
       If LCase(Right(f,3)) = ".cs" Then
            files(i) = f
            i = i + 1
       End If
    Next
    GetFileNames = files
End Function
'''''''''''''''''''''''''''''''''''''''''''''''''
Function CountFiles(files)
    Dim i, f
    i=0
    For Each f in files
       If LCase(Right(f,3)) = ".cs" Then
            i = i + 1
       End If
    Next
    CountFiles = i - 1
End Function
'''''''''''''''''''''''''''''''''''''''''''''''''
Function GetPath(fileName)
    Dim p, f, fs
    set fs = CreateObject("Scripting.FileSystemObject")
    set f = fs.GetFile(fileName)
    p = f.ParentFolder
    GetPath = p    
End Function

Function ReadFile(fileName)
    Dim text, fso, file
    Set fso = CreateObject("Scripting.FileSystemObject")
	Set file = fso.OpenTextFile(fileName)
	text = file.ReadAll()
    file.Close()
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
    