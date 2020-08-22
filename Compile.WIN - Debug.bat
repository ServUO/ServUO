@SET CURPATH=%~dp0
@SET CSCPATH=%CURPATH%bin\roslyn\

@SET SDKPATH=%CURPATH%Ultima\
@SET SRVPATH=%CURPATH%Server\

@SET EXENAME=ServUO

@TITLE: %EXENAME% - https://www.servuo.com

::##########

@ECHO:
@ECHO: Compile %EXENAME% for Windows
@ECHO:

@PAUSE

dotnet build -c Debug

@ECHO:
@ECHO: Done!
@ECHO:

@PAUSE

@CLS

::##########

@ECHO OFF

"%CURPATH%%EXENAME%.exe"

