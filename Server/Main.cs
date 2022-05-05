#region References
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Server.Network;
#endregion

namespace Server
{
	public delegate void Slice();

	public static class Core
	{
		public static Action<CrashedEventArgs> CrashedHandler { get; set; }

		public static bool Crashed { get; private set; }

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
			get => _Profiling;
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

				_ProfileStart = _Profiling ? DateTime.UtcNow : DateTime.MinValue;
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

		public static HashSet<string> DataDirectories { get; } = new HashSet<string>();

		public static Assembly Assembly { get; private set; }

		public static Version Version { get; } = new Version(ServUO.Constants.Assembly.Version);

		public static Process Process { get; private set; }
		public static Thread Thread { get; private set; }

		public static MultiTextWriter MultiConsoleOut { get; private set; }

		private static readonly bool _HighRes = Stopwatch.IsHighResolution;

		private static readonly double _HighFrequency = 1000.0 / Stopwatch.Frequency;
		private const double _LowFrequency = 1000.0 / TimeSpan.TicksPerSecond;

		private static bool _UseHRT;

		public static bool UsingHighResolutionTiming => _UseHRT && _HighRes && !Unix;

		public static long TickCount => (long)Ticks;

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
			foreach (var p in DataDirectories)
			{
				var fullPath = Path.Combine(p, path);

				if (File.Exists(fullPath))
				{
					return fullPath;
				}
			}

			var dataPath = Path.Combine(BaseDirectory, "Data");
			var fileName = Path.GetFileName(path);

			foreach (var file in Directory.EnumerateFiles(dataPath, fileName, SearchOption.AllDirectories))
			{
				if (Insensitive.Equals(Path.GetFileName(path), fileName))
				{
					return file;
				}
			}

			return null;
		}

		public static string FindDataFile(string format, params object[] args)
		{
			return FindDataFile(String.Format(format, args));
		}

		#region Expansions

		private static readonly Expansion[] _Expansions = (Expansion[])Enum.GetValues(typeof(Expansion));

		[ConfigProperty("Server.Expansion")]
		public static Expansion Expansion 
		{ 
			get => Config.GetEnum("Server.Expansion", _Expansions[_Expansions.Length - 1]);
			set
			{
				if (Expansion != value)
				{
					Config.SetEnum("Server.Expansion", value);

					OnExpansionChanged?.Invoke();
				}
			}
		}

		public static bool T2A => Expansion >= Expansion.T2A;
		public static bool UOR => Expansion >= Expansion.UOR;
		public static bool UOTD => Expansion >= Expansion.UOTD;
		public static bool LBR => Expansion >= Expansion.LBR;
		public static bool AOS => Expansion >= Expansion.AOS;
		public static bool SE => Expansion >= Expansion.SE;
		public static bool ML => Expansion >= Expansion.ML;
		public static bool SA => Expansion >= Expansion.SA;
		public static bool HS => Expansion >= Expansion.HS;
		public static bool TOL => Expansion >= Expansion.TOL;
		public static bool EJ => Expansion >= Expansion.EJ;

		[ConfigProperty("Siege.IsSiege")]
		public static bool IsSiege
		{
			get => Config.Get("Siege.IsSiege", false);
			set
			{
				if (IsSiege != value)
				{
					Config.Set("Siege.IsSiege", value); 
					
					OnSiegeStateChanged?.Invoke();
				}
			}
		}

		public static event Action OnSiegeStateChanged;
		public static event Action OnExpansionChanged;

		#endregion

		public static string ExePath => _ExePath ?? (_ExePath = Assembly.Location);

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
				Crashed = true;

				var close = false;

				var args = new CrashedEventArgs(e.ExceptionObject as Exception);

				try
				{
					EventSink.InvokeCrashed(args);
					close = args.Close;
				}
				catch (Exception ex)
				{
					Diagnostics.ExceptionLogging.LogException(ex);
				}

				if (CrashedHandler != null)
				{
					try
					{
						CrashedHandler(args);
						close = args.Close;
					}
					catch (Exception ex)
					{
						Diagnostics.ExceptionLogging.LogException(ex);
					}
				}

