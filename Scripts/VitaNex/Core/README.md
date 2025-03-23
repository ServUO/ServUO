Vita-Nex: Core (VNc)
Copyright (c) 2016, Vita-Nex | http://core.vita-nex.com

A dynamic extension library for RunUO, written in C#, targeting .NET Framework 4.0

VNc extends the RunUO server emulator software to expose many features and utilities, 
as well as Services and Modules designed to enhance server management and game-play, 
while maintaining 100% "plug & play" capability.

*** Requires RunUO server software be compiled targeting .NET Framework 4.0
*** Supports Mono 2.6, but there are known issues with Mono's support of .NET Framework 4.0 features, such as optional parameters.

***

Installation:

Recommended directory path: Scripts/VitaNex/Core/

Install the entire project to the above recommended directory path;
An example of a correct path is "Scripts/VitaNex/Core/VNC.cfg"
All project files and folders should be installed here.

The above path is recommended, you may use a different path, as long as the project is installed to a sub-directory of Scripts/
For example: "Scripts/Customs/VNc"

When all the files are installed, you should be able to boot up your server without errors, as long as you have met the requirements for installation; see notes below for more information.

***

Notes:

Vita-Nex: Core is designed to be modular and dynamic.

When developing the project, I always consider the end-user and a lot of thought goes in to the implementation of each unique 
feature to try to ensure that you have to the power to modify or extend any aspect of the project without editing it's source files.
There is always a way - if Vita-Nex: Core doesn't do something the way you thought it would, consider checking the source code for a 
way to change that feature without editing the source code directly.

A good example of changing something without editing the source code would be SuperGump's default field and property values.
Considering the default values are used globally throughout SuperGumps, it figures you should be able to change them without
having to edit them directly, they have been defined as "static" instead  of "const", allowing them to be modified by any code.
In this isntance, you would create a new "script" in your customs folder, write a new static class, give the class a static constructor 
and set the default value by accessing it like so: VitaNex.SuperGumps.SuperGump.Default* = *

This doesn't just apply to setting fields and properties, in cases where you need a special class, you should be able to
inherit many of the existing classes provided by the project.
With functionaility and accessibility being key factors in development, attention has been paid to detail in order to ensure 
that there is always a way to change the way things work while maintaining core function integrity by protecting it accordingly.

You've probably done most of this before, but if you haven't, it's implementations like this that make everything easier and safer for you.

***

When you first install Vita-Nex: Core, it will create the necessary directories on the first boot of your shard.
Everything you see in the console output is colored for readability, there is generally a lot more console output when using 
Vita-Nex: Core, but that's because the project ensures you know what's going on when it's important.
Each service and module has it's own message pool in the console and each state will provide you with state information.
It is possible to enable quiet-mode on services and modules when accessing the VNCPanel via the "[VNC <srv | mod>" command, to 
reduce the amount of console output, errors will usually be logged to a file in the project base directory ("VitaNexCore/Logs/").

***

The first thing to do after installing Vita-Nex: Core, is log-in and use one of these commands (no quotes):

"[VNC"
"[VNC SRV"
"[VNC MOD"

You can also use the "[MyCommands" command to see a searchable list of all commands you have access to on the shard.

***

If you have any compile errors when installing the project, it may be because your RunUO requires an update.
For the most part, I have tried to keep the project backward compatible, but some things do get missed from time to time.
The recommended RunUO version to use is 2.7 with .NET Framework 4.0 support, the older the shard, the less compatible it will be.
It is recommended to update RunUO to work with the project, rather than update the project to fix the errors, this will ensure the 
project is consistent and stable.

***

VNC.cfg is important, the project can function without it, but some features may be inaccessible or function in unintended ways, 
though there are fail-safes in place to prevent damage, it is strongly recommended to keep this file in it's original location 
after installing the project.
That original location is designed to be in the same directory as VitaNex.cs, VitaNex_Init.cs, VitaNex_Modules.cs and VitaNex_Services.cs

Modifying the project's directory or file tree by adding, moving, deleting or renaming directories/files can also have unintended consequences, 
it is therefore strongly recommended that you leave everything where it is.

If you want to write your own services or modules, it is recommended that you work in a separate directory to the project root directory.
When I develop custom Vita-Nex: Core content for clients, I typically store the work in a path like "Scripts/VitaNex/Custom/"

***

I updated the file headers with some ASCII Art because it's not as boring as a standard header, if you don't like the header, 
feel free to wipe it, but replace it with the original important information, if releasing publicly.

***

For more information, downloads and other notes like this, visit the project website and wiki.
