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
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CustomsFramework;

using Server.Network;
#endregion

namespace Server
{
	public delegate void Slice();

	public static class Core
	{
		private static bool m_Crashed;
		private static Thread timerThread;
		private static string m_BaseDirectory;
		private static string m_ExePath;
		private static readonly List<string> m_DataDirectories = new List<string>();
		private static Assembly m_Assembly;
		private static Process m_Process;
		private static Thread m_Thread;
		private static bool m_Service;
		private static bool m_Debug;
		private static bool m_Cache = true;
		private static bool m_HaltOnWarning;
		private static bool m_VBdotNET;
		private static MultiTextWriter m_MultiConOut;
		private static OpenUOSDK _openUOSDK;

		private static bool m_Profiling;
		private static DateTime m_ProfileStart;
		private static TimeSpan m_ProfileTime;

		private static MessagePump m_MessagePump;

		public static MessagePump MessagePump { get { return m_MessagePump; } set { m_MessagePump = value; } }

		public static Slice Slice;

		public static bool Profiling
		{
			get { return m_Profiling; }
			set
			{
				if (m_Profiling == value)
				{
					return;
				}

				m_Profiling = value;

				if (m_ProfileStart > DateTime.MinValue)
				{
					m_ProfileTime += DateTime.UtcNow - m_ProfileStart;
				}

				m_ProfileStart = (m_Profiling ? DateTime.UtcNow : DateTime.MinValue);
			}
		}

		public static TimeSpan ProfileTime
		{
			get
			{
				if (m_ProfileStart > DateTime.MinValue)
				{
					return m_ProfileTime + (DateTime.UtcNow - m_ProfileStart);
				}

				return m_ProfileTime;
			}
		}

		public static bool Service { get { return m_Service; } }
		public static bool Debug { get { return m_Debug; } }
		internal static bool HaltOnWarning { get { return m_HaltOnWarning; } }
		internal static bool VBdotNet { get { return m_VBdotNET; } }
		public static List<string> DataDirectories { get { return m_DataDirectories; } }
		public static Assembly Assembly { get { return m_Assembly; } set { m_Assembly = value; } }
		public static Version Version { get { return m_Assembly.GetName().Version; } }
		public static Process Process { get { return m_Process; } }
		public static Thread Thread { get { return m_Thread; } }
		public static MultiTextWriter MultiConsoleOut { get { return m_MultiConOut; } }

		public static OpenUOSDK OpenUOSDK { get { return _openUOSDK; } }

#if false && !MONO
		[DllImport("kernel32")]
		private static extern long GetTickCount64();
#endif

		/* DateTime.Now and DateTime.UtcNow depend on the system time which is undesirable.
		 * GetTickCount64 is unavailable on Windows XP and Windows Server 2003.
		 * Stopwatch.GetTimestamp() (QueryPerformanceCounter) is high resolution,
		 * but expensive to call and unreliable with certain system configurations.
		 */

		/* The following implementation is an effective substitute for GetTickCount64 that
		 * is reliable as long as it is retrieved once every 2^32 ms (~49 days).
		 */

		private static readonly ThreadLocal<long> _HighOrder = new ThreadLocal<long>();
		private static readonly ThreadLocal<uint> _LastTickCount = new ThreadLocal<uint>();

		private static readonly double _Frequency = 1000.0 / Stopwatch.Frequency;

		public static long TickCount
		{
			get
			{
				if (Stopwatch.IsHighResolution) // TODO: Unreliable with certain system configurations.
				{
					return (long)(Stopwatch.GetTimestamp() * _Frequency);
				}

				uint t = (uint)Environment.TickCount;

				if (_LastTickCount.Value > t) // Wrapped
				{
					_HighOrder.Value += 0x100000000;
				}

				_LastTickCount.Value = t;

				return _HighOrder.Value | _LastTickCount.Value;
			}
		}

		public static readonly bool Is64Bit = Environment.Is64BitProcess;

		private static bool m_MultiProcessor;
		private static int m_ProcessorCount;

