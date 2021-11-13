#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
#endregion

namespace Server
{
	public static class Config
	{
		public sealed class Entry : IEquatable<Entry>, IComparable<Entry>
		{
			private readonly string m_InitialValue;
			private readonly bool m_InitialDefault;

			private int? m_Hash;

			public int FileIndex { get; private set; }

			public string File { get; private set; }
			public string Scope { get; private set; }

			public string Desc { get; set; }

			public string Key { get; private set; }
			public string Value { get; private set; }

			public object Object { get; private set; }

			public bool UseDefault { get; set; }

			public Entry(string file, int fileIndex, string scope, string desc, string key, string value, object state, bool useDefault)
			{
				m_InitialValue = value;
				m_InitialDefault = useDefault;

				File = file;
				FileIndex = fileIndex;

				Scope = scope;
				Desc = desc;

				Key = key;
				Value = value;
				Object = state;

				UseDefault = useDefault;
			}

			public void Set(string value, object state)
			{
				var oldValue = Value;
				var oldObject = Object;

				Value = value;
				Object = state;

				if (m_InitialValue == value)
				{
					UseDefault = m_InitialDefault;
				}
				else if (oldValue != value)
				{
					UseDefault = false;
				}

				OnEntryChanged?.Invoke(this, oldValue, oldObject);
			}

			public override string ToString()
			{
				return String.Format("{0}.{1}{2}={3}", Scope, UseDefault ? "@" : "", Key, Value);
			}

			public override int GetHashCode()
			{
				if (m_Hash != null)
				{
					return m_Hash.Value;
				}

				unchecked
				{
					var hash = -1;

					hash = (hash * 397) ^ File.GetHashCode();
					hash = (hash * 397) ^ Scope.GetHashCode();
					hash = (hash * 397) ^ Key.GetHashCode();

					m_Hash = hash;

					return hash;
				}
			}

			public override bool Equals(object obj)
			{
				return obj is Entry e && Equals(e);
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

				return Insensitive.Equals(File, other.File) && Insensitive.Equals(Scope, other.Scope) && Insensitive.Equals(Key, other.Key);
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

		private static readonly HashSet<string> _Warned = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

		private static readonly Dictionary<string, Entry> _Entries = new Dictionary<string, Entry>(StringComparer.InvariantCultureIgnoreCase);

		public static IEnumerable<Entry> Entries => _Entries.Values;

		public static event Action<Entry, string, object> OnEntryChanged;

		public static Entry Find(string key)
		{
			Load();

			_Entries.TryGetValue(key, out var e);

			return e;
		}

		public static void Dump(string scope)
		{
			Utility.PushColor(ConsoleColor.DarkYellow);
			Dump(Console.Out, scope);
			Utility.PopColor();
		}

		public static void Dump(TextWriter output, string scope)
		{
			Load();

			if (_Entries.Count > 0)
			{
				var count = 0;

				foreach (var e in _Entries.Values)
				{
					if (String.IsNullOrWhiteSpace(scope) || Insensitive.Equals(e.Scope, scope))
					{
						if (++count == 1)
						{
							output.WriteLine();
						}

						output.WriteLine(e);
					}
				}

				if (count > 0)
				{
					output.WriteLine();
				}
			}
		}

		public static void Unload()
		{
			_Initialized = false;

			_Warned.Clear();
			_Entries.Clear();
		}

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
				Utility.WriteLine(ConsoleColor.Yellow, "Config: No configuration files found!");
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
					Utility.WriteLine(ConsoleColor.Red, "Config: Failed to load configuration file:");
					Utility.WriteLine(ConsoleColor.Gray, $"Config: {path}");
					Utility.WriteLine(ConsoleColor.Red, $"Config: {e.Message}");

					ConsoleKey key;

					do
					{
						Utility.WriteLine(ConsoleColor.Gray, "Ignore this warning? (y/n)");

						key = Console.ReadKey(true).Key;
					}
					while (key != ConsoleKey.Y && key != ConsoleKey.N);

					if (key != ConsoleKey.Y)
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

		public static bool Load(string scope)
		{
			if (!Directory.Exists(_Path))
			{
				Directory.CreateDirectory(_Path);
			}

			var success = false;

			var path = Path.Combine(_Path, $"{scope}.cfg");

			try
			{
				LoadFile(path);

				success = true;
			}
			catch (Exception e)
			{
				Utility.WriteLine(ConsoleColor.Red, "Config: Failed to load configuration file:");
				Utility.WriteLine(ConsoleColor.Gray, $"Config: {path}");
				Utility.WriteLine(ConsoleColor.Red, $"Config: {e.Message}");
			}

			return success;
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
					throw new FormatException($"Bad format at line {i + 1}");
				}

				var key = line.Substring(0, io);
				var val = line.Substring(io + 1);

				if (String.IsNullOrWhiteSpace(key))
				{
					throw new NullReferenceException($"Key can not be null at line {i + 1}");
				}

				key = key.Trim();

				if (String.IsNullOrEmpty(val))
				{
					val = null;
				}

				var e = new Entry(info.FullName, idx++, scope, String.Join("\n", desc), key, val, null, useDef);

				_Entries[$"{e.Scope}.{e.Key}"] = e;

				desc.Clear();
			}
		}

