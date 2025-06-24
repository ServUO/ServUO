/***************************************************************************
 *                             CoreCompiler.cs
 *                            -------------------
 *   derived from         : ScriptCompiler.cs (SVN 313)
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: CoreCompiler.cs 297 2009-05-03 03:57:42Z aldor $
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Microsoft.CSharp;

namespace Launcher
{
    public class CoreCompiler
    {
         public static Assembly[] Assemblies { get; set; }

        public static bool Compile(bool debug, bool cache = false)
        {
            try
            {

                // Find dotnet.exe
                string dotnet = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "dotnet", "dotnet.exe");
                string dotnet_x86 = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "dotnet", "dotnet.exe");

                if (!File.Exists(dotnet))
                {
                    if (File.Exists(dotnet_x86))
                        dotnet = dotnet_x86;
                    else throw new FileNotFoundException("Couldn't find dotnet.exe");
                }
                Console.WriteLine("Found dotnet.exe: {0}", dotnet);

                var assemblies = new HashSet<Assembly>
                {
                    typeof(CoreCompiler).Assembly
                };

                    Console.WriteLine("Launcher: Compiling core...");

                    var path = Path.Combine(Launcher.BaseDirectory, "Server");

                    foreach (var proj in Directory.EnumerateFiles(path, "Server.csproj"))
                    {
                        try
                        {
                            Console.WriteLine("Compiling {0}...", proj);
                            var info = new ProcessStartInfo
                            {
                                FileName = "dotnet",
                                Arguments = $"build \"{proj}\" -c {(debug ? "Debug" : "Release")} --no-dependencies",
                                ErrorDialog = false,
                                UseShellExecute = false,
                                CreateNoWindow = true,
                                WindowStyle = ProcessWindowStyle.Hidden,
                                RedirectStandardOutput = true,
                                WorkingDirectory = Launcher.BaseDirectory
                            };

                            var proc = Process.Start(info);

                            Console.WriteLine(proc.StandardOutput.ReadToEnd());

                            proc.WaitForExit();
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }

                        break;
                    }

                _ = assemblies.Add(Assembly.LoadFrom("Server.dll"));

                Assemblies = assemblies.ToArray();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

                return false;
            }
        }


    }
}
