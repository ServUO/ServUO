#region Header
// **********
// ServUO - Main.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CustomsFramework;

using Server.Network;
using System.Collections;
#endregion

namespace Server
{
	public delegate void Slice();

	public static class Core
	{
		static Core()
		{
			DataDirectories = new List<string>();

			GlobalMaxUpdateRange = 24;
			GlobalUpdateRange = 18;
            GlobalRadarRange = 37;
		}

		public static bool Crashed { get { return _Crashed; } }

		private static bool _Crashed;
		private static Thread _TimerThread;
		private static string _BaseDirectory;
		private static string _ExePath;

		private static bool _Cache = true;

		private static bool _Profiling;
		private static DateTime _ProfileStart;
		private static TimeSpan _ProfileTime;

		public static MessagePump MessagePump { get; set; }

		public static Slice Slice;

		public static bool Profiling
		{
			get { return _Profiling; }
			set
			{
				if (_Profiling == value)
				{
					return;
				}

				_Profiling = value;

				if (_ProfileStart > DateTime.MinValue)
				{
					_ProfileTime += DateTime.UtcNow - _ProfileStart;
				}

				_ProfileStart = (_Profiling ? DateTime.UtcNow : DateTime.MinValue);
			}
		}

		public static TimeSpan ProfileTime
		{
			get
			{
				if (_ProfileStart > DateTime.MinValue)
				{
					return _ProfileTime + (DateTime.UtcNow - _ProfileStart);
				}

				return _ProfileTime;
			}
		}

		public static bool Service { get; private set; }

        public static bool NoConsole { get; private set; }
		public static bool Debug { get; private set; }

		public static bool HaltOnWarning { get; private set; }
		public static bool VBdotNet { get; private set; }

		public static List<string> DataDirectories { get; private set; }

		public static Assembly Assembly { get; set; }

		public static Version Version { get { return Assembly.GetName().Version; } }

		public static Process Process { get; private set; }
		public static Thread Thread { get; private set; }

		public static MultiTextWriter MultiConsoleOut { get; private set; }

		/* 
		 * DateTime.Now and DateTime.UtcNow are based on actual system clock time.
		 * The resolution is acceptable but large clock jumps are possible and cause issues.
		 * GetTickCount and GetTickCount64 have poor resolution.
		 * GetTickCount64 is unavailable on Windows XP and Windows Server 2003.
		 * Stopwatch.GetTimestamp() (QueryPerformanceCounter) is high resolution, but
		 * somewhat expensive to call because of its defference to DateTime.Now,
		 * which is why Stopwatch has been used to verify HRT before calling GetTimestamp(),
		 * enabling the usage of DateTime.UtcNow instead.
		 */

		private static readonly bool _HighRes = Stopwatch.IsHighResolution;

		private static readonly double _HighFrequency = 1000.0 / Stopwatch.Frequency;
		private const double _LowFrequency = 1000.0 / TimeSpan.TicksPerSecond;

		private static bool _UseHRT;

		public static bool UsingHighResolutionTiming { get { return _UseHRT && _HighRes && !Unix; } }

		public static long TickCount { get { return (long)Ticks; } }

		public static double Ticks
		{
			get
			{
				if (_UseHRT && _HighRes && !Unix)
				{
					return Stopwatch.GetTimestamp() * _HighFrequency;
				}

				return DateTime.UtcNow.Ticks * _LowFrequency;
			}
		}

		public static readonly bool Is64Bit = Environment.Is64BitProcess;

		public static bool MultiProcessor { get; private set; }
		public static int ProcessorCount { get; private set; }

		public static bool Unix { get; private set; }
		
		public static string FindDataFile(string path)
		{
			if (DataDirectories.Count == 0)
			{
				throw new InvalidOperationException("Attempted to FindDataFile before DataDirectories list has been filled.");
			}

			string fullPath = null;

			foreach (string p in DataDirectories)
			{
				fullPath = Path.Combine(p, path);

				if (File.Exists(fullPath))
				{
					break;
				}

				fullPath = null;
			}

			return fullPath;
		}

