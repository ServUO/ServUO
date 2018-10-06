@SET CURPATH=%~dp0
@SET CSCPATH=%windir%\Microsoft.NET\Framework\v4.0.30319\

@SET SDKPATH=%CURPATH%Ultima\
@SET SRVPATH=%CURPATH%Server\

@SET EXENAME=ServUO

@TITLE: %EXENAME% - https://www.servuo.com

::##########

@ECHO:
@ECHO: Compile Ultima SDK
@ECHO:

@PAUSE

@DEL "%CURPATH%Ultima.dll"

@ECHO ON

%CSCPATH%csc.exe /target:library /out:"%CURPATH%Ultima.dll" /recurse:"%SDKPATH%*.cs" /d:ServUO /d:NEWTIMERS /nowarn:0618 /nologo /unsafe /optimize

@ECHO OFF

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO:
@ECHO: Compile %EXENAME% for Windows
@ECHO:

@PAUSE

@DEL "%CURPATH%%EXENAME%.exe"

@ECHO ON

%CSCPATH%csc.exe /win32icon:"%SRVPATH%servuo.ico" /r:"%CURPATH%Ultima.dll" /target:exe /out:"%CURPATH%%EXENAME%.exe" /recurse:"%SRVPATH%*.cs" /d:ServUO /d:NEWTIMERS /d:NETFX_40 /nowarn:0618 /nologo /unsafe /optimize

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

%CURPATH%%EXENAME%.exe
