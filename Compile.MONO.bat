@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET EBKPATH=%CURPATH%EmergencyBackup\
@SET SRVPATH=%CURPATH%Server\
@SET SCRPATH=%CURPATH%Scripts\

@TITLE: ServUO - http://www.servuo.com


::##########

@ECHO:
@ECHO: Step 1 - Compile EmergencyBackup (MONO)
@ECHO:

@PAUSE

@DEL "%CURPATH%EmergencyBackup.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%SevenZipSharp.dll" /target:exe /out:"%CURPATH%EmergencyBackup.exe" /recurse:"%EBKPATH%*.cs" /d:MONO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########

@ECHO:
@ECHO: Step 2 - Compile ServUO (MONO)
@ECHO:

@PAUSE

::#Append .MONO for people who test on WIN and host on MONO
@DEL "%CURPATH%ServUO.MONO.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%OpenUO.Core.dll" /r:"%CURPATH%OpenUO.Ultima.dll" /r:"%CURPATH%OpenUO.Ultima.Windows.Forms.dll" /r:"%CURPATH%SevenZipSharp.dll" /target:exe /out:"%CURPATH%ServUO.MONO.exe" /recurse:"%SRVPATH%*.cs" /d:MONO /d:Framework_4_0 /d:ServUO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########

@ECHO:
@ECHO: Step 3 - Compile Scripts (MONO)
@ECHO:

@PAUSE

@DEL "%SCRPATH%Output\Scripts.CS.dll"

@ECHO ON

%CSCPATH%csc.exe /r:"%CURPATH%ServUO.MONO.exe" /r:"%CURPATH%OpenUO.Core.dll" /r:"%CURPATH%OpenUO.Ultima.dll" /r:"%CURPATH%OpenUO.Ultima.Windows.Forms.dll" /r:"%CURPATH%SevenZipSharp.dll" /target:library /out:"%SCRPATH%Output\Scripts.CS.dll" /recurse:"%SCRPATH%*.cs" /d:MONO /d:Framework_4_0 /d:ServUO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########

@ECHO:
@ECHO: Step 4 - Launch ServUO (MONO)
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%ServUO.MONO.exe