				if (!close && !Service)
				{
					try
					{
						foreach (var l in MessagePump.Listeners)
						{
							l.Dispose();
						}
					}
					catch
					{
					}

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

		public static IEnumerable<string> ReadConsoleBuffer()
		{
			return ReadConsoleLines(0, Console.BufferHeight);
		}

		public static string ReadConsoleLine(int index)
		{
			return ReadConsoleLines(index, 1).FirstOrDefault();
		}

		public static IEnumerable<string> ReadConsoleLines(int index, int count)
		{
			return ReadConsole(0, index, Console.BufferWidth, count);
		}

		public static IEnumerable<string> ReadConsole(int x, int y, int width, int height)
		{
			var r = new Rectangle2D(x, y, width, height); // fixes invalid points

			return ConsoleReader.ReadFromBuffer((short)r.X, (short)r.Y, (short)r.Width, (short)r.Height);
		}

		private static class ConsoleReader
		{
			public static IEnumerable<string> ReadFromBuffer(short x, short y, short width, short height)
			{
				var buffer = Marshal.AllocHGlobal(width * height * Marshal.SizeOf(typeof(CHAR_INFO)));

				if (buffer == null)
					throw new OutOfMemoryException();

				try
				{
					var coord = new COORD();

					var rc = new SMALL_RECT
					{
						Left = x,
						Top = y,
						Right = (short)(x + width - 1),
						Bottom = (short)(y + height - 1)
					};

					var size = new COORD
					{
						X = width,
						Y = height
					};

					const int STD_OUTPUT_HANDLE = -11;

					if (!ReadConsoleOutput(GetStdHandle(STD_OUTPUT_HANDLE), buffer, size, coord, ref rc))
						throw new Win32Exception(Marshal.GetLastWin32Error());

					var ptr = buffer;

					var sb = new StringBuilder();

					for (var h = 0; h < height; h++)
					{
						for (var w = 0; w < width; w++)
						{
							var ci = (CHAR_INFO)Marshal.PtrToStructure(ptr, typeof(CHAR_INFO));
							var chars = Console.OutputEncoding.GetChars(ci.charData);

							sb.Append(chars[0]);

							ptr += Marshal.SizeOf(typeof(CHAR_INFO));
						}

						var s = sb.ToString().TrimEnd();

						sb.Clear();

						if (!String.IsNullOrWhiteSpace(s))
							yield return s;
					}
				}
				finally
				{
					Marshal.FreeHGlobal(buffer);
				}
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct CHAR_INFO
			{
				[MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
				public byte[] charData;
				public short attributes;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct COORD
			{
				public short X;
				public short Y;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct SMALL_RECT
			{
				public short Left;
				public short Top;
				public short Right;
				public short Bottom;
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct CONSOLE_SCREEN_BUFFER_INFO
			{
				public COORD dwSize;
				public COORD dwCursorPosition;
				public short wAttributes;
				public SMALL_RECT srWindow;
				public COORD dwMaximumWindowSize;
			}

			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern bool ReadConsoleOutput(IntPtr hConsoleOutput, IntPtr lpBuffer, COORD dwBufferSize, COORD dwBufferCoord, ref SMALL_RECT lpReadRegion);

			[DllImport("kernel32.dll", SetLastError = true)]
			private static extern IntPtr GetStdHandle(int nStdHandle);
		}

		public static bool Closing { get; private set; }

		private static int _CycleIndex = 1;
		private static readonly float[] _CyclesPerSecond = new float[100];

		public static float CyclesPerSecond => _CyclesPerSecond[(_CycleIndex - 1) % _CyclesPerSecond.Length];

		public static float AverageCPS
		{
			get
			{
				var t = 0f;
				var i = _CycleIndex;

				while (--i >= 0)
					t += _CyclesPerSecond[i];

				return t / (_CycleIndex + 1);
			}
		}

		public static void Kill()
		{
			Kill(false);
		}

#if MONO
		private static string[] SupportedTerminals => new string[]
		{
			"xfce4-terminal", "gnome-terminal", "xterm"
		};

		private static void RebootTerminal(int i = 0)
		{
			if(SupportedTerminals.Length > i)
			{
				try {
					if(SupportedTerminals[i] != "xterm")
						Process.Start(SupportedTerminals[i], $"--working-directory={BaseDirectory} -x ./ServUO.sh");
					else
						Process.Start(SupportedTerminals[i], $"-lcc {BaseDirectory} -e ./ServUO.sh");
					Thread.Sleep(500); // a sleep here to not close the programm to quick, so that the new windows cant start.
				}
				catch(System.ComponentModel.Win32Exception)
				{
					RebootTerminal(i+1);
				}
			}
		}
#endif

		public static void Kill(bool restart)
		{
			HandleClosed();

			if (restart)
			{
#if MONO
				RebootTerminal();
				Environment.Exit(0);
			}
#else
				Process.Start(ExePath, Arguments);
			}

			Process.Kill();
#endif
		}

		private static void HandleClosed()
		{
			if (Closing)
			{
				return;
			}

			Closing = true;

			if (Debug)
				Console.Write("Exiting...");

			if (World.Saving)
				World.WaitForWriteCompletion();

			if (!Crashed)
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

		public static void Setup(string[] args)
		{
#if DEBUG
			Debug = true;
#endif

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;

			foreach (var a in args)
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
					Console.WriteLine(AppDomain.CurrentDomain.FriendlyName + " [Parameter]\n\n");
					Console.WriteLine("     -debug              Starting ServUO in Debug Mode. Debug Mode is being used in Core and Scripts to give extended inforamtion during runtime.");
					Console.WriteLine("     -haltonwarning      ServUO halts if any warning is raised during compilation of scripts.");
					Console.WriteLine("     -h or -help         Displays this help text.");
					Console.WriteLine("     -nocache            No known effect.");
					Console.WriteLine("     -noconsole          No user interaction during startup and runtime.");
					Console.WriteLine("     -profile            Enables profiling allowing to get performance diagnostic information of packets, timers etc. in AdminGump -> Maintenance. Use with caution. This increases server load.");
					Console.WriteLine("     -service            This parameter should be set if you're running ServUO as a Windows Service. No user interaction. *Windows only*");
					Console.WriteLine("     -usehrt             Enables High Resolution Timing if requirements are met. Increasing the resolution of the timer. *Windows only*");
					Console.WriteLine("     -vb                 Enables compilation of VB.NET Scripts. Without this option VB.NET Scripts are skipped.");

					Environment.Exit(0);
				}
			}

			if (!Environment.UserInteractive || Service)
			{
				NoConsole = true;
			}

			Thread = Thread.CurrentThread;
			Process = Process.GetCurrentProcess();
			Assembly = Assembly.GetEntryAssembly();

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
			catch (Exception e)
			{
				Diagnostics.ExceptionLogging.LogException(e);
			}

			if (Thread != null)
			{
				Thread.Name = "Core Thread";
			}

			if (BaseDirectory.Length > 0)
			{
				Directory.SetCurrentDirectory(BaseDirectory);
			}

			_TimerThread = new Thread(Timer.TimerThread.TimerMain)
			{
				Name = "Timer Thread"
			};

			Console.Title = $"{ServUO.Constants.Assembly.Title} {Version}";

			Utility.PushColor(Console.ForegroundColor);

			BeginColor(ConsoleColor.Blue);

			Console.WriteLine($"{ServUO.Constants.Assembly.Product} {Version}");
			Console.WriteLine($"{ServUO.Constants.Assembly.Description}");
			Console.WriteLine($"{ServUO.Constants.Assembly.Company} [{ServUO.Constants.Website}]");

			NextColor(ConsoleColor.Yellow);

			var s = Arguments;

			if (s.Length > 0)
			{
				Console.WriteLine($"Core: Running with arguments: {s}");
			}

			ProcessorCount = Environment.ProcessorCount;

			if (ProcessorCount > 1)
			{
				MultiProcessor = true;
			}

			if (MultiProcessor || Is64Bit)
			{
				Console.WriteLine($"Core: Optimizing for {ProcessorCount} {(Is64Bit ? "64-bit " : "")}processor{(ProcessorCount == 1 ? "" : "s")}");
			}

			string dotnet = null;

			if (Type.GetType("Mono.Runtime") != null)
			{
				var displayName = Type.GetType("Mono.Runtime").GetMethod("GetDisplayName", BindingFlags.NonPublic | BindingFlags.Static);

				if (displayName != null)
				{
					dotnet = displayName.Invoke(null, null).ToString();

					Console.WriteLine("Core: Unix environment detected");

					Unix = true;
				}
			}
			else
			{
				m_ConsoleEventHandler = OnConsoleEvent;
				UnsafeNativeMethods.SetConsoleCtrlHandler(m_ConsoleEventHandler, true);
			}

			if (String.IsNullOrWhiteSpace(dotnet))
			{
				dotnet = Environment.Version.ToString();
			}

			Console.WriteLine($"Core: {(Unix ? $"MONO .NET {dotnet}" : $".NET {dotnet}")}");

			if (GCSettings.IsServerGC)
			{
				Console.WriteLine("Core: Server garbage collection mode enabled");
			}

			if (_UseHRT)
			{
				Console.WriteLine($"Core: High resolution timing {(UsingHighResolutionTiming ? "supported" : "unsupported")}");
			}

			Console.WriteLine($"Core: RNG using {RandomImpl.Type.Name} ({(RandomImpl.IsHardwareRNG ? "Hardware" : "Software")})");

			NextColor(ConsoleColor.DarkYellow);

			Console.WriteLine("Core: Preparing...");
			Console.WriteLine();

			var sw = Stopwatch.StartNew();

			Config.Load();

			InitDataDirectories();

			while (!ScriptCompiler.Compile(Debug, _Cache))
			{
				sw.Stop();

				NextColor(ConsoleColor.Red);

				Console.WriteLine("Core: Assembly load failed");

				if (Service)
				{
					break;
				}

				Console.WriteLine("Core: - Press return to exit, or R to try again");

				if (Console.ReadKey(true).Key != ConsoleKey.R)
				{
					break;
				}

				sw.Start();
			}

			sw.Stop();

			Console.WriteLine();
			Console.WriteLine($"Core: Preparation took {sw.Elapsed.TotalSeconds:F1} seconds");

			EndColor();

			Console.WriteLine();
			Console.WriteLine("Core: Configuring...");
			Console.WriteLine();

			sw.Restart();
			ScriptCompiler.Invoke("Configure");
			sw.Stop();

			Console.WriteLine();
			Console.WriteLine($"Core: Configuration took {sw.Elapsed.TotalSeconds:F1} seconds");
			Console.WriteLine();

			Region.Load();
			World.Load();

			Console.WriteLine();
			Console.WriteLine("Core: Initializing...");
			Console.WriteLine();

			sw.Restart();
			ScriptCompiler.Invoke("Initialize");
			sw.Stop();

			Console.WriteLine();
			Console.WriteLine($"Core: Initialization took {sw.Elapsed.TotalSeconds:F1} seconds");
			Console.WriteLine();

			sw.Reset();

			DisplayDataDirectories();

			MessagePump = new MessagePump();

			foreach (var m in Map.AllMaps)
			{
				m.Tiles.Force();
			}

			NetState.Initialize();
		}

		[ConfigProperty("Server.DataPath")]
		public static string DataDirectory 
		{
			get => Config.Get("Server.DataPath", DataDirectories.FirstOrDefault()); 
			set
			{
				var old = DataDirectory;

				if (old != value)
				{
					if (value == null)
					{
						DataDirectories.Remove(old);

						Config.Set("Server.DataPath", value);
					}
					else if (!String.IsNullOrWhiteSpace(value) && File.Exists(Path.Combine(value, "client.exe")))
					{
						DataDirectories.Remove(old);

						Config.Set("Server.DataPath", value);

						DataDirectories.Add(value);
					}
				}
			}
		}

		private static void InitDataDirectories()
		{
			var path = DataDirectory;

			if (!String.IsNullOrWhiteSpace(path) && File.Exists(Path.Combine(path, "client.exe")))
			{
				DataDirectories.Add(path);
			}

			while (DataDirectories.Count == 0 && !Service)
			{
				Utility.WriteLine(ConsoleColor.DarkYellow, "Core: Enter a path to Ultima Online:");

				path = Console.ReadLine();

				if (!String.IsNullOrWhiteSpace(path) && File.Exists(Path.Combine(path, "client.exe")))
				{
					if (DataDirectories.Add(path))
					{
						Config.Set("Server.DataPath", path);
						Config.Save("Server");

						Utility.WriteLine(ConsoleColor.DarkYellow, "Core: Ultima Online path has been updated...");
					}

					return;
				}

				Utility.WriteLine(ConsoleColor.Red, "Core: Invalid path...");
			}
		}

		public static void DisplayDataDirectories()
		{
			Console.WriteLine();
			Utility.WriteLine(ConsoleColor.DarkYellow, $"Core: Data Paths: {String.Join("\n > ", DataDirectories)}");
			Console.WriteLine();
		}

		private static void BeginColor(ConsoleColor color)
		{
			Console.BackgroundColor = color;
			Console.Write(new string(' ', Console.BufferWidth));
			Console.BackgroundColor = ConsoleColor.Black;
			Console.WriteLine();

			Utility.PushColor(color);
		}

		private static void NextColor(ConsoleColor color)
		{
			EndColor();
			BeginColor(color);
		}

		private static void EndColor()
		{
			Console.WriteLine();
			Console.BackgroundColor = Console.ForegroundColor;
			Console.Write(new string(' ', Console.BufferWidth));
			Console.BackgroundColor = ConsoleColor.Black;

			Utility.PopColor();
		}

		public static void Run()
		{
			EventSink.InvokeServerStarted();

			_TimerThread.Start();

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

					MessagePump.Slice();

					NetState.FlushAll();
					NetState.ProcessDisposedQueue();

					Sector.Slice();

					Slice?.Invoke();

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
				var sb = new StringBuilder();

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

		public static int GlobalUpdateRange { get; set; } = 18;
		public static int GlobalMaxUpdateRange { get; set; } = 24;
		public static int GlobalRadarRange { get; set; } = 40;

		private static int m_ItemCount, m_MobileCount;

		public static int ScriptItems => m_ItemCount;
		public static int ScriptMobiles => m_MobileCount;

		public static void VerifySerialization()
		{
			m_ItemCount = 0;
			m_MobileCount = 0;

			VerifySerialization(Assembly.GetCallingAssembly());

			foreach (var a in ScriptCompiler.Assemblies)
			{
				VerifySerialization(a);
			}
		}

		private static readonly Type[] m_SerialTypeArray = { typeof(Serial) };

		private static readonly StringBuilder m_TypeWarning = new StringBuilder();

		private static void VerifyType(Type t)
		{
			var warningSb = m_TypeWarning;

			try
			{
				var isItem = t.IsSubclassOf(typeof(Item));

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

					try
					{
						if (t.GetConstructor(m_SerialTypeArray) == null)
						{
							warningSb.AppendLine("       - No serialization constructor");
						}

						if (t.GetMethod("Serialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
						{
							warningSb.AppendLine("       - No Serialize() method");
						}

						if (t.GetMethod("Deserialize", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly) == null)
						{
							warningSb.AppendLine("       - No Deserialize() method");
						}

						if (warningSb.Length > 0)
							Utility.WriteLine(ConsoleColor.Yellow, $"Warning: {t}\n{warningSb}");
					}
					catch
					{
						Utility.WriteLine(ConsoleColor.Yellow, $"Warning: Exception in serialization verification of type {t}");
					}
				}
			}
			finally
			{
				warningSb.Clear();
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

			using (var writer = new StreamWriter(new FileStream(FileName, append ? FileMode.Append : FileMode.Create, FileAccess.Write, FileShare.Read)))
			{
				writer.WriteLine(">>>Logging started on {0:f}.", DateTime.Now);
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

		public override Encoding Encoding => Encoding.Default;
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

		public override Encoding Encoding => Encoding.Default;
	}
}
