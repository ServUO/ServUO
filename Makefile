MCS=mcs
CURPATH=`pwd`
SRVPATH=${CURPATH}/Server
SDKPATH=${CURPATH}/Ultima
REFS=System.Drawing.dll
NOWARNS=0618,0219,0414,1635

PHONY : default build clean run

default: run

run: build
	${CURPATH}/ServUO.sh

build: ServUO.sh

clean:
	rm -f ServUO.sh
	rm -f ServUO.MONO.exe
	rm -f ServUO.MONO.pdb
	rm -f Ultima.dll
	rm -f Ultima.pdb
	rm -f *.bin

Ultima.dll: Ultima/*.cs
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -d:ServUO -d:NEWTIMERS -nowarn:${NOWARNS} -debug -nologo -optimize -unsafe -recurse:${SDKPATH}/*.cs

ServUO.MONO.exe: Ultima.dll Server/*.cs
	${MCS} -win32icon:${SRVPATH}/servuo.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/ServUO.MONO.exe -d:ServUO -d:NEWTIMERS -d:MONO -debug -nologo -optimize -unsafe -recurse:${SRVPATH}/*.cs

ServUO.sh: ServUO.MONO.exe
	echo "#!/bin/sh" > ${CURPATH}/ServUO.sh
	echo "mono ${CURPATH}/ServUO.MONO.exe" >> ${CURPATH}/ServUO.sh
	chmod a+x ${CURPATH}/ServUO.sh
