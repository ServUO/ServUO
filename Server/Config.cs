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
#endregion

namespace Server
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class ConfigPropertyAttribute : Attribute
    {
        public string Scope { get; }

        public ConfigPropertyAttribute(string scope)
        {
            Scope = scope;
        }
    }

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
                if (m_InitialValue == value)
                {
                    UseDefault = m_InitialDefault;
                }
                else if (Value != value)
                {
                    UseDefault = state == null;
                }

                var oldValue = Value;
                var oldObject = Object;

                Value = value;
                Object = state;

                OnEntryChanged?.Invoke(this, oldValue, oldObject);
            }

            public override string ToString()
            {
                if (UseDefault)
                {
                    return $"{Scope}.@{Key}={Value}";
                }

                return $"{Scope}.{Key}={Value}";
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
                if (other is null)
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

        private static readonly HashSet<string> _Warned = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

        private static readonly Dictionary<string, Entry> _Entries = new Dictionary<string, Entry>(StringComparer.InvariantCultureIgnoreCase);

        public static IEnumerable<Entry> Entries => _Entries.Values;

        public static IFormatProvider NumFormatter => CultureInfo.InvariantCulture.NumberFormat;

        public static event Action<Entry, string, object> OnEntryChanged;

        public static Entry Find(string key)
        {
            Load();

            _ = _Entries.TryGetValue(key, out var e);

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
                    if (string.IsNullOrWhiteSpace(scope) || Insensitive.Equals(e.Scope, scope))
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

            Console.WriteLine("Config: Loading...");

            _ = Directory.CreateDirectory(_Path);

            IEnumerable<string> files;

            try
            {
                files = Directory.EnumerateFiles(_Path, "*.cfg", SearchOption.AllDirectories);
            }
            catch (DirectoryNotFoundException)
            {
                Utility.PushColor(ConsoleColor.Yellow);
                Console.WriteLine("Config: Files Not Found");
                Utility.PopColor();

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
                    var relativePath = path.Replace(Path.GetDirectoryName(_Path), string.Empty);

                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"Config: Load Failed [{e.Message}]");
                    Utility.PopColor();
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.WriteLine(relativePath);
                    Utility.PopColor();

                    Console.WriteLine("Ignore and continue? (Y/N)");

                    var answer = Console.ReadKey(true);

                    if (answer.Key == ConsoleKey.N)
                    {
                        Console.WriteLine("Press any key to exit...");

                        _ = Console.ReadKey(true);

                        Core.Kill(false);

                        return;
                    }
                }
            }

            if (Core.Debug)
            {
                Utility.PushColor(ConsoleColor.Blue);

                foreach (var e in _Entries.Values)
                {
                    Console.WriteLine(e);
                }

                Utility.PopColor();
            }
        }

        public static bool Load(string scope)
        {
            _ = Directory.CreateDirectory(_Path);

            var success = false;

            var path = Path.Combine(_Path, $"{scope}.cfg");

            try
            {
                LoadFile(path);

                success = true;
            }
            catch (Exception e)
            {
                var relativePath = path.Replace(Path.GetDirectoryName(_Path), string.Empty);

                Utility.PushColor(ConsoleColor.Red);
                Console.WriteLine($"Config: Load Failed [{e.Message}]");
                Utility.PopColor();
                Utility.PushColor(ConsoleColor.Gray);
                Console.WriteLine(relativePath, ConsoleColor.Gray);
                Utility.PopColor();
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

            path = info.Directory != null ? info.Directory.FullName : string.Empty;

            var io = path.IndexOf(_Path, StringComparison.OrdinalIgnoreCase);

            if (io > -1)
            {
                path = path.Substring(io + _Path.Length);
            }

            var parts = path.Split(Path.DirectorySeparatorChar);

            var scope = string.Join(".", parts.Where(p => !string.IsNullOrWhiteSpace(p)));

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

                if (string.IsNullOrWhiteSpace(line))
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

                if (string.IsNullOrWhiteSpace(key))
                {
                    throw new NullReferenceException($"Key can not be null at line {i + 1}");
                }

                key = key.Trim();

                if (string.IsNullOrEmpty(val))
                {
                    val = null;
                }

                var e = new Entry(info.FullName, idx++, scope, string.Join("\n", desc), key, val, null, useDef);

                _Entries[$"{e.Scope}.{e.Key}"] = e;

                desc.Clear();
            }
        }

        public static void Save()
        {
            Load();

            _ = Directory.CreateDirectory(_Path);

            foreach (var g in _Entries.Values.GroupBy(e => e.File))
            {
                try
                {
                    SaveFile(g.Key, g.OrderBy(e => e.FileIndex));
                }
                catch (Exception e)
                {
                    var relativePath = g.Key.Replace(Path.GetDirectoryName(_Path), string.Empty);

                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"Config: Save Failed [{e.Message}]");
                    Utility.PopColor();
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.WriteLine(relativePath, ConsoleColor.Gray);
                    Utility.PopColor();
                }
            }
        }

        public static void Save(string scope)
        {
            if (!_Entries.Values.Any(e => Insensitive.Equals(e.Scope, scope)))
            {
                _ = Load(scope);
            }

            _ = Directory.CreateDirectory(_Path);

            foreach (var g in _Entries.Values.Where(e => Insensitive.Equals(e.Scope, scope)).GroupBy(e => e.File))
            {
                try
                {
                    SaveFile(g.Key, g.OrderBy(e => e.FileIndex));
                }
                catch (Exception e)
                {
                    var relativePath = g.Key.Replace(Path.GetDirectoryName(_Path), string.Empty);

                    Utility.PushColor(ConsoleColor.Red);
                    Console.WriteLine($"Config: Save Failed [{e.Message}]");
                    Utility.PopColor();
                    Utility.PushColor(ConsoleColor.Gray);
                    Console.WriteLine(relativePath);
                    Utility.PopColor();
                }
            }
        }

        private static void SaveFile(string path, IEnumerable<Entry> entries)
        {
            var content = new StringBuilder(0x1000);

            foreach (var e in entries)
            {
                _ = content.AppendLine();

                if (!string.IsNullOrWhiteSpace(e.Desc))
                {
                    var split = e.Desc.Split('\n');

                    if (split.Length > 0)
                    {
                        foreach (var line in split)
                        {
                            _ = content.AppendLine($"# {line}");
                        }
                    }
                    else
                    {
                        _ = content.AppendLine($"# {e.Desc}");
                    }
                }

                _ = content.AppendLine($"{(e.UseDefault ? "@" : string.Empty)}{e.Key}={e.Value}");
            }

            var dir = Path.GetDirectoryName(path);

            _ = Directory.CreateDirectory(dir);

            File.WriteAllText(path, content.ToString());
        }

        private static string GetFilePath(ref string key, out string scope)
        {
            var parts = key.Split('.');

            key = parts[parts.Length - 1];

            Array.Resize(ref parts, parts.Length - 1);

            scope = string.Join(".", parts);

            if (_Gen)
            {
                return Path.Combine(_Path, "_GEN", Path.Combine(parts) + ".cfg");
            }

            return Path.Combine(_Path, Path.Combine(parts) + ".cfg");
        }

        #region Setters

        private static void InternalSet<T>(string key, Stringifier<T> stringify, T state)
        {
            _ = _Warned.Remove(key);

            var value = stringify(state);

            if (_Entries.TryGetValue(key, out var e) && e != null)
            {
                e.Set(value, state);
                return;
            }

            var path = GetFilePath(ref key, out var scope);

            var idx = _Entries.Values.Where(o => Insensitive.Equals(o.File, path)).Select(o => o.FileIndex).DefaultIfEmpty().Max();

            _Entries[key] = new Entry(path, idx, scope, string.Empty, key, value, state, false);
        }

        public static void Set<T>(string key, T value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, string value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, char value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, sbyte value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, byte value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, short value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, ushort value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, int value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, uint value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, long value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, ulong value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, float value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, double value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, decimal value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, bool value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, TimeSpan value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, DateTime value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, IPAddress value)
        {
            InternalSet(key, ToString, value);
        }

        public static void Set(string key, Version value)
        {
            InternalSet(key, ToString, value);
        }

        public static void SetEnum<T>(string key, T value) where T : Enum
        {
            InternalSet(key, ToEnumString, value);
        }

        public static void SetDelegate<T>(string key, T value) where T : Delegate
        {
            InternalSet(key, ToDelegateString, value);
        }

        public static void SetArray<T>(string key, T[] value)
        {
            InternalSet(key, ToArrayString, value);
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
                        Utility.PushColor(ConsoleColor.Yellow);
                        Console.WriteLine($"Config: Invalid Value [{key}, '{entry.Value}']");
                        Utility.PopColor();
                    }
                }
            }

            return def;
        }

        public static T Get<T>(string key, T defaultValue)
        {
            return InternalGet(key, defaultValue, TryParse);
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

        #region Stringifiers

        private delegate string Stringifier<T>(T value);

        private static readonly Dictionary<Type, MethodInfo> _DetectedStringifiers = new Dictionary<Type, MethodInfo>();

        public static string ToString<T>(T value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var type = typeof(T);

            if (type.IsArray)
            {
                try
                {
                    if (!_DetectedStringifiers.TryGetValue(typeof(Array), out var stringify))
                    {
                        _DetectedStringifiers[typeof(Array)] = stringify = typeof(Config).GetMethod("ToArrayString", new[] { type });
                    }

                    if (stringify != null)
                    {
                        return (string)stringify.Invoke(null, new object[] { value });
                    }
                }
                catch
                { }

                return string.Empty;
            }

            if (type.IsEnum)
            {
                try
                {
                    if (!_DetectedStringifiers.TryGetValue(typeof(Enum), out var stringify))
                    {
                        _DetectedStringifiers[typeof(Enum)] = stringify = typeof(Config).GetMethod("ToEnumString", new[] { type });
                    }

                    if (stringify != null)
                    {
                        return (string)stringify.Invoke(null, new object[] { value });
                    }
                }
                catch
                { }

                return string.Empty;
            }

            if (type.IsAssignableFrom(typeof(Delegate)))
            {
                try
                {
                    if (!_DetectedStringifiers.TryGetValue(typeof(Delegate), out var stringify))
                    {
                        _DetectedStringifiers[typeof(Delegate)] = stringify = typeof(Config).GetMethod("ToDelegateString", new[] { type });
                    }

                    if (stringify != null)
                    {
                        return (string)stringify.Invoke(null, new object[] { value });
                    }
                }
                catch
                { }

                return string.Empty;
            }

            try
            {
                if (!_DetectedStringifiers.TryGetValue(type, out var stringify))
                {
                    _DetectedStringifiers[type] = stringify = typeof(Config).GetMethod("ToString", new[] { type });
                }

                if (stringify != null)
                {
                    return (string)stringify.Invoke(null, new object[] { value });
                }

                return value.ToString();
            }
            catch
            { }

            return string.Empty;
        }

        public static string ToString(string value)
        {
            return value ?? string.Empty;
        }

        public static string ToString(char value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(sbyte value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(byte value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(short value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(ushort value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(int value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(uint value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(long value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(ulong value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(float value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(double value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(decimal value)
        {
            return value.ToString(NumFormatter);
        }

        public static string ToString(bool value)
        {
            return value ? bool.TrueString : bool.FalseString;
        }

        public static string ToString(TimeSpan value)
        {
            return value.ToString();
        }

        public static string ToString(DateTime value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ToString(IPAddress value)
        {
            return value.ToString();
        }

        public static string ToString(Version value)
        {
            return value.ToString();
        }

        public static string ToEnumString<T>(T value) where T : Enum
        {
            return value.ToString();
        }

        public static string ToDelegateString<T>(T value) where T : Delegate
        {
            return $"{value.Method.DeclaringType.FullName}.{value.Method.Name}";
        }

        public static string ToArrayString<T>(T[] value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            if (value.Length == 0)
            {
                return "[]";
            }

            var type = typeof(T);

            var isString = type == typeof(string);

            var encoded = new StringBuilder();

            _ = encoded.Append('[');

            for (var i = 0; i < value.Length; i++)
            {
                var val = value[i];

                if (i > 0)
                {
                    _ = encoded.Append(", ");
                }

                if (val != null)
                {
                    var str = ToString(val);

                    var isSubArray = val?.GetType()?.IsArray == true;

                    if (isSubArray)
                    {
                        _ = encoded.Append('\t');
                    }

                    var isSubString = isString || val?.GetType() == typeof(string);

                    if (isSubString)
                    {
                        _ = encoded.Append('"');
                    }

                    foreach (var c in str)
                    {
                        switch (c)
                        {
                            case '"':
                            _ = encoded.Append("\\\"");
                            break;
                            case '\\':
                            _ = encoded.Append("\\\\");
                            break;
                            case '\b':
                            _ = encoded.Append("\\b");
                            break;
                            case '\f':
                            _ = encoded.Append("\\f");
                            break;
                            case '\n':
                            _ = encoded.Append("\\n");
                            break;
                            case '\r':
                            _ = encoded.Append("\\r");
                            break;
                            case '\t':
                            _ = encoded.Append("\\t");
                            break;
                            default:
                            {
                                var sur = Convert.ToInt32(c);

                                if (sur >= 32 && sur <= 126)
                                {
                                    _ = encoded.Append(c);
                                }
                                else
                                {
                                    _ = encoded.Append("\\u" + Convert.ToString(sur, 16).PadLeft(4, '0'));
                                }
                            }

                            break;
                        }
                    }

                    if (isSubString)
                    {
                        _ = encoded.Append('"');
                    }

                    if (isSubArray)
                    {
                        _ = encoded.Append(Environment.NewLine);
                    }
                }
            }

            _ = encoded.Append(']');

            var result = encoded.ToString();

            _ = encoded.Clear();

            return result;
        }

        #region Args

        public static string ToArgsString(params object[] args)
        {
            return ToArrayString(args);
        }

        #endregion

        #endregion

        #region Parsers

        private delegate bool Parser<T>(string input, out T value);

        private static readonly Dictionary<Type, MethodInfo> _DetectedParsers = new Dictionary<Type, MethodInfo>();
        private static readonly Dictionary<Type, MethodInfo> _DetectedTryParsers = new Dictionary<Type, MethodInfo>();

        public static bool TryParse<T>(string input, out T value)
        {
            value = default;

            try
            {
                var type = typeof(T);

                if (type == typeof(string))
                {
                    if (input == null)
                    {
                        value = default;
                    }
                    else if (input.Length == 0)
                    {
                        value = (T)(object)string.Empty;
                    }
                    else
                    {
                        value = (T)(object)input.Trim().Trim('"');
                    }

                    return true;
                }

                if (string.IsNullOrEmpty(input))
                {
                    value = default;
                    return false;
                }

                if (type.IsArray)
                {
                    try
                    {
                        if (!_DetectedTryParsers.TryGetValue(typeof(Array), out var tryParse))
                        {
                            _DetectedTryParsers[typeof(Array)] = tryParse = typeof(Config).GetMethod("TryParseArray", new[] { typeof(string), type.MakeByRefType() });
                        }

                        if (tryParse != null)
                        {
                            var args = new object[] { input.Trim(), default(T) };

                            if ((bool)tryParse.Invoke(null, args))
                            {
                                value = (T)args[1];
                                return true;
                            }
                        }
                    }
                    catch
                    { }

                    return false;
                }

                if (type.IsEnum)
                {
                    try
                    {
                        if (!_DetectedTryParsers.TryGetValue(typeof(Enum), out var tryParse))
                        {
                            _DetectedTryParsers[typeof(Enum)] = tryParse = typeof(Config).GetMethod("TryParseEnum", new[] { typeof(string), type.MakeByRefType() });
                        }

                        if (tryParse != null)
                        {
                            var args = new object[] { input.Trim(), default(T) };

                            if ((bool)tryParse.Invoke(null, args))
                            {
                                value = (T)args[1];
                                return true;
                            }
                        }
                    }
                    catch
                    { }

                    return false;
                }

                if (type.IsAssignableFrom(typeof(Delegate)))
                {
                    try
                    {
                        if (!_DetectedTryParsers.TryGetValue(typeof(Delegate), out var tryParse))
                        {
                            _DetectedTryParsers[typeof(Delegate)] = tryParse = typeof(Config).GetMethod("TryParseDelegate", new[] { typeof(string), type.MakeByRefType() });
                        }

                        if (tryParse != null)
                        {
                            var args = new object[] { input.Trim(), value };

                            if ((bool)tryParse.Invoke(null, args))
                            {
                                value = (T)args[1];
                                return true;
                            }
                        }
                    }
                    catch
                    { }

                    return false;
                }

                try
                {
                    if (!_DetectedTryParsers.TryGetValue(type, out var tryParse))
                    {
                        _DetectedTryParsers[type] = tryParse = typeof(Config).GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
                    }

                    var args = new object[] { input.Trim(), value };

                    if (tryParse != null && (bool)tryParse.Invoke(null, args))
                    {
                        value = (T)args[1];
                        return true;
                    }
                }
                catch
                { }

                if (type.IsDefined(typeof(ParsableAttribute)))
                {
                    try
                    {
                        if (!_DetectedParsers.TryGetValue(type, out var parse))
                        {
                            _DetectedParsers[type] = parse = type.GetMethod("Parse", new[] { typeof(string) });
                        }

                        if (parse != null)
                        {
                            value = (T)parse.Invoke(null, new object[] { input.Trim() });
                            return true;
                        }
                    }
                    catch
                    { }
                }
                else
                {
                    try
                    {
                        if (!_DetectedTryParsers.TryGetValue(type, out var tryParse))
                        {
                            _DetectedTryParsers[type] = tryParse = type.GetMethod("TryParse", new[] { typeof(string), type.MakeByRefType() });
                        }

                        var args = new object[] { input.Trim(), value };

                        if (tryParse != null && (bool)tryParse.Invoke(null, args))
                        {
                            value = (T)args[1];
                            return true;
                        }
                    }
                    catch
                    { }
                }
            }
            catch
            { }

            return false;
        }

        public static bool TryParse(string input, out string value)
        {
            return (value = input) != null;
        }

        public static bool TryParse(string input, out sbyte value)
        {
            return sbyte.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out byte value)
        {
            return byte.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, NumFormatter, out value);
        }

        public static bool TryParse(string input, out short value)
        {
            return short.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out ushort value)
        {
            return ushort.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, NumFormatter, out value);
        }

        public static bool TryParse(string input, out int value)
        {
            return int.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out uint value)
        {
            return uint.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, NumFormatter, out value);
        }

        public static bool TryParse(string input, out long value)
        {
            return long.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out ulong value)
        {
            return ulong.TryParse(input, NumberStyles.Any & ~NumberStyles.AllowLeadingSign, NumFormatter, out value);
        }

        public static bool TryParse(string input, out float value)
        {
            return float.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out double value)
        {
            return double.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out decimal value)
        {
            return decimal.TryParse(input, NumberStyles.Any, NumFormatter, out value);
        }

        public static bool TryParse(string input, out bool value)
        {
            if (bool.TryParse(input, out value))
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

        public static bool TryParse(string input, out TimeSpan value)
        {
            return TimeSpan.TryParse(input, out value);
        }

        public static bool TryParse(string input, out DateTime value)
        {
            return DateTime.TryParse(input, out value);
        }

        public static bool TryParse(string input, out Type value)
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

        public static bool TryParse(string input, out IPAddress value)
        {
            if (IPAddress.TryParse(input, out value))
            {
                Utility.Intern(ref value);

                return true;
            }

            return false;
        }

        public static bool TryParse(string input, out Version value)
        {
            return Version.TryParse(input, out value);
        }

        public static bool TryParseEnum<T>(string input, out T value) where T : struct, Enum
        {
            return Enum.TryParse(input, out value);
        }

        public static bool TryParseDelegate<T>(string input, out T value) where T : Delegate
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

        #region Args

        public static bool TryParseArgs<T1, T2>(string input, out (T1, T2) value)
        {
            value = default;

            if (TryParse(input, out object[] values) && values.Length >= 2)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index]);

                return true;
            }

            return false;
        }

        public static bool TryParseArgs<T1, T2, T3>(string input, out (T1, T2, T3) value)
        {
            value = default;

            if (TryParseArray(input, out object[] values) && values.Length >= 3)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index], (T3)values[++index]);

                return true;
            }

            return false;
        }

        public static bool TryParseArgs<T1, T2, T3, T4>(string input, out (T1, T2, T3, T4) value)
        {
            value = default;

            if (TryParseArray(input, out object[] values) && values.Length >= 4)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index], (T3)values[++index], (T4)values[++index]);

                return true;
            }

            return false;
        }

        public static bool TryParseArgs<T1, T2, T3, T4, T5>(string input, out (T1, T2, T3, T4, T5) value)
        {
            value = default;

            if (TryParseArray(input, out object[] values) && values.Length >= 5)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index], (T3)values[++index], (T4)values[++index], (T5)values[++index]);

                return true;
            }

            return false;
        }

        public static bool TryParseArgs<T1, T2, T3, T4, T5, T6>(string input, out (T1, T2, T3, T4, T5, T6) value)
        {
            value = default;

            if (TryParseArray(input, out object[] values) && values.Length >= 6)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index], (T3)values[++index], (T4)values[++index], (T5)values[++index], (T6)values[++index]);

                return true;
            }

            return false;
        }

        public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7>(string input, out (T1, T2, T3, T4, T5, T6, T7) value)
        {
            value = default;

            if (TryParseArray(input, out object[] values) && values.Length >= 7)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index], (T3)values[++index], (T4)values[++index], (T5)values[++index], (T6)values[++index], (T7)values[++index]);

                return true;
            }

            return false;
        }

        public static bool TryParseArgs<T1, T2, T3, T4, T5, T6, T7, T8>(string input, out (T1, T2, T3, T4, T5, T6, T7, T8) value)
        {
            value = default;

            if (TryParseArray(input, out object[] values) && values.Length >= 8)
            {
                var index = -1;

                value = ((T1)values[++index], (T2)values[++index], (T3)values[++index], (T4)values[++index], (T5)values[++index], (T6)values[++index], (T7)values[++index], (T8)values[++index]);

                return true;
            }

            return false;
        }

        #endregion

        #region Arrays

        public static bool TryParse(string input, out object[] value)
        {
            return TryParseArray(input, out value);
        }

        public static bool TryParse(string input, out string[] value)
        {
            return TryParseArray(input, out value);
        }

        private static bool TryParseArray<T>(string input, out T[] value)
        {
            try
            {
                if (!input.StartsWith("[") || !input.EndsWith("]"))
                {
                    value = null;
                    return false;
                }

                input = input.Substring(1, input.Length - 2);

                var isGeneric = typeof(T) == typeof(object);
                var isString = typeof(T) == typeof(string);

                if (isGeneric || isString)
                {
                    var output = new List<string>();

                    var build = new StringBuilder(0x20);

                    var idx = 0;

                    while (idx < input.Length)
                    {
                        var oidx = idx;

                        idx = input.IndexOf('"', idx);

                        var inString = false;

                        if (idx >= 0)
                        {
                            inString = true;
                        }
                        else if (isGeneric)
                        {
                            idx = oidx;
                        }
                        else
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

                            if (inString && c == '\\')
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
                                    case '"':
                                    _ = build.Append('"');
                                    break;
                                    case '\\':
                                    _ = build.Append('\\');
                                    break;
                                    case '/':
                                    _ = build.Append('/');
                                    break;
                                    case 'b':
                                    _ = build.Append('\b');
                                    break;
                                    case 'f':
                                    _ = build.Append('\f');
                                    break;
                                    case 'n':
                                    _ = build.Append('\n');
                                    break;
                                    case 'r':
                                    _ = build.Append('\r');
                                    break;
                                    case 't':
                                    _ = build.Append('\t');
                                    break;
                                    default:
                                    found = false;
                                    break;
                                }

                                if (!found && c == 'u')
                                {
                                    var length = input.Length - idx;

                                    if (length < 4)
                                    {
                                        value = null;
                                        return false;
                                    }

                                    if (!uint.TryParse(input.Substring(idx, 4), NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var sur))
                                    {
                                        value = null;
                                        return false;
                                    }

                                    _ = build.Append(char.ConvertFromUtf32((int)sur));

                                    idx += 4;
                                }
                            }
                            else if (inString && c == '"')
                            {
                                dump = true;
                            }
                            else if (isGeneric && (char.IsWhiteSpace(c) || c == ','))
                            {
                                dump = true;
                            }
                            else
                            {
                                _ = build.Append(c);
                            }
                        }

                        if (dump)
                        {
                            output.Add(build.ToString());
                        }

                        _ = build.Clear();
                    }

                    value = new T[output.Count];

                    for (var i = 0; i < output.Count; i++)
                    {
                        if (!TryParse(output[i], out T obj))
                        {
                            value = null;
                            return false;
                        }

                        value[i] = obj;
                    }

                    ColUtility.Free(output);

                    return true;
                }

                var vals = input.Split(',');

                value = new T[vals.Length];

                for (var i = 0; i < vals.Length; i++)
                {
                    if (!TryParse(vals[i], out T obj))
                    {
                        value = null;
                        return false;
                    }

                    value[i] = obj;
                }

                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        #endregion

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
                Console.WriteLine($"Config: Generating...");

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
                                    _ = setter.Invoke(null, new object[] { key, val });
                                }
                                else
                                {
                                    InternalSet<object>(key, ToString, null);
                                }

                                _ = files.Add(GetFilePath(ref key, out _));
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
                                    _ = setter.Invoke(null, new object[] { key, val });
                                }
                                else
                                {
                                    InternalSet<object>(key, ToString, null);
                                }

                                _ = files.Add(GetFilePath(ref key, out _));
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
                            if (string.IsNullOrWhiteSpace(entry.Desc))
                            {
                                entry.Desc = string.Empty;
                            }

                            if (entry.Desc.Length > 0)
                            {
                                entry.Desc += "\n";
                            }

                            entry.Desc += $"Default: {entry.Value}";

                            if (Core.Debug)
                            {
                                Utility.PushColor(ConsoleColor.Blue);
                                Console.WriteLine(entry);
                                Utility.PopColor();
                            }
                        }

                        SaveFile(group.Key, group);

                        ++count;
                    }
                }

                Console.WriteLine($"Config: Generated {count:N0} new files.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Config: Generation Failed");

                Console.WriteLine(e);
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
