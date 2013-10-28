@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET EBKPATH=%CURPATH%EmergencyBackup\
@SET SRVPATH=%CURPATH%Server\
@SET SCRPATH=%CURPATH%Scripts\

@TITLE: ServUO - http://www.servuo.com


::##########

@ECHO:
@ECHO: Step 1 - Compile EmergencyBackup
@ECHO:

@PAUSE

@DEL "%CURPATH%EmergencyBackup.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%SevenZipSharp.dll" /target:exe /out:"%CURPATH%EmergencyBackup.exe" /recurse:"%EBKPATH%*.cs" /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########

@ECHO:
@ECHO: Step 2 - Compile ServUO
@ECHO:

@PAUSE

@DEL "%CURPATH%ServUO.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%OpenUO.Core.dll" /r:"%CURPATH%OpenUO.Ultima.dll" /r:"%CURPATH%OpenUO.Ultima.Windows.Forms.dll" /r:"%CURPATH%SevenZipSharp.dll" /target:exe /out:"%CURPATH%ServUO.exe" /recurse:"%SRVPATH%*.cs" /d:Framework_4_0 /d:ServUO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########

@ECHO:
@ECHO: Step 3 - Compile Scripts
@ECHO:

@PAUSE

@DEL "%SCRPATH%Output\Scripts.CS.dll"

@ECHO ON

%CSCPATH%csc.exe /r:"%CURPATH%ServUO.exe" /r:"%CURPATH%OpenUO.Core.dll" /r:"%CURPATH%OpenUO.Ultima.dll" /r:"%CURPATH%OpenUO.Ultima.Windows.Forms.dll" /r:"%CURPATH%SevenZipSharp.dll" /target:library /out:"%SCRPATH%Output\Scripts.CS.dll" /recurse:"%SCRPATH%*.cs" /d:Framework_4_0 /d:ServUO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS


::##########

@ECHO:
@ECHO: Step 4 - Launch ServUO
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%ServUO.exe