		public static bool MultiProcessor { get { return m_MultiProcessor; } }
		public static int ProcessorCount { get { return m_ProcessorCount; } }

		private static bool m_Unix;

		public static bool Unix { get { return m_Unix; } }

		public static string FindDataFile(string path)
		{
			if (m_DataDirectories.Count == 0)
			{
				throw new InvalidOperationException("Attempted to FindDataFile before DataDirectories list has been filled.");
			}

			string fullPath = null;

			for (int i = 0; i < m_DataDirectories.Count; ++i)
			{
				fullPath = Path.Combine(m_DataDirectories[i], path);

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
		private static Expansion m_Expansion;
		public static Expansion Expansion { get { return m_Expansion; } set { m_Expansion = value; } }

		public static bool T2A { get { return m_Expansion >= Expansion.T2A; } }

		public static bool UOR { get { return m_Expansion >= Expansion.UOR; } }

		public static bool UOTD { get { return m_Expansion >= Expansion.UOTD; } }

		public static bool LBR { get { return m_Expansion >= Expansion.LBR; } }

		public static bool AOS { get { return m_Expansion >= Expansion.AOS; } }

		public static bool SE { get { return m_Expansion >= Expansion.SE; } }

		public static bool ML { get { return m_Expansion >= Expansion.ML; } }

		public static bool SA { get { return m_Expansion >= Expansion.SA; } }

		public static bool HS { get { return m_Expansion >= Expansion.HS; } }
		#endregion

		public static string ExePath
		{
			get
			{
				if (m_ExePath == null)
				{
					m_ExePath = Assembly.Location;
					//m_ExePath = Process.GetCurrentProcess().MainModule.FileName;
				}

				return m_ExePath;
			}
		}

		public static string BaseDirectory
		{
			get
			{
				if (m_BaseDirectory == null)
				{
					try
					{
						m_BaseDirectory = ExePath;

						if (m_BaseDirectory.Length > 0)
						{
							m_BaseDirectory = Path.GetDirectoryName(m_BaseDirectory);
						}
					}
					catch
					{
						m_BaseDirectory = "";
					}
				}

				return m_BaseDirectory;
			}
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine(e.IsTerminating ? "Error:" : "Warning:");
			Console.WriteLine(e.ExceptionObject);

			if (e.IsTerminating)
			{
				m_Crashed = true;

				bool close = false;

				try
				{
					CrashedEventArgs args = new CrashedEventArgs(e.ExceptionObject as Exception);

					EventSink.InvokeCrashed(args);

					close = args.Close;
				}
				catch
				{ }

				if (!close && !m_Service)
				{
					try
					{
						for (int i = 0; i < m_MessagePump.Listeners.Length; i++)
						{
							m_MessagePump.Listeners[i].Dispose();
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

		internal enum ConsoleEventType
		{
			CTRL_C_EVENT,
			CTRL_BREAK_EVENT,
			CTRL_CLOSE_EVENT,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT
		}

		internal delegate bool ConsoleEventHandler(ConsoleEventType type);

		internal static ConsoleEventHandler m_ConsoleEventHandler;

		internal class UnsafeNativeMethods
		{
			[DllImport("Kernel32")]
			internal static extern bool SetConsoleCtrlHandler(ConsoleEventHandler callback, bool add);
		}

		private static bool OnConsoleEvent(ConsoleEventType type)
		{
			if (World.Saving || (m_Service && type == ConsoleEventType.CTRL_LOGOFF_EVENT))
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

		private static bool m_Closing;
		public static bool Closing { get { return m_Closing; } }

		private static long m_CycleIndex = 1;
		private static readonly float[] m_CyclesPerSecond = new float[100];

		public static float CyclesPerSecond { get { return m_CyclesPerSecond[(m_CycleIndex - 1) % m_CyclesPerSecond.Length]; } }

		public static float AverageCPS
		{
			get
			{
				float t = 0.0f;
				int c = 0;

				for (int i = 0; i < m_CycleIndex && i < m_CyclesPerSecond.Length; ++i)
				{
					t += m_CyclesPerSecond[i];
					++c;
				}

				return (t / Math.Max(c, 1));
			}
		}

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

			m_Process.Kill();
		}

		private static void HandleClosed()
		{
			if (m_Closing)
			{
				return;
			}

			m_Closing = true;

			Console.Write("Exiting...");

			World.WaitForWriteCompletion();

			if (!m_Crashed)
			{
				EventSink.InvokeShutdown(new ShutdownEventArgs());
			}

			Timer.TimerThread.Set();

			Console.WriteLine("done");
		}

		private static readonly AutoResetEvent m_Signal = new AutoResetEvent(true);

		public static void Set()
		{
			m_Signal.Set();
		}

		public static void Main(string[] args)
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

			for (int i = 0; i < args.Length; ++i)
			{
				if (Insensitive.Equals(args[i], "-debug"))
				{
					m_Debug = true;
				}
				else if (Insensitive.Equals(args[i], "-service"))
				{
					m_Service = true;
				}
				else if (Insensitive.Equals(args[i], "-profile"))
				{
					Profiling = true;
				}
				else if (Insensitive.Equals(args[i], "-nocache"))
				{
					m_Cache = false;
				}
				else if (Insensitive.Equals(args[i], "-haltonwarning"))
				{
					m_HaltOnWarning = true;
				}
				else if (Insensitive.Equals(args[i], "-vb"))
				{
					m_VBdotNET = true;
				}
			}

			try
			{
				if (m_Service)
				{
					if (!Directory.Exists("Logs"))
					{
						Directory.CreateDirectory("Logs");
					}

					Console.SetOut(m_MultiConOut = new MultiTextWriter(new FileLogger("Logs/Console.log")));
				}
				else
				{
					Console.SetOut(m_MultiConOut = new MultiTextWriter(Console.Out));
				}
			}
			catch
			{ }

			m_Thread = Thread.CurrentThread;
			m_Process = Process.GetCurrentProcess();
			m_Assembly = Assembly.GetEntryAssembly();

			if (m_Thread != null)
			{
				m_Thread.Name = "Core Thread";
			}

			if (BaseDirectory.Length > 0)
			{
				Directory.SetCurrentDirectory(BaseDirectory);
			}

			Timer.TimerThread ttObj = new Timer.TimerThread();
			timerThread = new Thread(ttObj.TimerMain);
			timerThread.Name = "Timer Thread";

			Version ver = m_Assembly.GetName().Version;

			String publishNumber = "";

			if (File.Exists("publish.txt"))
			{
				try
				{
					FileStream fs = new FileStream("publish.txt", FileMode.Open, FileAccess.Read, FileShare.Read);
					StreamReader sr = new StreamReader(fs);

					publishNumber = sr.ReadLine();

					sr.Close();
					fs.Close();
				}
				catch
				{ }
			}

			// Added to help future code support on forums, as a 'check' people can ask for to it see if they recompiled core or not
			Utility.PushColor(ConsoleColor.DarkGreen);
			Console.WriteLine(@"----------------------------------------------------------------------------");
			Utility.PopColor();
			Utility.PushColor(ConsoleColor.Cyan);
			Console.WriteLine(
				"ServUO - [http://www.servuo.com] Version {0}.{1}, Build {2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
			Console.WriteLine("Publish {0}", publishNumber);
			Utility.PopColor();

			string s = Arguments;

			if (s.Length > 0)
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine("Core: Running with arguments: {0}", s);
				Utility.PopColor();
			}

			m_ProcessorCount = Environment.ProcessorCount;

			if (m_ProcessorCount > 1)
			{
				m_MultiProcessor = true;
			}

			if (m_MultiProcessor || Is64Bit)
			{
				Utility.PushColor(ConsoleColor.Green);
				Console.WriteLine(
					"Core: Optimizing for {0} {2}processor{1}",
					m_ProcessorCount,
					m_ProcessorCount == 1 ? "" : "s",
					Is64Bit ? "64-bit " : "");
				Utility.PopColor();
			}

			int platform = (int)Environment.OSVersion.Platform;
			if (platform == 4 || platform == 128)
			{
				// MS 4, MONO 128
				m_Unix = true;
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine("Core: Unix environment detected");
				Utility.PopColor();
			}
			else
			{
				m_ConsoleEventHandler = OnConsoleEvent;
				UnsafeNativeMethods.SetConsoleCtrlHandler(m_ConsoleEventHandler, true);
			}

			if (GCSettings.IsServerGC)
			{
				Utility.PushColor(ConsoleColor.DarkYellow);
				Console.WriteLine("Core: Server garbage collection mode enabled");
				Utility.PopColor();
			}

			Console.WriteLine("RandomImpl: {0} ({1})", RandomImpl.Type.Name, RandomImpl.IsHardwareRNG ? "Hardware" : "Software");

			_openUOSDK = new OpenUOSDK();

			while (!ScriptCompiler.Compile(m_Debug, m_Cache))
			{
				Utility.PushColor(ConsoleColor.Red);
				Console.WriteLine("Scripts: One or more scripts failed to compile or no script files were found.");
				Utility.PopColor();

				if (m_Service)
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

			MessagePump messagePump = m_MessagePump = new MessagePump();

			timerThread.Start();

			for (int i = 0; i < Map.AllMaps.Count; ++i)
			{
				Map.AllMaps[i].Tiles.Force();
			}

			NetState.Initialize();

			EventSink.InvokeServerStarted();

			try
			{
				long now, last = TickCount;

				const int sampleInterval = 100;
				const float ticksPerSecond = (float)(1000 * sampleInterval);

				long sample = 0;

				while (!m_Closing)
				{
					m_Signal.WaitOne();

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

					if ((++sample % sampleInterval) == 0)
					{
						now = TickCount;
						m_CyclesPerSecond[m_CycleIndex++ % m_CyclesPerSecond.Length] = ticksPerSecond / (now - last);
						last = now;
					}
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

				if (!m_Cache)
				{
					Utility.Separate(sb, "-nocache", " ");
				}

				if (m_HaltOnWarning)
				{
					Utility.Separate(sb, "-haltonwarning", " ");
				}

				if (m_VBdotNET)
				{
					Utility.Separate(sb, "-vb", " ");
				}

				return sb.ToString();
			}
		}

		private static int m_GlobalMaxUpdateRange = 24;

		public static int GlobalMaxUpdateRange { get { return m_GlobalMaxUpdateRange; } set { m_GlobalMaxUpdateRange = value; } }

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

			for (int a = 0; a < ScriptCompiler.Assemblies.Length; ++a)
			{
				VerifySerialization(ScriptCompiler.Assemblies[a]);
			}
		}

		private static readonly Type[] m_SerialTypeArray = new Type[1] {typeof(Serial)};
		private static readonly Type[] m_CustomsSerialTypeArray = new Type[1] {typeof(CustomSerial)};

		private static void VerifyType(Type t)
		{
			bool isItem = t.IsSubclassOf(typeof(Item));

			if (isItem || t.IsSubclassOf(typeof(Mobile)))
			{
				if (isItem)
				{
					// ++m_ItemCount;
					Interlocked.Increment(ref m_ItemCount);
				}
				else
				{
					// ++m_MobileCount;
					Interlocked.Increment(ref m_MobileCount);
				}

				StringBuilder warningSb = null;

				try
				{
					/*
					if( isItem && t.IsPublic && !t.IsAbstract )
					{
					ConstructorInfo cInfo = t.GetConstructor( Type.EmptyTypes );

					if( cInfo == null )
					{
					if (warningSb == null)
					warningSb = new StringBuilder();

					warningSb.AppendLine("       - No zero paramater constructor");
					}
					}*/
					if (t.GetConstructor(m_SerialTypeArray) == null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No serialization constructor");
					}

					if (
						t.GetMethod(
							"Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) ==
						null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Serialize() method");
					}

					if (
						t.GetMethod(
							"Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) ==
						null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Deserialize() method");
					}

					if (warningSb != null && warningSb.Length > 0)
					{
						Console.WriteLine("Warning: {0}\n{1}", t, warningSb);
					}
				}
				catch
				{
					Utility.PushColor(ConsoleColor.Red);
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
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No serialization constructor");
					}

					if (
						t.GetMethod(
							"Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) ==
						null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Serialize() method");
					}

					if (
						t.GetMethod(
							"Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) ==
						null)
					{
						if (warningSb == null)
						{
							warningSb = new StringBuilder();
						}

						warningSb.AppendLine("       - No Deserialize() method");
					}

					if (warningSb != null && warningSb.Length > 0)
					{
						Console.WriteLine("Warning: {0}\n{1}", t, warningSb);
					}
				}
				catch
				{
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine("Warning: Exception in serialization verification of type {0}", t);
					Utility.PopColor();
				}
			}
		}

		private static void VerifySerialization(Assembly a)
		{
			if (a == null)
			{
				return;
			}

			Parallel.ForEach(a.GetTypes(), t => { VerifyType(t); });
		}
	}

	public class FileLogger : TextWriter, IDisposable
	{
		private readonly string m_FileName;
		private bool m_NewLine;
		public const string DateFormat = "[MMMM dd hh:mm:ss.f tt]: ";

		public string FileName { get { return m_FileName; } }

		public FileLogger(string file)
			: this(file, false)
		{ }

		public FileLogger(string file, bool append)
		{
			m_FileName = file;
			using (
				StreamWriter writer =
					new StreamWriter(
						new FileStream(m_FileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				writer.WriteLine(">>>Logging started on {0}.", DateTime.UtcNow.ToString("f"));
				//f = Tuesday, April 10, 2001 3:51 PM 
			}
			m_NewLine = true;
		}

		public override void Write(char ch)
		{
			using (
				StreamWriter writer = new StreamWriter(
					new FileStream(m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
			{
				if (m_NewLine)
				{
					writer.Write(DateTime.UtcNow.ToString(DateFormat));
					m_NewLine = false;
				}
				writer.Write(ch);
			}
		}

		public override void Write(string str)
		{
			using (
				StreamWriter writer = new StreamWriter(
					new FileStream(m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
			{
				if (m_NewLine)
				{
					writer.Write(DateTime.UtcNow.ToString(DateFormat));
					m_NewLine = false;
				}
				writer.Write(str);
			}
		}

		public override void WriteLine(string line)
		{
			using (
				StreamWriter writer = new StreamWriter(
					new FileStream(m_FileName, FileMode.Append, FileAccess.Write, FileShare.Read)))
			{
				if (m_NewLine)
				{
					writer.Write(DateTime.UtcNow.ToString(DateFormat));
				}
				writer.WriteLine(line);
				m_NewLine = true;
			}
		}

		public override Encoding Encoding { get { return Encoding.Default; } }
	}

	public class MultiTextWriter : TextWriter
	{
		private readonly List<TextWriter> m_Streams;

		public MultiTextWriter(params TextWriter[] streams)
		{
			m_Streams = new List<TextWriter>(streams);

			if (m_Streams.Count < 0)
			{
				throw new ArgumentException("You must specify at least one stream.");
			}
		}

		public void Add(TextWriter tw)
		{
			m_Streams.Add(tw);
		}

		public void Remove(TextWriter tw)
		{
			m_Streams.Remove(tw);
		}

		public override void Write(char ch)
		{
			for (int i = 0; i < m_Streams.Count; i++)
			{
				m_Streams[i].Write(ch);
			}
		}

		public override void WriteLine(string line)
		{
			for (int i = 0; i < m_Streams.Count; i++)
			{
				m_Streams[i].WriteLine(line);
			}
		}

		public override void WriteLine(string line, params object[] args)
		{
			WriteLine(String.Format(line, args));
		}

		public override Encoding Encoding { get { return Encoding.Default; } }
	}
}