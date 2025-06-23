/***************************************************************************
 *                                  Main.cs
 *                            -------------------
 *   begin                : May 1, 2002
 *   copyright            : (C) The RunUO Software Team
 *   email                : info@runuo.com
 *
 *   $Id: Main.cs 2250 2008-09-15 23:22:16Z rayparker $
 *   $Author: rayparker $
 *   $Date: 2008-09-16 01:22:16 +0200 (Di, 16 Sep 2008) $
 *
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
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;

namespace Launcher
{
	public class Launcher
	{
		private static string m_BaseDirectory;
		private static string m_ExePath;
		private static ArrayList m_DataDirectories = new ArrayList();
		private static Assembly m_Assembly;
		private static Process m_Process;
		private static Thread m_Thread;

		public static ArrayList DataDirectories { get { return m_DataDirectories; } }
		public static Assembly Assembly { get { return m_Assembly; } set { m_Assembly = value; } }
		public static Process Process { get { return m_Process; } }
        public static Thread Thread { get { return m_Thread; } }
        public static bool Precompiled { get; private set; }

		public static bool HaltOnWarning { get { return false; } }

		public static string ExePath
		{
			get
			{
				if( m_ExePath == null )
					m_ExePath = Assembly.GetExecutingAssembly().Location; //Kelon: Geändert um vshost debugging zu unterstuetzen

				return m_ExePath;
			}
		}

		public static string BaseDirectory
		{
			get
			{
				if( m_BaseDirectory == null )
				{
					try
					{
						m_BaseDirectory = ExePath;

						if( m_BaseDirectory.Length > 0 )
							m_BaseDirectory = Path.GetDirectoryName( m_BaseDirectory );
					}
					catch
					{
						m_BaseDirectory = "";
					}
				}

				return m_BaseDirectory;
			}
		}

		public static void Main( string[] args )
		{
			bool debug = Compiler.Default.Debug;

			for( int i = 0; i < args.Length; ++i )
			{
				if ( CaseInsensitiveComparer.Default.Compare( args[i], "-debug" ) == 0 )
					debug = true;
                if( CaseInsensitiveComparer.Default.Compare( args[i], "-precompiled" ) == 0 )
                    Precompiled = true;
			}

			m_Thread = Thread.CurrentThread;
			m_Process = Process.GetCurrentProcess();
			m_Assembly = Assembly.GetExecutingAssembly();

			if( BaseDirectory.Length > 0 )
				Directory.SetCurrentDirectory( BaseDirectory );

			Version ver = m_Assembly.GetName().Version;

			// Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
			Console.WriteLine( "RunUOLauncher - Version {0}.{1}.{3}, Build {2}", ver.Major, ver.Minor, ver.Revision, ver.Build );

			while( !CoreCompiler.Compile( debug ) )
			{
				Console.WriteLine( "Core: One or more scripts failed to compile or no script files were found." );
				Console.WriteLine( " - Press return to exit, or R to try again." );

				string line = Console.ReadLine();
				if( line == null || line.ToLower() != "r" )
					return;
			}

			Type serverCoreType = null;

			foreach ( Assembly asm in CoreCompiler.Assemblies )
			{
				foreach ( Type type in asm.GetTypes() )
				{
					if ( type.FullName == "Server.Starter" )
					{
						serverCoreType = type;
						break;
					}
				}
				if ( serverCoreType != null )
				{
					break;
				}
			}
			if(serverCoreType == null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine( "Core Starter class not found!" );
				Console.ReadLine();
			}
			else
			{
				IRun starter = (IRun)Activator.CreateInstance(serverCoreType);
				try
				{
					starter.Run(args);
				}
				catch
				{
					Console.ForegroundColor = ConsoleColor.Red;
					throw;
				}
			}
		}
	}
}
