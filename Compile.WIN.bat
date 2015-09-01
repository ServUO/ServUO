@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET SRVPATH=%CURPATH%Server\

@TITLE: ServUO - http://www.servuo.com

::##########

@ECHO:
@ECHO: Compile Server for Windows
@ECHO:

@PAUSE

@DEL "%CURPATH%ServUO.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%SevenZipSharp.dll" /target:exe /out:"%CURPATH%ServUO.exe" /recurse:"%SRVPATH%*.cs" /d:ServUO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO:
@ECHO: Ready To Run!
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%ServUO.exe