		public static string FindDataFile(string format, params object[] args)
		{
			return FindDataFile(String.Format(format, args));
		}

		#region Expansions
		public static Expansion Expansion { get; set; }

		public static bool T2A { get { return Expansion >= Expansion.T2A; } }
		public static bool UOR { get { return Expansion >= Expansion.UOR; } }
		public static bool UOTD { get { return Expansion >= Expansion.UOTD; } }
		public static bool LBR { get { return Expansion >= Expansion.LBR; } }
		public static bool AOS { get { return Expansion >= Expansion.AOS; } }
		public static bool SE { get { return Expansion >= Expansion.SE; } }
		public static bool ML { get { return Expansion >= Expansion.ML; } }
		public static bool SA { get { return Expansion >= Expansion.SA; } }
		public static bool HS { get { return Expansion >= Expansion.HS; } }
		public static bool TOL { get { return Expansion >= Expansion.TOL; } }
		public static bool EJ { get { return Expansion >= Expansion.EJ; } }
		#endregion

		public static string ExePath { get { return _ExePath ?? (_ExePath = Assembly.Location); } }

		public static string BaseDirectory
		{
			get
			{
				if (_BaseDirectory == null)
				{
					try
					{
						_BaseDirectory = ExePath;

						if (_BaseDirectory.Length > 0)
						{
							_BaseDirectory = Path.GetDirectoryName(_BaseDirectory);
						}
					}
					catch
					{
						_BaseDirectory = "";
					}
				}

				return _BaseDirectory;
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(e.IsTerminating ? "Error:" : "Warning:");
			Console.WriteLine(e.ExceptionObject);

			if (e.IsTerminating)
			{
				_Crashed = true;

				bool close = false;

				try
				{
					CrashedEventArgs args = new CrashedEventArgs(e.ExceptionObject as Exception);

					EventSink.InvokeCrashed(args);

					close = args.Close;
				}
				catch
				{ }

				if (!close && !Service)
				{
					try
					{
						foreach (Listener l in MessagePump.Listeners)
						{
							l.Dispose();
						}
					}
					catch
					{ }

					Console.WriteLine("This exception is fatal, press return to exit");
					Console.ReadLine();
				}

				Kill();
			}
		}

		private enum ConsoleEventType
		{
			CTRL_C_EVENT,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}

		private delegate bool ConsoleEventHandler(ConsoleEventType type);

		private static ConsoleEventHandler m_ConsoleEventHandler;

		private static class UnsafeNativeMethods
		{
			[DllImport("Kernel32")]
			public static extern bool SetConsoleCtrlHandler(ConsoleEventHandler callback, bool add);
		}

		private static bool OnConsoleEvent(ConsoleEventType type)
		{
			if (World.Saving || (Service && type == ConsoleEventType.CTRL_LOGOFF_EVENT))
			{
				return true;
			}

			Kill(); //Kill -> HandleClosed will handle waiting for the completion of flushing to disk

			return true;
		}

		private static void CurrentDomain_ProcessExit(object sender, EventArgs e)
		{
			HandleClosed();
		}

		public static bool Closing { get; private set; }

		private static int _CycleIndex = 1;
		private static readonly float[] _CyclesPerSecond = new float[100];

		public static float CyclesPerSecond { get { return _CyclesPerSecond[(_CycleIndex - 1) % _CyclesPerSecond.Length]; } }

		public static float AverageCPS { get { return _CyclesPerSecond.Take(_CycleIndex).Average(); } }

		public static void Kill()
		{
			Kill(false);
		}

		public static void Kill(bool restart)
		{
			HandleClosed();

			if (restart)
			{
				Process.Start(ExePath, Arguments);
			}

			Process.Kill();
		}

		private static void HandleClosed()
		{
			if (Closing)
			{
				return;
			}

			Closing = true;

            if(Debug)
                Console.Write("Exiting...");

			World.WaitForWriteCompletion();

			if (!_Crashed)
			{
				EventSink.InvokeShutdown(new ShutdownEventArgs());
			}

			Timer.TimerThread.Set();

            if (Debug)
                Console.WriteLine("done");
		}

		private static readonly AutoResetEvent _Signal = new AutoResetEvent(true);

		public static void Set()
		{
			_Signal.Set();
		}

		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

			foreach (string a in args)
			{
				if (Insensitive.Equals(a, "-debug"))
				{
					Debug = true;
				}
				else if (Insensitive.Equals(a, "-service"))
				{
					Service = true;
				}
				else if (Insensitive.Equals(a, "-profile"))
				{
					Profiling = true;
				}
				else if (Insensitive.Equals(a, "-nocache"))
				{
					_Cache = false;
				}
				else if (Insensitive.Equals(a, "-haltonwarning"))
				{
					HaltOnWarning = true;
				}
				else if (Insensitive.Equals(a, "-vb"))
				{
					VBdotNet = true;
				}
				else if (Insensitive.Equals(a, "-usehrt"))
				{
					_UseHRT = true;
				}
                else if (Insensitive.Equals(a, "-noconsole"))
                {
                    NoConsole = true;
                }
                else if (Insensitive.Equals(a, "-h") || Insensitive.Equals(a, "-help"))
                {
                    Console.WriteLine("An Ultima Online server emulator written in C# - Visit https://www.servuo.com for more information.\n\n");
                    Console.WriteLine(System.AppDomain.CurrentDomain.FriendlyName + " [Parameter]\n\n");
                    Console.WriteLine("     -debug              Starting ServUO in Debug Mode. Debug Mode is being used in Core and Scripts to give extended inforamtion during runtime.");
                    Console.WriteLine("     -haltonwarning      ServUO halts if any warning is raised during compilation of scripts.");
                    Console.WriteLine("     -h or -help         Displays this help text.");
                    Console.WriteLine("     -nocache            No known effect.");
                    Console.WriteLine("     -noconsole          No user interaction during startup and runtime.");
                    Console.WriteLine("     -profile            Enables profiling allowing to get performance diagnostic information of packets, timers etc. in AdminGump -> Maintenance. Use with caution. This increases server load.");
                    Console.WriteLine("     -service            This parameter should be set if you're running ServUO as a Windows Service. No user interaction. *Windows only*");
                    Console.WriteLine("     -usehrt             Enables High Resolution Timing if requirements are met. Increasing the resolution of the timer. *Windows only*");
                    Console.WriteLine("     -vb                 Enables compilation of VB.NET Scripts. Without this option VB.NET Scripts are skipped.");

                    System.Environment.Exit(0);
                }
            }

            if (!Environment.UserInteractive || Service)
            {
                NoConsole = true;
            }

			try
			{
				if (Service)
				{
					if (!Directory.Exists("Logs"))
					{
						Directory.CreateDirectory("Logs");
					}

					Console.SetOut(MultiConsoleOut = new MultiTextWriter(new FileLogger("Logs/Console.log")));
				}
				else
				{
					Console.SetOut(MultiConsoleOut = new MultiTextWriter(Console.Out));
				}
			}
			catch
			{ }

			Thread = Thread.CurrentThread;
			Process = Process.GetCurrentProcess();
			Assembly = Assembly.GetEntryAssembly();

			if (Thread != null)
			{
				Thread.Name = "Core Thread";
			}

			if (BaseDirectory.Length > 0)
			{
				Directory.SetCurrentDirectory(BaseDirectory);
			}

			Timer.TimerThread ttObj = new Timer.TimerThread();

			_TimerThread = new Thread(ttObj.TimerMain)
			{
				Name = "Timer Thread"
			};

			Version  ver 		= Assembly.GetName().Version;
			DateTime buildDate 	= new DateTime(2000, 1, 1).AddDays(ver.Build).AddSeconds(ver.Revision * 2);
			

			Utility.PushColor(ConsoleColor.Cyan);
        #if DEBUG
            Console.WriteLine(
                "ServUO - [https://www.servuo.com] Version {0}.{1}, Build {2}.{3} - Build on {4} UTC - Debug",
                ver.Major,
                ver.Minor,
                ver.Build,
                ver.Revision,
				buildDate);
        #else
            Console.WriteLine(
				"ServUO - [https://www.servuo.com] Version {0}.{1}, Build {2}.{3} - Build on {4} UTC - Release",
				ver.Major,
				ver.Minor,
				ver.Build,
				ver.Revision,
				buildDate);
        #endif
			Utility.PopColor();

			string s = Arguments;

            if (s.Length > 0)
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine("Core: Running with arguments: {0}", s);
				Utility.PopColor();
			}

