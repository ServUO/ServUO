using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

namespace Server
{
	public class NameList
	{
		private static readonly Dictionary<string, NameList> m_Table;

		static NameList()
		{
			m_Table = new Dictionary<string, NameList>(StringComparer.OrdinalIgnoreCase);

			var filePath = Path.Combine(Core.BaseDirectory, "Data", "names.xml");

			if (!File.Exists(filePath))
				return;

			try
			{
				Load(filePath);
			}
			catch (Exception e)
			{
				Console.WriteLine("Warning: Exception caught loading name lists:");
				Diagnostics.ExceptionLogging.LogException(e);
			}
		}

		private static void Load(string filePath)
		{
			var doc = new XmlDocument();

			doc.Load(filePath);

			var root = doc["names"];

			foreach (XmlElement element in root.GetElementsByTagName("namelist"))
			{
				var type = element.GetAttribute("type");

				if (String.IsNullOrEmpty(type))
					continue;

				try
				{
					m_Table[type] = new NameList(type, element);
				}
				catch (Exception e)
				{
					Console.WriteLine($"Warning: Exception caught loading name list '{type}':");
					Diagnostics.ExceptionLogging.LogException(e);
				}
			}
		}

		public static NameList GetRandomNameList()
		{
			return m_Table.Values.ElementAtOrDefault(Utility.Random(m_Table.Count));
		}

		public static NameList GetNameList(string type)
		{
			m_Table.TryGetValue(type, out var list);

			return list;
		}

		public static string RandomName()
		{
			return GetRandomNameList()?.GetRandomName() ?? String.Empty;
		}

		public static string RandomName(string type)
		{
			return GetNameList(type)?.GetRandomName() ?? String.Empty;
		}

		public static bool Contains(string name)
		{
			return m_Table.Values.Any(list => list.ContainsName(name));
		}

		public string Type { get; }
		public string[] List { get; }

		public NameList(string type, XmlElement xml)
		{
			Type = type;
			List = xml.InnerText.Split(',');

			for (var i = 0; i < List.Length; i++)
				List[i] = Utility.Intern(List[i].Trim());
		}

		public bool ContainsName(string name)
		{
			return List.Contains(name);
		}

		public string GetRandomName()
		{
			return Utility.RandomList(List);
		}
	}
}