		public static void Save()
		{
			Load();

			if (!Directory.Exists(_Path))
			{
				Directory.CreateDirectory(_Path);
			}

			foreach (var g in _Entries.Values.GroupBy(e => e.File))
			{
				try
				{
					SaveFile(g.Key, g.OrderBy(e => e.FileIndex));
				}
				catch (Exception e)
				{
					Utility.WriteLine(ConsoleColor.Red, "Config: Failed to save configuration file:");
					Utility.WriteLine(ConsoleColor.Gray, $"Config: {g.Key}");
					Utility.WriteLine(ConsoleColor.Red, $"Config: {e.Message}");
				}
			}
		}

		public static void Save(string scope)
		{
			if (!_Entries.Values.Any(e => Insensitive.Equals(e.Scope, scope)))
			{
				Load(scope);
			}

			if (!Directory.Exists(_Path))
			{
				Directory.CreateDirectory(_Path);
			}

			foreach (var g in _Entries.Values.Where(e => Insensitive.Equals(e.Scope, scope)).GroupBy(e => e.File))
			{
				try
				{
					SaveFile(g.Key, g.OrderBy(e => e.FileIndex));
				}
				catch (Exception e)
				{
					Utility.WriteLine(ConsoleColor.Red, "Config: Failed to save configuration file:");
					Utility.WriteLine(ConsoleColor.Gray, $"Config: {g.Key}");
					Utility.WriteLine(ConsoleColor.Red, $"Config: {e.Message}");
				}
			}
		}

		private static void SaveFile(string path, IEnumerable<Entry> entries)
		{
			var content = new StringBuilder(0x1000);

			foreach (var e in entries)
			{
				content.AppendLine();

				if (!String.IsNullOrWhiteSpace(e.Desc))
				{
					var split = e.Desc.Split('\n');

					if (split.Length > 0)
					{
						foreach (var line in split)
						{
							content.AppendLine($"# {line}");
						}
					}
					else
					{
						content.AppendLine($"# {e.Desc}");
					}
				}

				content.AppendLine($"{(e.UseDefault ? "@" : String.Empty)}{e.Key}={e.Value}");
			}

			var dir = Path.GetDirectoryName(path);

			if (!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}

			File.WriteAllText(path, content.ToString());
		}

		private static string GetFilePath(ref string key, out string scope)
		{
			var parts = key.Split('.');

			key = parts[parts.Length - 1];

			Array.Resize(ref parts, parts.Length - 1);

			scope = String.Join(".", parts);

			if (_Gen)
			{
				return Path.Combine(_Path, "_GEN", Path.Combine(parts) + ".cfg");
			}

			return Path.Combine(_Path, Path.Combine(parts) + ".cfg");
		}

		#region Setters

		private static void InternalSet(string key, string value, object state)
		{
			_Warned.Remove(key);

			if (_Entries.TryGetValue(key, out var e) && e != null)
			{
				e.Set(value, state);
				return;
			}

			var path = GetFilePath(ref key, out var scope);

			var idx = _Entries.Values.Where(o => Insensitive.Equals(o.File, path)).Select(o => o.FileIndex).DefaultIfEmpty().Max();

			_Entries[key] = new Entry(path, idx, scope, String.Empty, key, value, state, false);
		}

