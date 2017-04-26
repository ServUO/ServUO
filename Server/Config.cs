#region Header
// **********
// ServUO - Config.cs
// **********
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
#endregion

namespace Server
{
	public static class Config
	{
		public sealed class Entry : IEquatable<Entry>, IComparable<Entry>
		{
			public int FileIndex { get; private set; }

			public string File { get; private set; }
			public string Scope { get; private set; }

			public string Desc { get; set; }

			public string Key { get; set; }
			public string Value { get; set; }

			public bool UseDefault { get; set; }

			public Entry(string file, int fileIndex, string scope, string desc, string key, string value, bool useDefault)
			{
				File = file;
				FileIndex = fileIndex;

				Scope = scope;
				Desc = desc;

				Key = key;
				Value = value;

				UseDefault = useDefault;
			}

			public override string ToString()
			{
				return String.Format("{0}.{1}{2}={3}", Scope, UseDefault ? "@" : "", Key, Value);
			}

			public override int GetHashCode()
			{
				unchecked
				{
					var hash = -1;

					hash = (hash * 397) ^ File.GetHashCode();
					hash = (hash * 397) ^ Scope.GetHashCode();
					hash = (hash * 397) ^ Key.GetHashCode();

					return hash;
				}
			}

			public override bool Equals(object obj)
			{
				return obj is Entry && Equals((Entry)obj);
			}

			public bool Equals(Entry other)
			{
				if (ReferenceEquals(other, null))
				{
					return false;
				}

				if (ReferenceEquals(other, this))
				{
					return true;
				}

				return Insensitive.Equals(File, other.File) && //
					   Insensitive.Equals(Scope, other.Scope) && //
					   Insensitive.Equals(Key, other.Key);
			}

			public int CompareTo(Entry other)
			{
				if (other == null)
				{
					return -1;
				}

				if (!Insensitive.Equals(File, other.File))
				{
					return Insensitive.Compare(File, other.File);
				}

				return FileIndex.CompareTo(other.FileIndex);
			}
		}

		private static bool _Initialized;

		private static readonly string _Path = Path.Combine(Core.BaseDirectory, "Config");

		private static readonly IFormatProvider _NumFormatter = CultureInfo.InvariantCulture.NumberFormat;

		private static readonly Dictionary<string, Entry> _Entries =
			new Dictionary<string, Entry>(StringComparer.OrdinalIgnoreCase);

		public static IEnumerable<Entry> Entries { get { return _Entries.Values; } }

		public static void Load()
		{
			if (_Initialized)
			{
				return;
			}

			_Initialized = true;

			if (!Directory.Exists(_Path))
			{
				Directory.CreateDirectory(_Path);
			}

			IEnumerable<string> files;

			try
			{
				files = Directory.EnumerateFiles(_Path, "*.cfg", SearchOption.AllDirectories);
			}
			catch (DirectoryNotFoundException)
			{
				Console.WriteLine("Warning: No configuration files found!");
				return;
			}

			foreach (var path in files)
			{
				try
				{
					LoadFile(path);
				}
				catch (Exception e)
				{
					Console.WriteLine("Warning: Failed to load configuration file:");
					Console.WriteLine(path);
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine(e.Message);
					Utility.PopColor();

					string key;

                    do
                    {
                        Console.WriteLine("Ignore this warning? (y/n)");
                        key = Console.ReadLine().ToUpper();
                    }
                    while (!key.Equals("Y") && !key.Equals("N"));
                    
					if (!key.Equals("Y"))
					{
						
							Console.WriteLine("Press any key to exit...");
							Console.ReadKey();

							Core.Kill(false);
						
							return;
					}
				}
			}

			if (Core.Debug)
			{
				Console.WriteLine();

				foreach (var e in _Entries.Values)
				{
					Console.WriteLine(e);
				}

				Console.WriteLine();
			}
		}

