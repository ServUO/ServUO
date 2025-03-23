#region Header
//               _,-'/-'/
//   .      __,-; ,'( '/
//    \.    `-.__`-._`:_,-._       _ , . ``
//     `:-._,------' ` _,`--` -: `_ , ` ,' :
//        `---..__,,--'  (C) 2023  ` -'. -'
//        #  Vita-Nex [http://core.vita-nex.com]  #
//  {o)xxx|===============-   #   -===============|xxx(o}
//        #                                       #
#endregion

#region References
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

using Server;
using Server.Commands;

using VitaNex.Collections;
using VitaNex.IO;
using VitaNex.Network;
#endregion

namespace VitaNex
{
	public static partial class Clilocs
	{
		private static readonly Dictionary<Type, int> _TypeCache = new Dictionary<Type, int>(0x1000);

		private static readonly ObjectProperty _LabelNumberProp = new ObjectProperty("LabelNumber");

		private static readonly ClilocLNG[] _Languages = default(ClilocLNG).EnumerateValues<ClilocLNG>().Not(o => o == ClilocLNG.NULL).ToArray();

		public static readonly Regex VarPattern = new Regex(@"~(?<index>\d+)(?<sep>_*)(?<desc>\w*)~", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);
		public static readonly Regex NumPattern = new Regex(@"#(?<index>\d+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

		public static ClilocLNG DefaultLanguage { get; set; } = ClilocLNG.ENU;

		public static CoreServiceOptions CSOptions { get; private set; }

		public static Dictionary<ClilocLNG, ClilocTable> Tables { get; private set; }

		private static void ExportCommand(CommandEventArgs e)
		{
			if (e.Mobile?.Deleted != false)
			{
				return;
			}

			e.Mobile.SendMessage(0x55, "Export requested...");

			if (e.Arguments?.Length > 0 && !String.IsNullOrWhiteSpace(e.Arguments[0]))
			{
				if (Enum.TryParse(e.Arguments[0], true, out ClilocLNG lng) && lng != ClilocLNG.NULL)
				{
					VitaNexCore.TryCatch(() =>
					{
						var file = Export(lng);

						if (file != null && file.Exists && file.Length > 0)
						{
							e.Mobile.SendMessage(0x55, $"{lng} clilocs have been exported to: {file.FullName}");
						}
						else
						{
							e.Mobile.SendMessage(0x22, $"Could not export clilocs for {lng}");
						}
					},
					ex =>
					{
						e.Mobile.SendMessage(0x22, "A fatal exception occurred, check the console for details.");

						CSOptions.ToConsole(ex);
					});

					return;
				}
			}

			e.Mobile.SendMessage($"Usage: {CommandSystem.Prefix}{e.Command} <{String.Join(" | ", _Languages)}>");
		}

		public static FileInfo Export(ClilocLNG lng)
		{
			if (lng == ClilocLNG.NULL)
			{
				lng = DefaultLanguage;
			}

			var list = new XmlDataStore<int, ClilocData>(VitaNexCore.DataDirectory + "/Exported Clilocs/", lng.ToString());
			var table = Tables[lng];

			list.OnSerialize = doc =>
			{
				XmlNode node;
				XmlCDataSection cdata;
				ClilocInfo info;

				XmlNode root = doc.CreateElement("clilocs");

				var attr = doc.CreateAttribute("len");

				attr.Value = table.Count.ToString(CultureInfo.InvariantCulture);

				if (root.Attributes != null)
				{
					root.Attributes.Append(attr);
				}

				attr = doc.CreateAttribute("lng");
				attr.Value = table.Language.ToString();

				if (root.Attributes != null)
				{
					root.Attributes.Append(attr);
				}

				foreach (var d in table)
				{
					if (d.Length <= 0)
					{
						continue;
					}

					info = d.Lookup(table.InputFile, true);

					if (String.IsNullOrWhiteSpace(info?.Text))
					{
						continue;
					}

					node = doc.CreateElement("cliloc");

					attr = doc.CreateAttribute("idx");
					attr.Value = d.Index.ToString(CultureInfo.InvariantCulture);

					if (node.Attributes != null)
					{
						node.Attributes.Append(attr);
					}

					attr = doc.CreateAttribute("len");
					attr.Value = d.Length.ToString(CultureInfo.InvariantCulture);

					if (node.Attributes != null)
					{
						node.Attributes.Append(attr);
					}

					cdata = doc.CreateCDataSection(info.Text);
					node.AppendChild(cdata);

					root.AppendChild(node);
				}

				doc.AppendChild(root);
				table.Clear();

				return true;
			};

			list.Export();
			list.Clear();

			return list.Document;
		}

		public static ClilocInfo Lookup(this ClilocLNG lng, Type type)
		{
			if (type == null)
			{
				return null;
			}

			if (lng == ClilocLNG.NULL)
			{
				lng = DefaultLanguage;
			}

			try
			{
				if (_TypeCache.TryGetValue(type, out var index))
				{
					if (index < 0)
					{
						return null;
					}

					return Lookup(lng, index);
				}

				if (!_LabelNumberProp.IsSupported(typeof(int), type))
				{
					return null;
				}

				var o = type.CreateInstanceSafe<object>();

				if (o != null)
				{
					index = _LabelNumberProp.GetValue(o, -1); // LabelNumber_get()

					o.InvokeMethod("Delete");
				}
				else
				{
					index = -1;
				}

				_TypeCache[type] = index;

				if (index >= 0)
				{
					return Lookup(lng, index);
				}
			}
			catch
			{
			}

			return null;
		}

		public static ClilocInfo Lookup(this ClilocLNG lng, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (lng == ClilocLNG.NULL)
			{
				lng = DefaultLanguage;
			}

			if (Tables.TryGetValue(lng, out var table) && !table.IsNullOrWhiteSpace(index))
			{
				return table[index];
			}

			if (lng != ClilocLNG.ENU)
			{
				return Lookup(ClilocLNG.ENU, index);
			}

			return null;
		}

		public static string GetRawString(this ClilocLNG lng, int index)
		{
			if (index < 0)
			{
				return null;
			}

			if (lng == ClilocLNG.NULL)
			{
				lng = DefaultLanguage;
			}

			if (Tables.TryGetValue(lng, out var table) && !table.IsNullOrWhiteSpace(index))
			{
				return table[index]?.Text ?? String.Empty;
			}

			if (lng != ClilocLNG.ENU)
			{
				return GetRawString(ClilocLNG.ENU, index);
			}

			return String.Empty;
		}

		public static string GetString(this ClilocLNG lng, int index, StringBuilder args)
		{
			return Lookup(lng, index)?.ToString(args) ?? String.Empty;
		}

		public static string GetString(this ClilocLNG lng, int index, string args)
		{
			return Lookup(lng, index)?.ToString(args) ?? String.Empty;
		}

		public static string GetString(this ClilocLNG lng, int index, params object[] args)
		{
			return Lookup(lng, index)?.ToString(args) ?? String.Empty;
		}

		public static string GetString(this ClilocLNG lng, Type type)
		{
			return Lookup(lng, type)?.ToString() ?? String.Empty;
		}

		public static string GetString(this TextDefinition text)
		{
			return GetString(text, DefaultLanguage);
		}

		public static string GetString(this TextDefinition text, Mobile m)
		{
			return GetString(text, GetLanguage(m));
		}

		public static string GetString(this TextDefinition text, ClilocLNG lng)
		{
			if (ReferenceEquals(text, null))
			{
				return String.Empty;
			}

			return text.Number > 0 ? GetString(lng, text.Number) : (text.String ?? String.Empty);
		}

		public static bool IsNullOrEmpty(this TextDefinition text)
		{
			if (ReferenceEquals(text, null))
			{
				return true;
			}

			return text.Number <= 0 && String.IsNullOrEmpty(text.String) && String.IsNullOrEmpty(GetString(text));
		}

		public static bool IsNullOrWhiteSpace(this TextDefinition text)
		{
			if (ReferenceEquals(text, null))
			{
				return true;
			}

			return text.Number <= 0 && String.IsNullOrWhiteSpace(text.String) && String.IsNullOrWhiteSpace(GetString(text));
		}

		private static IEnumerable<T> Enumerate<T>(ObjectPropertyList list, ClilocLNG lng, bool lookup, Func<int, string, T> selector)
		{
			if (list?.Entity?.Deleted != false || selector == null)
			{
				yield break;
			}

			if (lng == ClilocLNG.NULL)
			{
				lng = DefaultLanguage;
			}

			ObjectPool.Acquire(out StringBuilder param);

			var data = list.GetBuffer();

			var index = 15;

			while (index + 4 < data.Length)
			{
				var num = data[index++] << 24 | data[index++] << 16 | data[index++] << 8 | data[index++];

				if (index + 2 > data.Length)
				{
					break;
				}

				var paramLength = data[index++] << 8 | data[index++];

				if (paramLength > 0)
				{
					var terminate = index + paramLength;

					if (terminate >= data.Length)
					{
						terminate = data.Length - 1;
					}

					while (index + 2 <= terminate + 1)
					{
						if ((data[index++] | data[index++] << 8) == 0)
						{
							break;
						}

						param.Append(Encoding.Unicode.GetChars(data, index - 2, 2));
					}
				}

				yield return selector(num, lookup ? GetString(lng, num, param) : String.Empty);

				param.Clear();
			}

			ObjectPool.Free(ref param);
		}

		public static IEnumerable<int> EnumerateNumbers(this ObjectPropertyList list)
		{
			return Enumerate(list, DefaultLanguage, false, (id, text) => id);
		}

		public static IEnumerable<string> EnumerateLines(this ObjectPropertyList list)
		{
			return EnumerateLines(list, DefaultLanguage);
		}

		public static IEnumerable<string> EnumerateLines(this ObjectPropertyList list, Mobile m)
		{
			return EnumerateLines(list, GetLanguage(m));
		}

		public static IEnumerable<string> EnumerateLines(this ObjectPropertyList list, ClilocLNG lng)
		{
			return Enumerate(list, lng, true, (id, text) => text);
		}

		public static IEnumerable<TextDefinition> EnumerateEntries(this ObjectPropertyList list)
		{
			return EnumerateEntries(list, DefaultLanguage);
		}

		public static IEnumerable<TextDefinition> EnumerateEntries(this ObjectPropertyList list, Mobile m)
		{
			return EnumerateEntries(list, GetLanguage(m));
		}

		public static IEnumerable<TextDefinition> EnumerateEntries(this ObjectPropertyList list, ClilocLNG lng)
		{
			return Enumerate(list, lng, true, (id, text) => new TextDefinition(id, text));
		}

		public static string[] DecodePropertyList(this ObjectPropertyList list)
		{
			return DecodePropertyList(list, DefaultLanguage);
		}

		public static string[] DecodePropertyList(this ObjectPropertyList list, Mobile m)
		{
			return DecodePropertyList(list, GetLanguage(m));
		}

		public static string[] DecodePropertyList(this ObjectPropertyList list, ClilocLNG lng)
		{
			return EnumerateLines(list, lng).ToArray();
		}

		public static string DecodePropertyListHeader(this ObjectPropertyList list)
		{
			return DecodePropertyListHeader(list, DefaultLanguage);
		}

		public static string DecodePropertyListHeader(this ObjectPropertyList list, Mobile v)
		{
			return DecodePropertyListHeader(list, GetLanguage(v));
		}

		public static string DecodePropertyListHeader(this ObjectPropertyList list, ClilocLNG lng)
		{
			return EnumerateLines(list, lng).FirstOrDefault() ?? String.Empty;
		}

		public static string[] GetAllLines(this ObjectPropertyList list)
		{
			return DecodePropertyList(list);
		}

		public static string[] GetAllLines(this ObjectPropertyList list, ClilocLNG lng)
		{
			return DecodePropertyList(list, lng);
		}

		public static string GetHeader(this ObjectPropertyList list)
		{
			return DecodePropertyListHeader(list);
		}

		public static string GetHeader(this ObjectPropertyList list, ClilocLNG lng)
		{
			return DecodePropertyListHeader(list, lng);
		}

		public static string[] GetBody(this ObjectPropertyList list)
		{
			return GetBody(list, DefaultLanguage);
		}

		public static string[] GetBody(this ObjectPropertyList list, ClilocLNG lng)
		{
			return EnumerateLines(list, lng).Skip(1).ToArray();
		}

		public static bool Contains(this ObjectPropertyList list, int num)
		{
			return IndexOf(list, num) != -1;
		}

		public static int IndexOf(this ObjectPropertyList list, int num)
		{
			return EnumerateNumbers(list).IndexOf(num);
		}

		public static ClilocLNG GetLanguage(this Mobile m)
		{
			if (m == null)
			{
				return ClilocLNG.NULL;
			}

			if (!Enum.TryParse(m.Language, out ClilocLNG lng))
			{
				lng = ClilocLNG.ENU;
			}

			return lng;
		}
	}
}