			ProcessorCount = Environment.ProcessorCount;

			if (ProcessorCount > 1)
			{
				MultiProcessor = true;
			}

			if (MultiProcessor || Is64Bit)
			{
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine(
					"Core: Optimizing for {0} {2}processor{1}",
					ProcessorCount,
					ProcessorCount == 1 ? "" : "s",
					Is64Bit ? "64-bit " : "");
				Utility.PopColor();
			}
			
			string dotnet = null;

			if (Type.GetType("Mono.Runtime") != null)
			{	
				MethodInfo displayName = Type.GetType("Mono.Runtime").GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);
				if (displayName != null)
				{
					dotnet = displayName.Invoke(null, null).ToString();
					
					Utility.PushColor(ConsoleColor.Yellow);
					Console.WriteLine("Core: Unix environment detected");
					Utility.PopColor();
					
					Unix = true;
				}
			}
			else
			{
				m_ConsoleEventHandler = OnConsoleEvent;
				UnsafeNativeMethods.SetConsoleCtrlHandler(m_ConsoleEventHandler, true);
			}
            
            #if NETFX_30
                        dotnet = "3.0";
            #endif

            #if NETFX_35
                        dotnet = "3.5";
            #endif

            #if NETFX_40
                        dotnet = "4.0";
            #endif