		private static void LoadFile(string path)
		{
			var info = new FileInfo(path);

			if (!info.Exists)
			{
				throw new FileNotFoundException();
			}

			path = info.Directory != null ? info.Directory.FullName : String.Empty;

			var io = path.IndexOf(_Path, StringComparison.OrdinalIgnoreCase);

			if (io > -1)
			{
				path = path.Substring(io + _Path.Length);
			}

			var parts = path.Split(Path.DirectorySeparatorChar);

			var scope = String.Join(".", parts.Where(p => !String.IsNullOrWhiteSpace(p)));
			
			if (scope.Length > 0)
			{
				scope += ".";
			}

			scope += Path.GetFileNameWithoutExtension(info.Name);

			var lines = File.ReadAllLines(info.FullName);

			var desc = new List<string>(0x10);

			for (int i = 0, idx = 0; i < lines.Length; i++)
			{
				var line = lines[i].Trim();

				if (String.IsNullOrWhiteSpace(line))
				{
					desc.Clear();
					continue;
				}

				if (line.StartsWith("#"))
				{
					desc.Add(line.TrimStart('#').Trim());
					continue;
				}

				var useDef = false;

				if (line.StartsWith("@"))
				{
					useDef = true;
					line = line.TrimStart('@').Trim();
				}

				io = line.IndexOf('=');
				
				if (io < 0)
				{
					throw new FormatException(String.Format("Bad format at line {0}", i + 1));
				}

				var key = line.Substring(0, io);
				var val = line.Substring(io + 1);

				if (String.IsNullOrWhiteSpace(key))
				{
					throw new NullReferenceException(String.Format("Key can not be null at line {0}", i + 1));
				}

				key = key.Trim();

				if (String.IsNullOrEmpty(val))
				{
					val = null;
				}

				var e = new Entry(info.FullName, idx++, scope, String.Join(String.Empty, desc), key, val, useDef);

				_Entries[String.Format("{0}.{1}", e.Scope, e.Key)] = e;

				desc.Clear();
			}
		}

		public static void Save()
		{
			if (!_Initialized)
			{
				Load();
			}
			
			if (!Directory.Exists(_Path))
			{
				Directory.CreateDirectory(_Path);
			}

			foreach (var g in _Entries.Values.ToLookup(e => e.File))
			{
				try
				{
					SaveFile(g.Key, g.OrderBy(e => e.FileIndex));
				}
				catch (Exception e)
				{
					Console.WriteLine("Warning: Failed to save configuration file:");
					Console.WriteLine(g.Key);
					Utility.PushColor(ConsoleColor.Red);
					Console.WriteLine(e.Message);
					Utility.PopColor();
				}
			}
		}

		private static void SaveFile(string path, IEnumerable<Entry> entries)
		{
			var content = new StringBuilder(0x1000);
			var line = new StringBuilder(0x80);

			foreach (var e in entries)
			{
				content.AppendLine();

				if (!String.IsNullOrWhiteSpace(e.Desc))
				{
					line.Clear();

					foreach (var word in e.Desc.Split(' '))
					{
						if ((line + word).Length > 100)
						{
							content.AppendLine(String.Format("# {0}", line));
							line.Clear();
						}

						line.AppendFormat("{0} ", word);
					}

					if (line.Length > 0)
					{
						content.AppendLine(String.Format("# {0}", line));
						line.Clear();
					}
				}

				content.AppendLine(String.Format("{0}{1}={2}", e.UseDefault ? "@" : String.Empty, e.Key, e.Value));
			}

			File.WriteAllText(path, content.ToString());
		}

		public static Entry Find(string key)
		{
			Entry e;
			_Entries.TryGetValue(key, out e);
			return e;
		}

		private static void InternalSet(string key, string value)
		{
			var e = Find(key);

			if (e != null)
			{
				e.Value = value;
				e.UseDefault = false;
				return;
			}

			var parts = key.Split('.');
			var realKey = parts.Last();

			parts = parts.Take(parts.Length - 1).ToArray();

			var file = new FileInfo(Path.Combine(_Path, Path.Combine(parts) + ".cfg"));
			var idx = _Entries.Values.Where(o => Insensitive.Equals(o.File, file.FullName)).Select(o => o.FileIndex).DefaultIfEmpty().Max();

			_Entries[key] = new Entry(file.FullName, idx, String.Join(".", parts), String.Empty, realKey, value, false);
		}

		public static void Set(string key, string value)
		{
			InternalSet(key, value);
		}

		public static void Set(string key, char value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, sbyte value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, byte value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, short value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, ushort value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, int value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, uint value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, long value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, ulong value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, float value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, double value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, decimal value)
		{
			InternalSet(key, value.ToString(_NumFormatter));
		}

		public static void Set(string key, bool value)
		{
			InternalSet(key, value ? "true" : "false");
		}

		public static void Set(string key, TimeSpan value)
		{
			InternalSet(key, value.ToString());
		}