		public static void Set(string key, string value)
		{
			InternalSet(key, value, value);
		}

		public static void Set(string key, char value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, sbyte value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, byte value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, short value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, ushort value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, int value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, uint value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, long value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, ulong value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, float value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, double value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, decimal value)
		{
			InternalSet(key, value.ToString(_NumFormatter), value);
		}

		public static void Set(string key, bool value)
		{
			InternalSet(key, value ? Boolean.TrueString : Boolean.FalseString, value);
		}

		public static void Set(string key, TimeSpan value)
		{
			InternalSet(key, value.ToString(), value);
		}

		public static void Set(string key, DateTime value)
		{
			InternalSet(key, value.ToString(CultureInfo.InvariantCulture), value);
		}

		public static void Set(string key, IPAddress value)
		{
			InternalSet(key, value.ToString(), value);
		}

		public static void Set(string key, Version value)
		{
			InternalSet(key, value.ToString(), value);
		}

		public static void Set(string key, ClientVersion value)
		{
			InternalSet(key, value.ToString(), value);
		}

		public static void SetEnum<T>(string key, T value) where T : Enum
		{
			var vals = (T[])Enum.GetValues(typeof(T));

			foreach (var o in vals.Where(o => o.Equals(value)))
			{
				InternalSet(key, o.ToString(), o);
				return;
			}

			throw new ArgumentException("Enumerated value not found");
		}

		public static void SetDelegate<T>(string key, T value) where T : Delegate
		{
			InternalSet(key, $"{value.Method.DeclaringType.FullName}.{value.Method.Name}", value);
		}

		public static void SetArray<T>(string key, T[] value)
		{
			if (value == null)
			{
				InternalSet(key, String.Empty, value);
			}
			else if (value.Length == 0)
			{
				InternalSet(key, "[]", value);
			}
			else
			{
				var encoded = new StringBuilder();

				for (var i = 0; i < value.Length; i++)
				{
					var val = value[i];

					if (i > 0)
					{
						encoded.Append(", ");
					}

					var str = val.ToString();

					encoded.Append('"');

					foreach (var c in str)
					{
						switch (c)
						{
							case '"': encoded.Append("\\\""); break;
							case '\\': encoded.Append("\\\\"); break;
							case '\b': encoded.Append("\\b"); break;
							case '\f': encoded.Append("\\f"); break;
							case '\n': encoded.Append("\\n"); break;
							case '\r': encoded.Append("\\r"); break;
							case '\t': encoded.Append("\\t"); break;
							default:
								{
									var sur = Convert.ToInt32(c);

									if (sur >= 32 && sur <= 126)
									{
										encoded.Append(c);
									}
									else
									{
										encoded.Append("\\u" + Convert.ToString(sur, 16).PadLeft(4, '0'));
									}
								}
								break;
						}
					}

					encoded.Append('"');
				}

				InternalSet(key, $"[{encoded}]", value);
			}
		}

		#endregion

		#region Getters

		private static bool InternalGet(string key, out Entry entry)
		{
			Load();

			if (!_Gen && Core.Debug && _Entries.TryGetValue($"_DEBUG.{key}", out entry) && entry != null)
			{
				return true;
			}

			if (_Entries.TryGetValue(key, out entry) && entry != null)
			{
				return true;
			}

			return false;
		}

		private static T InternalGet<T>(string key, T def, Parser<T> parser)
		{
			if (InternalGet(key, out var entry) && !entry.UseDefault)
			{
				if (entry.Object is T val)
				{
					return val;
				}

				if (entry.Value != null)
				{
					if (parser(entry.Value, out val))
					{
						entry.Set(entry.Value, val);

						return val;
					}

					if (_Warned.Add(key))
					{
						Utility.WriteLine(ConsoleColor.Yellow, $"Config: Invalid value '{entry.Value}' for '{key}'");
					}
				}
			}

			return def;
		}

