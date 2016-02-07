@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET SRVPATH=%CURPATH%Server\

@TITLE: ServUO - http://www.servuo.com

::##########

@ECHO:
@ECHO: Compile Ultima SDK
@ECHO:

@PAUSE

@DEL "%CURPATH%Ultima.dll"

@ECHO ON

%CSCPATH%csc.exe /target:library /out:"%CURPATH%Ultima.dll" /recurse:"%SDKPATH%*.cs" /d:ServUO /d:NEWTIMERS /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO:
@ECHO: Compile Server for Mono
@ECHO:

@PAUSE

@DEL "%CURPATH%ServUO.MONO.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%Ultima.dll" /target:exe /out:"%CURPATH%ServUO.MONO.exe" /recurse:"%SRVPATH%*.cs" /d:ServUO /d:NEWTIMERS /d:MONO /nowarn:0618 /debug /nologo /optimize /unsafe

@ECHO OFF

::########## Silent compile of Windows ServUO.exe
@DEL "%CURPATH%ServUO.exe"
%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%Ultima.dll" /target:exe /out:"%CURPATH%ServUO.exe" /recurse:"%SRVPATH%*.cs" /d:ServUO /d:NEWTIMERS /nowarn:0618 /debug /nologo /optimize /unsafe
::##########

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO:
@ECHO: Ready To Run! (Windows)
@ECHO:

@PAUSE

@CLS

@ECHO OFF

%CURPATH%ServUO.exe