            #if NETFX_45
                        dotnet = "4.5";
            #endif

            #if NETFX_451
                        dotnet = "4.5.1";
            #endif

            #if NETFX_46
                        dotnet = "4.6.0";
            #endif

            #if NETFX_461
                        dotnet = "4.6.1";
            #endif

            #if NETFX_462
                        dotnet = "4.6.2";
            #endif

            #if NETFX_47
                        dotnet = "4.7";
            #endif

            #if NETFX_471
                        dotnet = "4.7.1";
            #endif

            if (String.IsNullOrEmpty(dotnet))
                dotnet = "MONO/CSC/Unknown";
            
            Utility.PushColor(ConsoleColor.Green);
            Console.WriteLine("Core: Compiled for " + ( Unix ? "MONO and running on {0}" : ".NET {0}" ), dotnet);
            Utility.PopColor();

			if (GCSettings.IsServerGC)
			{
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine("Core: Server garbage collection mode enabled");
				Utility.PopColor();
			}

			if (_UseHRT)
			{
				Utility.PushColor(ConsoleColor.DarkYellow);
				Console.WriteLine(
					"Core: Requested high resolution timing ({0})",
					UsingHighResolutionTiming ? "Supported" : "Unsupported");
				Utility.PopColor();
			}

			Utility.PushColor(ConsoleColor.DarkYellow);
			Console.WriteLine("RandomImpl: {0} ({1})", RandomImpl.Type.Name, RandomImpl.IsHardwareRNG ? "Hardware" : "Software");
			Utility.PopColor();

			Utility.PushColor(ConsoleColor.Green);
			Console.WriteLine("Core: Loading config...");
			Config.Load();
			Utility.PopColor();

			while (!ScriptCompiler.Compile(Debug, _Cache))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Scripts: One or more scripts failed to compile or no script files were found.");
				Utility.PopColor();

				if (Service)
				{
					return;
				}

				Console.WriteLine(" - Press return to exit, or R to try again.");

