MCS=mcs
EXENAME=ServUO
CURPATH=`pwd`
SCRPATH=${CURPATH}/Scripts
SRVPATH=${CURPATH}/Server
APPPATH=${CURPATH}/Application
ICOPATH=${CURPATH}/Application
REFS=System.Drawing.dll,System.Web.dll,System.Data.dll,System.IO.Compression.FileSystem.dll
NOWARNS=0618,0219,0414,1635

PHONY : default build clean run

default: run

debug: 
	${MCS} -target:library -out:"${CURPATH}/Server.dll" -r:${REFS} -nowarn:${NOWARNS} -d:DEBUG -d:MONO -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:"${SRVPATH}/*.cs"
	${MCS} -target:library -out:"${CURPATH}/Scripts.dll" -r:$"{CURPATH}/Server.dll",${REFS} -nowarn:${NOWARNS} -d:MONO -d:DEBUG -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:"${SCRPATH}/*.cs"
	${MCS} -win32icon:"${ICOPATH}/servuo.ico" -r:"${CURPATH}/Server.dll","${CURPATH}/Scripts.dll",${REFS} -nowarn:${NOWARNS} -target:exe -out:"${CURPATH}/${EXENAME}.exe" -d:DEBUG -d:MONO -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:"${APPPATH}/*.cs"
	"${CURPATH}/${EXENAME}.sh"
run: build
	"${CURPATH}/${EXENAME}.sh"

build: ${EXENAME}.sh

clean:
	rm -f ${EXENAME}.sh
	rm -f ${EXENAME}.exe
	rm -f ${EXENAME}.exe.mdb
	rm -f Ultima.dll
	rm -f Ultima.dll.mdb
	rm -f Scripts.dll
	rm -f *.bin


Server.dll: Server/*.cs
	${MCS} -target:library -out:"${CURPATH}/Server.dll" -r:${REFS} -nowarn:${NOWARNS} -d:MONO -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:"${SRVPATH}/*.cs"

Scripts.dll: Server.dll Scripts/
	${MCS} -target:library -out:"${CURPATH}/Scripts.dll" -r:"${CURPATH}/Server.dll",${REFS} -nowarn:${NOWARNS} -d:MONO -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:"${SCRPATH}/*.cs"

${EXENAME}.exe: Server.dll Scripts.dll Application/*.cs 
	${MCS} -win32icon:"${ICOPATH}/servuo.ico" -r:"${CURPATH}/Server.dll","${CURPATH}/Scripts.dll",${REFS} -nowarn:${NOWARNS} -target:exe -out:"${CURPATH}/${EXENAME}.exe" -d:MONO -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:"./Application/*.cs"

${EXENAME}.sh: ${EXENAME}.exe
	echo "#!/bin/sh" > "${CURPATH}/${EXENAME}.sh"
	echo "mono \"${CURPATH}/${EXENAME}.exe\"" >> "${CURPATH}/${EXENAME}.sh"
	chmod a+x "${CURPATH}/${EXENAME}.sh"