		public static string Get(string key, string defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static sbyte Get(string key, sbyte defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static byte Get(string key, byte defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static short Get(string key, short defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static ushort Get(string key, ushort defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static int Get(string key, int defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static uint Get(string key, uint defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static long Get(string key, long defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static ulong Get(string key, ulong defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static float Get(string key, float defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static double Get(string key, double defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static decimal Get(string key, decimal defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static bool Get(string key, bool defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static TimeSpan Get(string key, TimeSpan defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static DateTime Get(string key, DateTime defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static Type Get(string key, Type defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static IPAddress Get(string key, IPAddress defaultValue)
		{
			return Utility.Intern(InternalGet(key, defaultValue, TryParse));
		}

		public static Version Get(string key, Version defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static ClientVersion Get(string key, ClientVersion defaultValue)
		{
			return InternalGet(key, defaultValue, TryParse);
		}

		public static T GetEnum<T>(string key, T defaultValue) where T : struct, Enum
		{
			return InternalGet(key, defaultValue, TryParseEnum);
		}

		public static T GetDelegate<T>(string key, T defaultValue) where T : Delegate
		{
			return InternalGet(key, defaultValue, TryParseDelegate);
		}

		public static T[] GetArray<T>(string key, T[] defaultValue)
		{
			return InternalGet(key, defaultValue, TryParseArray);
		}

		public static T[] GetArray<T>(string key, bool defaultEmpty)
		{
			return GetArray(key, defaultEmpty ? Array.Empty<T>() : null);
		}

		#endregion

		#region Parsers

		private delegate bool Parser<T>(string input, out T val);

		private static bool TryParse(string input, out string value)
		{
			return (value = input) != null;
		}

		private static bool TryParse(string input, out sbyte value)
		{
			return SByte.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out byte value)
		{
			return Byte.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out short value)
		{
			return Int16.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out ushort value)
		{
			return UInt16.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out int value)
		{
			return Int32.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out uint value)
		{
			return UInt32.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out long value)
		{
			return Int64.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out ulong value)
		{
			return UInt64.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out float value)
		{
			return Single.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out double value)
		{
			return Double.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out decimal value)
		{
			return Decimal.TryParse(input, NumberStyles.Any, _NumFormatter, out value);
		}

		private static bool TryParse(string input, out bool value)
		{
			if (Boolean.TryParse(input, out value))
			{
				return true;
			}

			if (Regex.IsMatch(input, @"(true|yes|on|1|enabled)", RegexOptions.IgnoreCase))
			{
				value = true;
				return true;
			}

			if (Regex.IsMatch(input, @"(false|no|off|0|disabled)", RegexOptions.IgnoreCase))
			{
				value = false;
				return true;
			}

			value = false;
			return false;
		}

		private static bool TryParse(string input, out TimeSpan value)
		{
			return TimeSpan.TryParse(input, out value);
		}

		private static bool TryParse(string input, out DateTime value)
		{
			return DateTime.TryParse(input, out value);
		}

		private static bool TryParse(string input, out Type value)
		{
			var type = Type.GetType(input, false);

			if (type != null)
			{
				return (value = type) != null;
			}

			if (input.IndexOf('.') < 0)
			{
				return (value = ScriptCompiler.FindTypeByName(input)) != null;
			}

			return (value = ScriptCompiler.FindTypeByFullName(input)) != null;
		}

		private static bool TryParse(string input, out IPAddress value)
		{
			return IPAddress.TryParse(input, out value);
		}

		private static bool TryParse(string input, out Version value)
		{
			return Version.TryParse(input, out value);
		}

		private static bool TryParse(string input, out ClientVersion value)
		{
			return ClientVersion.TryParse(input, out value);
		}

		private static bool TryParseEnum<T>(string input, out T value) where T : struct, Enum
		{
			return Enum.TryParse(input, out value);
		}

		private static bool TryParseDelegate<T>(string input, out T value) where T : Delegate
		{
			var i = input.LastIndexOf('.');

			if (i > 0)
			{
				try
				{
					if (TryParse(input.Remove(i), out Type target) && target != null)
					{
						var info = target.GetMethod(input.Substring(i + 1), (BindingFlags)0x38);

						if (info != null && Delegate.CreateDelegate(typeof(T), info) is T d)
						{
							value = d;
							return true;
						}
					}
				}
				catch
				{ }
			}

			value = null;
			return false;
		}

		private static bool TryParseArray<T>(string input, out T[] value)
		{
			if (!input.StartsWith("[") || !input.EndsWith("]"))
			{
				value = null;
				return false;
			}

			input = input.Trim('[', ']');

			if (typeof(T) == typeof(string))
			{
				var output = new List<string>();

				var build = new StringBuilder(0x20);

				var idx = 0;

				while (idx < input.Length)
				{
					idx = input.IndexOf('"', idx);

					if (idx < 0)
					{
						break;
					}

					var dump = false;

					var o = input[idx++];

					while (idx < input.Length)
					{
						var c = input[idx++];

						if (c == o)
						{
							dump = true;
							break;
						}

						if (c == '\\')
						{
							if (idx == input.Length)
							{
								dump = true;
								break;
							}

							c = input[idx++];

							var found = true;

							switch (c)
							{
								case '"': build.Append('"'); break;
								case '\\': build.Append('\\'); break;
								case '/': build.Append('/'); break;
								case 'b': build.Append('\b'); break;
								case 'f': build.Append('\f'); break;
								case 'n': build.Append('\n'); break;
								case 'r': build.Append('\r'); break;
								case 't': build.Append('\t'); break;
								default: found = false; break;
							}

							if (!found && c == 'u')
							{
								var length = input.Length - idx;

								if (length < 4)
								{
									value = null;
									return false;
								}

								if (!UInt32.TryParse(input.Substring(idx, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var sur))
								{
									value = null;
									return false;
								}

								build.Append(Char.ConvertFromUtf32((int)sur));

								idx += 4;
							}
						}
						else
						{
							build.Append(c);
						}
					}

					if (dump)
					{
						output.Add(build.ToString());
					}

					build.Clear();
				}

				value = output.Cast<T>().ToArray();

				ColUtility.Free(output);

				return true;
			}

			var vals = input.Split(',');

			value = new T[vals.Length];

			for (var i = 0; i < vals.Length; i++)
			{
				if (!GenericTryParse(vals[i], out T obj))
				{
					value = null;
					return false;
				}

				value[i] = obj;
			}

			return true;
		}

		private static readonly ThreadLocal<Type[]> _GenericParserParams = new ThreadLocal<Type[]>(() => new Type[] { typeof(string) });
		private static readonly ThreadLocal<Type[]> _GenericTryParserParams = new ThreadLocal<Type[]>(() => new Type[] { typeof(string), null });

		private static readonly ThreadLocal<object[]> _GenericParserArgs = new ThreadLocal<object[]>(() => new object[] { null });
		private static readonly ThreadLocal<object[]> _GenericTryParserArgs = new ThreadLocal<object[]>(() => new object[] { null, null });

		private static bool GenericTryParse<T>(string input, out T value)
		{
			try
			{
				var type = typeof(T);

				if (type == typeof(string))
				{
					if (input == null)
					{
						value = default(T);
					}
					else if (input.Length == 0)
					{
						value = (T)(object)String.Empty;
					}
					else
					{
						value = (T)(object)input.Trim().Trim('"');
					}

					return true;
				}

				if (String.IsNullOrEmpty(input))
				{
					value = default(T);
					return false;
				}

				Type[] types;
				object[] args;

				types = _GenericTryParserParams.Value;
				args = _GenericTryParserArgs.Value;

				try
				{
					var search = type.IsEnum ? "TryParseEnum" : type.IsAssignableFrom(typeof(Delegate)) ? "TryParseDelegate" : "TryParse";

					types[1] = type;

					args[0] = input;
					args[1] = null;

					var tryParse = typeof(Config).GetMethod(search, types);

					if (tryParse != null && (bool)tryParse.Invoke(null, args))
					{
						value = (T)args[1];
						return true;
					}
				}
				catch
				{ }
				finally
				{
					types[1] = null;

					args[0] = null;
					args[1] = null;
				}

				try
				{
					var search = "TryParse";

					types[1] = type;

					args[0] = input;
					args[1] = null;

					var tryParse = type.GetMethod(search, types);

					if (tryParse != null && (bool)tryParse.Invoke(null, args))
					{
						value = (T)args[1];
						return true;
					}
				}
				catch
				{ }
				finally
				{
					types[1] = null;

					args[0] = null;
					args[1] = null;
				}

				types = _GenericParserParams.Value;
				args = _GenericParserArgs.Value;

				try
				{
					var search = "Parse";

					args[0] = input.Trim();

					var parse = type.GetMethod(search, types);

					if (parse != null)
					{
						value = (T)parse.Invoke(null, args);
						return true;
					}
				}
				catch
				{ }
				finally
				{
					args[0] = null;
				}
			}
			catch
			{ }

			value = default(T);
			return false;
		}

		#endregion

		#region Generation

		private static readonly char[] _InvalidChars = Path.GetInvalidPathChars();

		private static bool _Gen;

		public static void Generate()
		{
			Load();

			var files = new HashSet<string>();

			var supportedTypes = new Dictionary<Type, MethodInfo>();

			_Gen = true;

			try
			{
				Utility.WriteLine(ConsoleColor.Yellow, "Config: Generating files...");

				foreach (var m in typeof(Config).GetMethods(BindingFlags.Public | BindingFlags.Static))
				{
					if (!m.Name.StartsWith("Set"))
					{
						continue;
					}

					var p = m.GetParameters();

					if (p.Length != 2)
					{
						continue;
					}

					var keyArg = p[0];

					if (keyArg.ParameterType != typeof(string))
					{
						continue;
					}

					var valArg = p[1];

					if (valArg.ParameterType.IsGenericParameter)
					{
						var a = valArg.ParameterType.GetGenericParameterConstraints();

						if (a != null && a.Length > 0)
						{
							supportedTypes[a[0]] = m;
						}
					}
					else
					{
						supportedTypes[valArg.ParameterType] = m;
					}
				}

				foreach (var asm in ScriptCompiler.Assemblies)
				{
					var cache = ScriptCompiler.GetTypeCache(asm);

					foreach (var type in cache.Types)
					{
						if (type == typeof(Config) || type == typeof(Entry))
						{
							continue;
						}

						foreach (var field in type.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
						{
							if (field.IsLiteral)
							{
								continue;
							}

							if (!supportedTypes.TryGetValue(field.FieldType, out var setter))
							{
								continue;
							}

							var key = $"{type.Name}.{field.Name}";

							if (key.IndexOfAny(_InvalidChars) >= 0)
							{
								continue;
							}

							if (!_Entries.TryGetValue(key, out var entry) || entry == null)
							{
								var val = field.GetValue(null);

								if (val != null)
								{
									setter.Invoke(null, new object[] { key, val });
								}
								else
								{
									InternalSet(key, null, null);
								}

								files.Add(GetFilePath(ref key, out _));
							}
						}

						foreach (var prop in type.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
						{
							if (!prop.CanRead)
							{
								continue;
							}

							if (!supportedTypes.TryGetValue(prop.PropertyType, out var setter))
							{
								continue;
							}

							var key = $"{type.Name}.{prop.Name}";

							if (key.IndexOfAny(_InvalidChars) >= 0)
							{
								continue;
							}

							if (!_Entries.TryGetValue(key, out var entry) || entry == null)
							{
								var val = prop.GetValue(null);

								if (val != null)
								{
									setter.Invoke(null, new object[] { key, val });
								}
								else
								{
									InternalSet(key, null, null);
								}

								files.Add(GetFilePath(ref key, out _));
							}
						}
					}
				}

				var count = 0;

				foreach (var group in _Entries.Values.GroupBy(e => e.File))
				{
					if (files.Contains(group.Key))
					{
						foreach (var entry in group)
						{
							if (String.IsNullOrWhiteSpace(entry.Desc))
							{
								entry.Desc = String.Empty;
							}

							if (entry.Desc.Length > 0)
							{
								entry.Desc += "\n";
							}

							entry.Desc += $"Default: {entry.Value}";

							if (Core.Debug)
							{
								Utility.WriteLine(ConsoleColor.Gray, $"Config: {entry}");
							}
						}

						SaveFile(group.Key, group);

						++count;
					}
				}

				Utility.WriteLine(ConsoleColor.Yellow, $"Config: Generated {count} new files.");
			}
			catch (Exception e)
			{
				Utility.WriteLine(ConsoleColor.Red, $"Config: Generation failed:\n{e}");
			}
			finally
			{
				files.Clear();
				supportedTypes.Clear();

				_Gen = false;
			}
		}

		#endregion
	}
}