                if (Console.ReadKey(true).Key != ConsoleKey.R)
				{
					return;
				}
			}

			ScriptCompiler.Invoke("Configure");

			Region.Load();
			World.Load();

			ScriptCompiler.Invoke("Initialize");

			MessagePump messagePump = MessagePump = new MessagePump();

			_TimerThread.Start();

			foreach (Map m in Map.AllMaps)
			{
				m.Tiles.Force();
			}

			NetState.Initialize();

			EventSink.InvokeServerStarted();

			try
			{
				long now, last = TickCount;

				const int sampleInterval = 100;
				const float ticksPerSecond = 1000.0f * sampleInterval;

				long sample = 0;

				while (!Closing)
				{
					_Signal.WaitOne();

					Mobile.ProcessDeltaQueue();
					Item.ProcessDeltaQueue();

					Timer.Slice();
					messagePump.Slice();

					NetState.FlushAll();
					NetState.ProcessDisposedQueue();

					if (Slice != null)
					{
						Slice();
					}

					if (sample++ % sampleInterval != 0)
					{
						continue;
					}

					now = TickCount;
					_CyclesPerSecond[_CycleIndex++ % _CyclesPerSecond.Length] = ticksPerSecond / (now - last);
					last = now;
				}
			}
			catch (Exception e)
			{
				CurrentDomain_UnhandledException(null, new UnhandledExceptionEventArgs(e, true));
			}
		}

		public static string Arguments
		{
			get
			{
				StringBuilder sb = new StringBuilder();

				if (Debug)
				{
					Utility.Separate(sb, "-debug", " ");
				}

				if (Service)
				{
					Utility.Separate(sb, "-service", " ");
				}

				if (Profiling)
				{
					Utility.Separate(sb, "-profile", " ");
				}

				if (!_Cache)
				{
					Utility.Separate(sb, "-nocache", " ");
				}

				if (HaltOnWarning)
				{
					Utility.Separate(sb, "-haltonwarning", " ");
				}

				if (VBdotNet)
				{
					Utility.Separate(sb, "-vb", " ");
				}

				if (_UseHRT)
				{
					Utility.Separate(sb, "-usehrt", " ");
				}

                if (NoConsole)
                {
                    Utility.Separate(sb, "-noconsole", " ");
                }

				return sb.ToString();
			}
		}

		public static int GlobalUpdateRange { get; set; }
		public static int GlobalMaxUpdateRange { get; set; }
        public static int GlobalRadarRange { get; set; }
		
		private static int m_ItemCount, m_MobileCount, m_CustomsCount;

		public static int ScriptItems { get { return m_ItemCount; } }
		public static int ScriptMobiles { get { return m_MobileCount; } }
		public static int ScriptCustoms { get { return m_CustomsCount; } }

		public static void VerifySerialization()
		{
			m_ItemCount = 0;
			m_MobileCount = 0;
			m_CustomsCount = 0;

			VerifySerialization(Assembly.GetCallingAssembly());

			foreach (Assembly a in ScriptCompiler.Assemblies)
			{
				VerifySerialization(a);
			}
		}

		private static readonly Type[] m_SerialTypeArray = {typeof(Serial)};
		private static readonly Type[] m_CustomsSerialTypeArray = {typeof(CustomSerial)};

		private static void VerifyType(Type t)
		{
			bool isItem = t.IsSubclassOf(typeof(Item));

			if (isItem || t.IsSubclassOf(typeof(Mobile)))
			{
				if (isItem)
				{
					Interlocked.Increment(ref m_ItemCount);
				}
				else
				{
					Interlocked.Increment(ref m_MobileCount);
				}

				StringBuilder warningSb = null;

				try
				{
					if (t.GetConstructor(m_SerialTypeArray) == null)
					{
						warningSb = new StringBuilder();

						warningSb.AppendLine("       - No serialization constructor");
					}

					if (
						t.GetMethod(
							"Serialize",
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Serialize() method");
					}

					if (
						t.GetMethod(
							"Deserialize",
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Deserialize() method");
					}

					if (warningSb != null && warningSb.Length > 0)
					{
						Utility.PushColor(ConsoleColor.Yellow);
						Console.WriteLine("Warning: {0}\n{1}", t, warningSb);
						Utility.PopColor();
					}
				}
				catch
				{
					Utility.PushColor(ConsoleColor.Yellow);
					Console.WriteLine("Warning: Exception in serialization verification of type {0}", t);
					Utility.PopColor();
				}
			}
			else if (t.IsSubclassOf(typeof(SaveData)))
			{
				Interlocked.Increment(ref m_CustomsCount);

				StringBuilder warningSb = null;

				try
				{
					if (t.GetConstructor(m_CustomsSerialTypeArray) == null)
					{
						warningSb = new StringBuilder();

						warningSb.AppendLine("       - No serialization constructor");
					}

					if (
						t.GetMethod(
							"Serialize",
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Serialize() method");
					}

					if (
						t.GetMethod(
							"Deserialize",
							BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Deserialize() method");
					}

					if (warningSb != null && warningSb.Length > 0)
					{
						Utility.PushColor(ConsoleColor.Yellow);
						Console.WriteLine("Warning: {0}\n{1}", t, warningSb);
						Utility.PopColor();
					}
				}
				catch
				{
					Utility.PushColor(ConsoleColor.Yellow);
					Console.WriteLine("Warning: Exception in serialization verification of type {0}", t);
					Utility.PopColor();
				}
			}
		}

		private static void VerifySerialization(Assembly a)
		{
			if (a != null)
			{
				Parallel.ForEach(a.GetTypes(), VerifyType);
			}
		}
	}

	public class FileLogger : TextWriter
	{
		public const string DateFormat = "[MMMM dd hh:mm:ss.f tt]: ";

		private bool _NewLine;

		public string FileName { get; private set; }

		public FileLogger(string file)
			: this(file, false)
		{ }

		public FileLogger(string file, bool append)
		{
			FileName = file;

			using (
				var writer =
					new StreamWriter(
						new FileStream(FileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				writer.WriteLine(">>>Logging started on {0}.", DateTime.UtcNow.ToString("f"));
				//f = Tuesday, April 10, 2001 3:51 PM 
			}

			_NewLine = true;
		}

		public override void Write(char ch)
		{
			using (var writer = new StreamWriter(new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
			{
				if (_NewLine)
				{
					writer.Write(DateTime.UtcNow.ToString(DateFormat));
					_NewLine = false;
				}

				writer.Write(ch);
			}
		}

		public override void Write(string str)
		{
			using (var writer = new StreamWriter(new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
			{
				if (_NewLine)
				{
					writer.Write(DateTime.UtcNow.ToString(DateFormat));
					_NewLine = false;
				}

				writer.Write(str);
			}
		}

		public override void WriteLine(string line)
		{
			using (var writer = new StreamWriter(new FileStream(FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
			{
				if (_NewLine)
				{
					writer.Write(DateTime.UtcNow.ToString(DateFormat));
				}

				writer.WriteLine(line);
				_NewLine = true;
			}
		}

		public override Encoding Encoding { get { return Encoding.Default; } }
	}

	public class MultiTextWriter : TextWriter
	{
		private readonly List<TextWriter> _Streams;

		public MultiTextWriter(params TextWriter[] streams)
		{
			_Streams = new List<TextWriter>(streams);

			if (_Streams.Count < 0)
			{
				throw new ArgumentException("You must specify at least one stream.");
			}
		}

		public void Add(TextWriter tw)
		{
			_Streams.Add(tw);
		}

		public void Remove(TextWriter tw)
		{
			_Streams.Remove(tw);
		}

		public override void Write(char ch)
		{
			foreach (var t in _Streams)
			{
				t.Write(ch);
			}
		}

		public override void WriteLine(string line)
		{
			foreach (var t in _Streams)
			{
				t.WriteLine(line);
			}
		}

		public override void WriteLine(string line, params object[] args)
		{
			WriteLine(String.Format(line, args));
		}

		public override Encoding Encoding { get { return Encoding.Default; } }
	}
}
