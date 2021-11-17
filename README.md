# [ServUO]

[![Build Status](https://travis-ci.com/ServUO/ServUO.svg?branch=master)](https://travis-ci.com/ServUO/ServUO)
[![GitHub issues](https://img.shields.io/github/issues/servuo/servuo.svg)](https://github.com/ServUO/ServUO/issues)
[![GitHub release](https://img.shields.io/github/release/servuo/servuo.svg)](https://github.com/ServUO/ServUO/releases)
[![GitHub repo size](https://img.shields.io/github/repo-size/servuo/servuo.svg)](https://github.com/ServUO/ServUO/)
[![Discord](https://img.shields.io/discord/110970849628000256.svg)](https://discord.gg/0cQjvnFUN26nRt7y)
[![GitHub contributors](https://img.shields.io/github/contributors/servuo/servuo.svg)](https://github.com/ServUO/ServUO/graphs/contributors)
[![GitHub](https://img.shields.io/github/license/servuo/servuo.svg?color=a)](https://github.com/ServUO/ServUO/blob/master/LICENSE)


[ServUO] is a community driven Ultima Online Server Emulator written in C#.


#### Requirements

[.NET 4.8] Runtime and Developer Pack
[.NET 5.0] Runtime and SDK
[Visual Studio] for editing and compiling.


#### Windows

Run `_windebug.bat` for development environments.
Run `_winrelease.bat` for production environments.


#### OSX
```
brew install mono
brew install dotnet
dotnet build
```
To run `mono ServUO.exe`


#### Ubuntu / Debian
```
apt-get install zlib1g-dev mono-complete dotnet-sdk-5.0 
dotnet build
```
To run `mono ServUO.exe`


#### Summary

1. Starting with the `/Config` directory, find and edit `Server.cfg` to set up the essentials.
2. Go through the remaining `*.cfg` files to ensure they suit your needs.
3. For Windows, run `_windebug.bat` to produce `ServUO.exe`, Linux users may run `Makefile`.
4. Run `ServUO.exe` to make sure everything boots up, if everything went well, you should see your IP adress being listened on the port you specified.
5. Load up UO and login! - If you require instructions on setting up your particular client, visit the Discord and ask!

   [ServUO]: <https://www.servuo.com>
   [.NET 4.8]: <https://dotnet.microsoft.com/download/dotnet-framework/net48>
   [.NET 5.0]: <https://dotnet.microsoft.com/download/dotnet/5.0>
   [Visual Studio]: <https://visualstudio.microsoft.com/>
