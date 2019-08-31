MCS=mcs
EXENAME=ServUO
CURPATH=`pwd`
SRVPATH=${CURPATH}/Server
SDKPATH=${CURPATH}/Ultima
REFS=System.Drawing.dll
NOWARNS=0618,0219,0414,1635

PHONY : default build clean run

default: run

debug: 
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -nowarn:${NOWARNS} -d:DEBUG -d:MONO -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:${SDKPATH}/*.cs
	${MCS} -win32icon:${SRVPATH}/servuo.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/${EXENAME}.exe -d:DEBUG -d:MONO -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:${SRVPATH}/*.cs
	sed -i.bak -e 's/<!--//g; s/-->//g' ${EXENAME}.exe.config

run: build
	${CURPATH}/${EXENAME}.sh

build: ${EXENAME}.sh

clean:
	rm -f ${EXENAME}.sh
	rm -f ${EXENAME}.exe
	rm -f ${EXENAME}.exe.mdb
	rm -f Ultima.dll
	rm -f Ultima.dll.mdb
	rm -f *.bin

Ultima.dll: Ultima/*.cs
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -nowarn:${NOWARNS} -d:MONO -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:${SDKPATH}/*.cs

${EXENAME}.exe: Ultima.dll Server/*.cs
	${MCS} -win32icon:${SRVPATH}/servuo.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/${EXENAME}.exe -d:MONO -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:${SRVPATH}/*.cs

${EXENAME}.sh: ${EXENAME}.exe
	echo "#!/bin/sh" > ${CURPATH}/${EXENAME}.sh
	echo "mono ${CURPATH}/${EXENAME}.exe" >> ${CURPATH}/${EXENAME}.sh
	chmod a+x ${CURPATH}/${EXENAME}.sh
	sed -i.bak -e 's/<!--//g; s/-->//g' ${EXENAME}.exe.config
