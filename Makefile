MCS=mcs
CURPATH=`pwd`
SRVPATH=${CURPATH}/Server
SDKPATH=${CURPATH}/Ultima
REFS=System.Drawing.dll
NOWARNS=0618,0219,0414,1635

PHONY : default build clean run

default: run

debug: 
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -nowarn:${NOWARNS} -d:DEBUG -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:${SDKPATH}/*.cs
	${MCS} -win32icon:${SRVPATH}/servuo.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/ServUO.exe -d:DEBUG -d:ServUO -d:NEWTIMERS -nologo -debug -unsafe -recurse:${SRVPATH}/*.cs
	sed -i.bak -e 's/<!--//g; s/-->//g' ServUO.exe.config

run: build
	${CURPATH}/ServUO.sh

build: ServUO.sh

clean:
	rm -f ServUO.sh
	rm -f ServUO.exe
	rm -f ServUO.exe.mdb
	rm -f Ultima.dll
	rm -f Ultima.dll.mdb
	rm -f *.bin

Ultima.dll: Ultima/*.cs
	${MCS} -target:library -out:${CURPATH}/Ultima.dll -r:${REFS} -nowarn:${NOWARNS} -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:${SDKPATH}/*.cs

ServUO.exe: Ultima.dll Server/*.cs
	${MCS} -win32icon:${SRVPATH}/servuo.ico -r:${CURPATH}/Ultima.dll,${REFS} -nowarn:${NOWARNS} -target:exe -out:${CURPATH}/ServUO.exe -d:ServUO -d:NEWTIMERS -nologo -optimize -unsafe -recurse:${SRVPATH}/*.cs

ServUO.sh: ServUO.exe
	echo "#!/bin/sh" > ${CURPATH}/ServUO.sh
	echo "mono ${CURPATH}/ServUO.exe" >> ${CURPATH}/ServUO.sh
	chmod a+x ${CURPATH}/ServUO.sh
	sed -i.bak -e 's/<!--//g; s/-->//g' ServUO.exe.config
