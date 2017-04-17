# !!! HIGHLY EXPERIMENTAL !!!
# 
# TODO:
#	- ENV Server.cfg
#	- Replace DataPath
#	- Fix BankCheck.cs & XmlSpawner


FROM mono:latest

MAINTAINER ServUO Team

RUN 	apt-get update \
	&& apt-get install -y zlib1g unzip \
	&& rm -rf /var/lib/apt/lists/* /tmp/*

COPY	./	/srv/ServUO/

RUN	mcs -target:library -out:/srv/ServUO/Ultima.dll -r:System.Drawing -d:ServUO -d:NEWTIMERS -nowarn:0618,0219,0414,1635 -debug -nologo -optimize -unsafe -recurse:/srv/ServUO/Ultima/*.cs

RUN	mcs -optimize+ -unsafe -t:exe -out:/srv/ServUO/ServUO.MONO.exe -win32icon:/srv/ServUO/Server/servuo.ico -nowarn:0618,0219,0414,1635 -d:NEWTIMERS -d:NEWPARENT -d:MONO -reference:System.Drawing,/srv/ServUO/Ultima.dll -recurse:/srv/ServUO/Server/*.cs

RUN mkdir /srv/ServUO/ServerData

VOLUME ["/srv/ServUO/ServerData", "/srv/ServUO/Config", "/srv/ServUO/Saves", "/srv/ServUO/Backups"]

EXPOSE	2593

ENTRYPOINT ["/srv/ServUO/Docker/entrypoint.sh"]
