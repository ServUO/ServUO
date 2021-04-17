MCS=mcs
EXENAME=ServUO
CURPATH=`pwd`
CFGPATH=${CURPATH}/Settings
SCRPATH=${CURPATH}/Scripts
SRVPATH=${CURPATH}/Server
APPPATH=${CURPATH}/Application
ICOPATH=${CURPATH}/Application
REFS=System.Drawing.dll,System.Web.dll,System.Data.dll,System.IO.Compression.FileSystem.dll
DEFS=-d:MONO -d:ServUO -d:P58 -d:NEWTIMERS
NOWARNS=0618,0219,0414,1635

PHONY : default build clean run

default: run

debug: 
	${MCS} -target:library -out:"${CURPATH}/Settings.dll" -r:${REFS} -nowarn:${NOWARNS} -d:DEBUG ${DEFS} -nologo -debug -unsafe -recurse:"${CFGPATH}/*.cs"
	${MCS} -target:library -out:"${CURPATH}/Server.dll" -r:"${CURPATH}/Settings.dll",${REFS} -nowarn:${NOWARNS} -d:DEBUG ${DEFS} -nologo -debug -unsafe -recurse:"${SRVPATH}/*.cs"
	${MCS} -target:library -out:"${CURPATH}/Scripts.dll" -r:"${CURPATH}/Settings.dll","${CURPATH}/Server.dll",${REFS} -nowarn:${NOWARNS} -d:DEBUG ${DEFS} -nologo -debug -unsafe -recurse:"${SCRPATH}/*.cs"
	${MCS} -target:exe -out:"${CURPATH}/${EXENAME}.exe" -win32icon:"${ICOPATH}/servuo.ico" -r:"${CURPATH}/Settings.dll","${CURPATH}/Server.dll","${CURPATH}/Scripts.dll",${REFS} -nowarn:${NOWARNS} -d:DEBUG ${DEFS} -nologo -debug -unsafe -recurse:"${APPPATH}/*.cs"
	"${CURPATH}/${EXENAME}.sh"
run: build
	"${CURPATH}/${EXENAME}.sh"

build: ${EXENAME}.sh

clean:
	rm -f ${EXENAME}.sh
	rm -f ${EXENAME}.exe
	rm -f ${EXENAME}.exe.mdb
	rm -f *.bin

Settings.dll: ${CFGPATH}/*.cs
	${MCS} -target:library -out:"${CURPATH}/Settings.dll" -r:${REFS} -nowarn:${NOWARNS} ${DEFS} -nologo -optimize -unsafe -recurse:"${CFGPATH}/*.cs"

Server.dll: Settings.dll ${SRVPATH}/*.cs
	${MCS} -target:library -out:"${CURPATH}/Server.dll" -r:"${CURPATH}/Settings.dll",${REFS} -nowarn:${NOWARNS} ${DEFS} -nologo -optimize -unsafe -recurse:"${SRVPATH}/*.cs"

Scripts.dll: Settings.dll Server.dll ${SCRPATH}/*.cs
	${MCS} -target:library -out:"${CURPATH}/Scripts.dll" -r:"${CURPATH}/Settings.dll","${CURPATH}/Server.dll",${REFS} -nowarn:${NOWARNS} ${DEFS} -nologo -optimize -unsafe -recurse:"${SCRPATH}/*.cs"

${EXENAME}.exe: Settings.dll Server.dll Scripts.dll ${APPPATH}/*.cs 
	${MCS} -target:exe -out:"${CURPATH}/${EXENAME}.exe" -win32icon:"${ICOPATH}/servuo.ico" -r:"${CURPATH}/Settings.dll","${CURPATH}/Server.dll","${CURPATH}/Scripts.dll",${REFS} -nowarn:${NOWARNS} ${DEFS} -nologo -optimize -unsafe -recurse:"${APPPATH}/*.cs"

${EXENAME}.sh: ${EXENAME}.exe
	echo "#!/bin/sh" > "${CURPATH}/${EXENAME}.sh"
	echo "mono \"${CURPATH}/${EXENAME}.exe\"" >> "${CURPATH}/${EXENAME}.sh"
	chmod a+x "${CURPATH}/${EXENAME}.sh"