		public static void Set(string key, DateTime value)
		{
			InternalSet(key, value.ToString(CultureInfo.InvariantCulture));
		}

		public static void SetEnum<T>(string key, T value) where T : struct, IConvertible
		{
			var t = typeof(T);

			if (!t.IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			var vals = Enum.GetValues(t).Cast<T>();

			foreach (T o in vals.Where(o => o.Equals(value)))
			{
				InternalSet(key, o.ToString(CultureInfo.InvariantCulture));
				return;
			}

			throw new ArgumentException("Enumerated value not found");
		}

		private static void Warn<T>(string key)
		{
			Utility.PushColor(ConsoleColor.Yellow);
			Console.WriteLine("Config: Warning, '{0}' invalid value for '{1}'", typeof(T), key);
			Utility.PopColor();
		}

		private static string InternalGet(string key)
		{
			if (!_Initialized)
			{
				Load();
			}

			Entry e;

			if (_Entries.TryGetValue(key, out e) && e != null)
			{
				return e.UseDefault ? null : e.Value;
			}

			Utility.PushColor(ConsoleColor.Yellow);
			Console.WriteLine("Config: Warning, using default value for {0}", key);
			Utility.PopColor();

			return null;
		}

		public static string Get(string key, string defaultValue)
		{
			return InternalGet(key) ?? defaultValue;
		}

		public static sbyte Get(string key, sbyte defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || sbyte.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<sbyte>(key);

			return defaultValue;
		}

		public static byte Get(string key, byte defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || byte.TryParse(value, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<byte>(key);

			return defaultValue;
		}

		public static short Get(string key, short defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || short.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<short>(key);

			return defaultValue;
		}

		public static ushort Get(string key, ushort defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || ushort.TryParse(value, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<ushort>(key);

			return defaultValue;
		}

		public static int Get(string key, int defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || int.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<int>(key);

			return defaultValue;
		}

		public static uint Get(string key, uint defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || uint.TryParse(value, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<uint>(key);

			return defaultValue;
		}

		public static long Get(string key, long defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || long.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<long>(key);

			return defaultValue;
		}

		public static ulong Get(string key, ulong defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || ulong.TryParse(value, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<ulong>(key);

			return defaultValue;
		}

		public static float Get(string key, float defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || float.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<float>(key);

			return defaultValue;
		}

		public static double Get(string key, double defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || double.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<double>(key);

			return defaultValue;
		}

		public static decimal Get(string key, decimal defaultValue)
		{
			var ret = defaultValue;
			var value = InternalGet(key);

			if (value == null || decimal.TryParse(value, NumberStyles.Any, _NumFormatter, out ret))
			{
				return ret;
			}

			Warn<decimal>(key);

			return defaultValue;
		}

		public static bool Get(string key, bool defaultValue)
		{
			var value = InternalGet(key);
			
			if (value == null)
			{
				return defaultValue;
			}

			if (Regex.IsMatch(value, @"(true|yes|on|1|enabled)", RegexOptions.IgnoreCase))
			{
				return true;
			}

			if (Regex.IsMatch(value, @"(false|no|off|0|disabled)", RegexOptions.IgnoreCase))
			{
				return false;
			}

			Warn<bool>(key);

			return defaultValue;
		}

		public static TimeSpan Get(string key, TimeSpan defaultValue)
		{
			var value = InternalGet(key);

			TimeSpan ts;

			if (TimeSpan.TryParse(value, out ts))
			{
				return ts;
			}

			Warn<TimeSpan>(key);

			return defaultValue;
		}

		public static DateTime Get(string key, DateTime defaultValue)
		{
			var value = InternalGet(key);

			DateTime dt;

			if (DateTime.TryParse(value, out dt))
			{
				return dt;
			}

			Warn<DateTime>(key);

			return defaultValue;
		}

		public static T GetEnum<T>(string key, T defaultValue) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("T must be an enumerated type");
			}

			var value = InternalGet(key);

			if (value == null)
			{
				return defaultValue;
			}

			value = value.Trim();

			var vals = Enum.GetValues(typeof(T)).Cast<T>();

			foreach (var o in vals.Where(o => Insensitive.Equals(value, o.ToString(CultureInfo.InvariantCulture))))
			{
				return o;
			}

			Warn<T>(key);

			return defaultValue;
		}
	}
}