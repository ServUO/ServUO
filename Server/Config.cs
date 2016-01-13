using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Server
{
	public static class Config
	{
		private static string m_Path = "Config";
		private static char[] m_ValueSeperators = { '=' };
		private static Dictionary<string, string> values;

		public static void Load()
		{
			values = new Dictionary<string, string>();

			IEnumerable<string> files = null;
			try
			{
				files = Directory.EnumerateFiles(m_Path, "*.cfg", SearchOption.AllDirectories);
			}
			catch (DirectoryNotFoundException)
			{
				Console.WriteLine("Warning: configuration directory not found!");
				return;
			}

			foreach (string path in files)
			{
				LoadFile(path);
			}
		}

		private static void LoadFile(string path)
		{
			string[] parts = path.Split(Path.DirectorySeparatorChar);
			string parent = "";
			for (int i = 1; i < parts.Length - 1; ++i)
			{
				if (parent.Length > 0)
				{
					parent += ".";
				}
				parent += parts[i];
			}
			if (parent.Length > 0)
			{
				parent += ".";
			}
			parent += Path.GetFileNameWithoutExtension(path);

			StreamReader reader = new StreamReader(path);
			string line;
			int lineNumber = 0;
			while ((line = reader.ReadLine()) != null)
			{
				++lineNumber;
				line = line.Trim();
				if (line.Length <= 0 ||
					line.StartsWith("#"))
				{
					continue;
				}

				string[] tokens = line.Split(m_ValueSeperators, 2);
				if (tokens.Length < 2)
				{
					Console.WriteLine(String.Format("Config: {0}:{1} Improperly formated line", path, lineNumber));
					continue;
				}

				string key = tokens[0].Trim();
				string value = tokens[1].Trim();

				Set(String.Format("{0}.{1}", parent, key), value);
			}
		}

		public static void Set(string key, string value)
		{
			if (values.ContainsKey(key))
			{
				values.Remove(key);
			}
			values.Add(key, value);
		}

		public static void Set(string key, int value)
		{
			Set(key, value.ToString());
		}

		public static void Set(string key, long value)
		{
			Set(key, value.ToString());
		}
		public static void Set(string key, float value)
		{
			Set(key, value.ToString());
		}
		public static void Set(string key, double value)
		{
			Set(key, value.ToString());
		}

		public static void Set(string key, bool value)
		{
			Set(key, value ? "true" : "false");
		}

		public static void Set<T>(string key, T value) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");

			foreach (T item in Enum.GetValues(typeof(T)))
			{
				if (item.Equals(value))
				{
					Set(key, item.ToString().ToLower());
					return;
				}
			}
			throw new ArgumentException("Enumerated value not found");
		}

		public static string GetString(string key, string defaultValue)
		{
			string value;
			if (values.TryGetValue(key, out value))
			{
				return value;
			}
			Utility.PushColor(ConsoleColor.Yellow);
			Console.WriteLine(String.Format("Config: Warning, using default value for {0}", key));
			Utility.PopColor();
			return defaultValue;
		}

		public static int GetInt(string key, int defaultValue)
		{
			string value = GetString(key, null);
			if (value == null)
			{
				return defaultValue;
			}
			return int.Parse(value);
		}

		public static long GetLong(string key, long defaultValue)
		{
			string value = GetString(key, null);
			if (value == null)
			{
				return defaultValue;
			}
			return long.Parse(value);
		}

		public static float GetFloat(string key, float defaultValue)
		{
			string value = GetString(key, null);
			if (value == null)
			{
				return defaultValue;
			}
			return float.Parse(value);
		}

		public static double GetDouble(string key, double defaultValue)
		{
			string value = GetString(key, null);
			if (value == null)
			{
				return defaultValue;
			}
			return double.Parse(value);
		}

		public static bool GetBool(string key, bool defaultValue)
		{
			string value = GetString(key, null);
			if (value == null)
			{
				return defaultValue;
			}
			value = value.ToLower();
			if (value == "yes" ||
				value == "true")
			{
				return true;
			}
			if (value == "no" ||
				value == "false")
			{
				return false;
			}
			Utility.PushColor(ConsoleColor.Yellow);
			Console.WriteLine(String.Format("Config: Warning, unrecognized boolean value for {0}", key));
			Utility.PopColor();
			return defaultValue;
		}

		public static T GetEnum<T>(string key, T defaultValue) where T : struct, IConvertible
		{
			if (!typeof(T).IsEnum) throw new ArgumentException("T must be an enumerated type");
			string value = GetString(key, null);
			if(value == null)
			{
				return defaultValue;
			}

			value = value.Trim().ToLower();
			foreach (T item in Enum.GetValues(typeof(T)))
			{
				if (item.ToString().ToLower().Equals(value)) return item;
			}
			return defaultValue;
		}
	}
}
