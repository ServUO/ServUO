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
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Server;

using VitaNex;
using VitaNex.Collections;
using VitaNex.IO;
using VitaNex.Text;
#endregion

namespace System
{
	public static class StringExtUtility
	{
		private static readonly Queue<object> _LockQueue = new Queue<object>(0x100);
		private static readonly Dictionary<string, object> _Locks = new Dictionary<string, object>();

		private static void LockedLog(FileInfo file, string text)
		{
			if (file == null || text == null)
			{
				return;
			}

			object value;

			lock (VitaNexCore.IOLock)
			{
				if (!_Locks.TryGetValue(file.FullName, out value) || value == null)
				{
					_Locks[file.FullName] = value = (_LockQueue.Count > 0 ? _LockQueue.Dequeue() : new object());
				}
			}

			lock (value)
			{
				file.AppendText(false, text);
			}

			lock (VitaNexCore.IOLock)
			{
				_Locks.Remove(file.FullName);

				if (_LockQueue.Count < 0x100)
				{
					_LockQueue.Enqueue(value);
				}
			}
		}

		private static readonly Regex _SpaceWordsRegex = new Regex(@"((?<=\p{Ll})\p{Lu})|((?!\A)\p{Lu}(?>\p{Ll}))");
		private static readonly Graphics _Graphics = Graphics.FromImage(new Bitmap(1, 1));

		private static readonly char[] _EscapeSearch = { '"', '\'', '/', '\\' };
		private static readonly char[] _EscapeIgnored = { 'b', 'f', 'n', 'r', 't', 'u', 'v' };
		private static readonly char[] _EscapeMerged = _EscapeSearch.Merge(_EscapeIgnored);

		private static IEnumerable<char> EscapeMap(string value)
		{
			char c, prev, next;

			for (var i = 0; i < value.Length; i++)
			{
				c = value[i];

				if (_EscapeSearch.Contains(c))
				{
					if (c == '\\')
					{
						if (i == 0)
						{
							if (i + 1 < value.Length)
							{
								next = value[i + 1];

								if (!_EscapeMerged.Contains(next))
								{
									yield return '\\';
								}
							}
							else
							{
								yield return '\\';
							}
						}
						else
						{
							prev = value[i - 1];

							if (i + 1 < value.Length)
							{
								next = value[i + 1];

								if (prev != '\\' && !_EscapeMerged.Contains(next))
								{
									yield return '\\';
								}
							}
							else if (prev != '\\')
							{
								yield return '\\';
							}
						}
					}
					else
					{
						if (i == 0)
						{
							yield return '\\';
						}
						else
						{
							prev = value[i - 1];

							if (prev != '\\')
							{
								yield return '\\';
							}
						}
					}
				}

				yield return c;
			}
		}

		public static string Escape(this string value)
		{
			return String.IsNullOrEmpty(value) ? String.Empty : String.Join(String.Empty, EscapeMap(value));
		}

		public static Size ComputeSize(
			this string str,
			SystemFont font,
			float emSize = 12,
			FontStyle style = FontStyle.Regular)
		{
			return ComputeSize(str ?? String.Empty, font.ToFont(emSize, style));
		}

		public static Size ComputeSize(this string str, Font font)
		{
			return _Graphics.MeasureString(str ?? String.Empty, font).ToSize();
		}

		public static int ComputeWidth(
			this string str,
			SystemFont font,
			float emSize = 12,
			FontStyle style = FontStyle.Regular)
		{
			return ComputeWidth(str ?? String.Empty, font.ToFont(emSize, style));
		}

		public static int ComputeWidth(this string str, Font font)
		{
			return (int)_Graphics.MeasureString(str ?? String.Empty, font).Width;
		}

		public static int ComputeHeight(
			this string str,
			SystemFont font,
			float emSize = 12,
			FontStyle style = FontStyle.Regular)
		{
			return ComputeHeight(str ?? String.Empty, font.ToFont(emSize, style));
		}

		public static int ComputeHeight(this string str, Font font)
		{
			return (int)_Graphics.MeasureString(str ?? String.Empty, font).Height;
		}

		public static Size ComputeSize(this string str, UOFont font)
		{
			return font.GetSize(str ?? String.Empty);
		}

		public static int ComputeWidth(this string str, UOFont font)
		{
			return font.GetWidth(str ?? String.Empty);
		}

		public static int ComputeHeight(this string str, UOFont font)
		{
			return font.GetHeight(str ?? String.Empty);
		}

		public static string ToLowerWords(this string str)
		{
			return ToLowerWords(str, false);
		}

		public static string ToLowerWords(this string str, bool strict)
		{
			if (String.IsNullOrWhiteSpace(str))
			{
				return str ?? String.Empty;
			}

			char c = str[0], lc;

			var value = ObjectPool<StringBuilder>.AcquireObject();

			if (Char.IsLetter(c) && Char.IsUpper(c))
			{
				c = Char.ToLower(c);
			}

			value.Append(c);

			for (var i = 1; i < str.Length; i++)
			{
				c = str[i];
				lc = str[i - 1];

				if (Char.IsLetter(c) && Char.IsUpper(c))
				{
					if (Char.IsWhiteSpace(lc))
					{
						c = Char.ToLower(c);
					}
					else if (!strict && !Char.IsDigit(lc) && !Char.IsSymbol(lc) && lc != '\'')
					{
						if (Char.IsSeparator(lc) || Char.IsPunctuation(lc))
						{
							c = Char.ToLower(c);
						}
					}
				}

				value.Append(c);
			}

			var s = value.ToString();

			ObjectPool.Free(ref value);

			return s;
		}

		public static string ToUpperWords(this string str)
		{
			return ToUpperWords(str, false);
		}

		public static string ToUpperWords(this string str, bool strict)
		{
			if (String.IsNullOrWhiteSpace(str))
			{
				return str ?? String.Empty;
			}

			char c = str[0], lc;

			var value = ObjectPool<StringBuilder>.AcquireObject();

			if (Char.IsLetter(c) && Char.IsLower(c))
			{
				c = Char.ToUpper(c);
			}

			value.Append(c);

			for (var i = 1; i < str.Length; i++)
			{
				c = str[i];
				lc = str[i - 1];

				if (Char.IsLetter(c) && Char.IsLower(c))
				{
					if (Char.IsWhiteSpace(lc) || lc == '>')
					{
						c = Char.ToUpper(c);
					}
					else if (!strict && !Char.IsDigit(lc) && !Char.IsSymbol(lc) && lc != '\'' && lc != '<')
					{
						if (Char.IsSeparator(lc) || Char.IsPunctuation(lc))
						{
							c = Char.ToUpper(c);
						}
					}
				}

				value.Append(c);
			}

			var s = value.ToString();

			ObjectPool.Free(ref value);

			return s;
		}

		public static string InvertCase(this string str)
		{
			if (String.IsNullOrWhiteSpace(str))
			{
				return str ?? String.Empty;
			}

			var value = ObjectPool<StringBuilder>.AcquireObject();

			foreach (var c in str)
			{
				if (Char.IsLetter(c))
				{
					value.Append(Char.IsLower(c) ? Char.ToUpper(c) : Char.IsUpper(c) ? Char.ToLower(c) : c);
				}
				else
				{
					value.Append(c);
				}
			}

			var s = value.ToString();

			ObjectPool.Free(ref value);

			return s;
		}

		public static string SpaceWords(this string str, params char[] whiteSpaceAlias)
		{
			if (String.IsNullOrWhiteSpace(str))
			{
				return str ?? String.Empty;
			}

			if (whiteSpaceAlias == null || whiteSpaceAlias.Length == 0)
			{
				whiteSpaceAlias = new[] { '_' };
			}

			str = whiteSpaceAlias.Aggregate(str, (s, c) => s.Replace(c, ' '));
			str = _SpaceWordsRegex.Replace(str, " $0");
			str = String.Join(" ", str.Split(' ').Not(String.IsNullOrWhiteSpace));

			return str;
		}

		public static string StripHtmlBreaks(this string str, bool preserve)
		{
			return Regex.Replace(str, @"<br[^>]?>", preserve ? "\n" : " ", RegexOptions.IgnoreCase);
		}

		public static string StripTabs(this string str)
		{
			return StripTabs(str, String.Empty);
		}

		public static string StripTabs(this string str, string replace)
		{
			return str.Replace("\t", replace);
		}

		public static string StripCRLF(this string str)
		{
			return StripCRLF(str, String.Empty);
		}

		public static string StripCRLF(this string str, string replace)
		{
			return str.Replace("\r\n", replace).Replace("\r", replace).Replace("\n", replace);
		}

		public static string StripHtml(this string str)
		{
			return StripHtml(str, false);
		}

		public static string StripHtml(this string str, bool preserve)
		{
			return StripHtml(str, preserve, String.Empty);
		}

		public static string StripHtml(this string str, bool preserve, string replace)
		{
			return preserve ? Utility.FixHtml(str) : Regex.Replace(str, @"<[^>]*>", replace);
		}

		public static string StripExcessWhiteSpace(this string str)
		{
			string old;

			do
			{
				old = str;
				str = str.Replace("  ", " ");
			}
			while (str != old);

			return str;
		}

		/// <summary>
		///     Convert a string containing UO-specific Html to web-standard Html and CSS
		/// </summary>
		/// <param name="str">String containing UO-specific Html</param>
		/// <returns>String with Html converted to web-standard Html and CSS</returns>
		public static string ConvertUOHtml(this string str)
		{
			const RegexOptions opts = RegexOptions.IgnoreCase;

			str = str.Replace("\n", "<br />");
			str = str.Replace("\r", "<br />");
			str = str.Replace("\t", String.Empty);

			str = Regex.Replace(str, @"<br>", "<br />", opts);

			str = Regex.Replace(str, @"<big>([^<]*)</big>", "<span style='font-size: large;'>$1</span>", opts);
			str = Regex.Replace(str, @"<small>([^<]*)</small>", "<span style='font-size: small;'>$1</span>", opts);

			str = Regex.Replace(str, @"<left>([^<]*)</left>", "<span style='text-align: left;'>$1</span>", opts);
			str = Regex.Replace(str, @"<center>([^<]*)</center>", "<span style='text-align: center;'>$1</span>", opts);
			str = Regex.Replace(str, @"<right>([^<]*)</right>", "<span style='text-align: right;'>$1</span>", opts);

			str = Regex.Replace(str, @"<b>([^<]*)</b>", "<span style='font-weight: bold;'>$1</span>", opts);
			str = Regex.Replace(str, @"<i>([^<]*)</i>", "<span style='font-style: italic;'>$1</span>", opts);
			str = Regex.Replace(str, @"<u>([^<]*)</u>", "<span style='text-decoration: underline;'>$1</span>", opts);
			str = Regex.Replace(str, @"<s>([^<]*)</s>", "<span style='text-decoration: line-through;'>$1</span>", opts);

			str = Regex.Replace(str, @"<(/?)basefont", "<$1span", opts);
			str = Regex.Replace(str, @"<(/?)div", "<$1span", opts);

			str = Regex.Replace(str, @"color[ =""^#]+(#[a-fA-F\d]{3,8})", "style='color: $1;'", opts);
			str = Regex.Replace(str, @"size[ =""]+(\d*)", "style='font-size: $1em;'", opts);
			str = Regex.Replace(str, @"align[ =""]+([\w\d]*)", "style='text-align: $1;'", opts);

			// Trim AA from AARRGGBB 
			str = Regex.Replace(str, @"(?=#[0-9a-fA-F]{8})#[0-9a-fA-F]{2}([0-9a-fA-F]{6})", "#$1", opts);

			// Close unclosed tags
			str = Regex.Replace(str, @"(?!<[^>]*>[^>]*</[^>]*>)<([^\s/>]*)\s?[^/>]*>([^<]*)", "$0</$1>", opts);

			// Remove empty tags
			str = Regex.Replace(str, @"<[^>]*>(\s*)</[^>]*>", "$1", opts);

			return str;
		}

		public static string WrapUOHtmlTag(this string str, string tag, params KeyValueString[] args)
		{
			if (String.IsNullOrWhiteSpace(tag))
			{
				return str ?? String.Empty;
			}

			if (args == null || args.Length == 0)
			{
				return String.Format("<{0}>{1}</{0}>", tag, str ?? String.Empty);
			}

			args = args.Not(a => String.IsNullOrWhiteSpace(a.Key)).ToArray();

			if (args.Length == 0)
			{
				return String.Format("<{0}>{1}</{0}>", tag, str ?? String.Empty);
			}

			return String.Format(
				"<{0} {1}>{2}</{0}>",
				tag,
				String.Join(" ", args.Select(a => a.Key + "=\"" + a.Value + "\"")),
				str ?? String.Empty);
		}

		public static string WrapUOHtmlTag(this string str, params string[] tags)
		{
			if (tags == null || tags.Length == 0)
			{
				return str ?? String.Empty;
			}

			tags = tags.Not(String.IsNullOrWhiteSpace).ToArray();

			if (tags.Length == 0)
			{
				return str ?? String.Empty;
			}

			return String.Format("<{0}>{1}</{0}>", String.Join("><", tags), str ?? String.Empty);
		}

		public static string WrapUOHtmlBold(this string str)
		{
			return WrapUOHtmlTag(str, "b");
		}

		public static string WrapUOHtmlItalic(this string str)
		{
			return WrapUOHtmlTag(str, "i");
		}

		public static string WrapUOHtmlUnderline(this string str)
		{
			return WrapUOHtmlTag(str, "u");
		}

		public static string WrapUOHtmlSmall(this string str)
		{
			return WrapUOHtmlTag(str, "small");
		}

		public static string WrapUOHtmlBig(this string str)
		{
			return WrapUOHtmlTag(str, "big");
		}

		public static string WrapUOHtmlCenter(this string str)
		{
			return WrapUOHtmlTag(str, "center");
		}

		public static string WrapUOHtmlUrl(this string str, string url)
		{
			return WrapUOHtmlTag(str, "a", new KeyValueString("href", url));
		}

		public static string WrapUOHtmlUrl(this string str, Uri url)
		{
			return WrapUOHtmlUrl(str, url?.AbsoluteUri ?? String.Empty);
		}

		public static string WrapUOHtmlImg(this string str, string url)
		{
			return WrapUOHtmlTag(str, "img", new KeyValueString("src", url));
		}

		public static string WrapUOHtmlImg(this string str, Uri url)
		{
			return WrapUOHtmlImg(str, url?.AbsoluteUri ?? String.Empty);
		}

		public static string WrapUOHtmlGradient(this string str, params Color[] colors)
		{
			if (colors == null || colors.Length == 0)
			{
				return str ?? String.Empty;
			}

			using (var g = new ColorGradient(colors))
			{
				var t = new StringBuilder(str.Length * g.Count);

				g.ForEachSegment(
					str.Length,
					(o, s, c) =>
					{
						if (o >= str.Length)
						{
							return;
						}

						if (o + s > str.Length)
						{
							s = str.Length - o;
						}

						t.AppendFormat(str.Substring(o, s).WrapUOHtmlColor(c, false));
					});

				return t.ToString();
			}
		}

		public static string WrapUOHtmlColors(this string str, Color555 start, Color end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start.ToColor(), end);
		}

		public static string WrapUOHtmlColors(this string str, Color555 start, Color555 end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start.ToColor(), end.ToColor());
		}

		public static string WrapUOHtmlColors(this string str, Color555 start, KnownColor end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start.ToColor(), end.ToColor());
		}

		public static string WrapUOHtmlColors(this string str, KnownColor start, Color end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start.ToColor(), end);
		}

		public static string WrapUOHtmlColors(this string str, KnownColor start, Color555 end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start.ToColor(), end.ToColor());
		}

		public static string WrapUOHtmlColors(this string str, KnownColor start, KnownColor end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start.ToColor(), end.ToColor());
		}

		public static string WrapUOHtmlColors(this string str, Color start, Color555 end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start, end.ToColor());
		}

		public static string WrapUOHtmlColors(this string str, Color start, KnownColor end)
		{
			return WrapUOHtmlColors(str ?? String.Empty, start, end.ToColor());
		}

		public static string WrapUOHtmlColors(this string str, Color start, Color end)
		{
			if (start == end)
			{
				return WrapUOHtmlColor(str, start);
			}

			var t = new StringBuilder();

			var tags = DictionaryPool<int, string>.AcquireObject();

			var tago = false;
			var tagi = 0;

			for (var i = 0; i < str.Length; i++)
			{
				if (str[i] == '<')
				{
					tago = true;
					tagi = i;
				}
				else if (tago && str[i] == '>')
				{
					tago = false;
				}

				if (tago)
				{
					if (i > tagi)
					{
						t.Append(str[i]);
					}
				}
				else if (t.Length > 0)
				{
					tags[tagi] = t.ToString();

					t.Clear();
				}
			}

			t.Clear();

			double n, o = 0.0;
			string tag, s;

			for (var i = 0; i < str.Length; i++)
			{
				tag = tags.GetValue(i);

				if (tag != null)
				{
					t.Append("<" + tag + ">");
				}

				n = i / (double)str.Length;

				if (n <= 0 || n >= o + 0.05)
				{
					o = n;
				}

				s = str[i].ToString();

				t.Append(o == n ? s.WrapUOHtmlColor(start.Interpolate(end, n), false) : s);
			}

			DictionaryPool<int, string>.FreeObject(tags);

			return t.ToString();
		}

		private static readonly KeyValueString _AlignRight = new KeyValueString("align", "right");

		public static string WrapUOHtmlRight(this string str)
		{
			return WrapUOHtmlTag(str, "div", _AlignRight);
		}

		public static string WrapUOHtmlColor(this string str, Color555 color, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), Color.White, close);
		}

		public static string WrapUOHtmlColor(this string str, Color555 color, Color reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), reset, close);
		}

		public static string WrapUOHtmlColor(this string str, Color555 color, KnownColor reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), reset.ToColor(), close);
		}

		public static string WrapUOHtmlColor(this string str, Color555 color, Color555 reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), reset.ToColor(), close);
		}

		public static string WrapUOHtmlColor(this string str, KnownColor color, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), Color.White, close);
		}

		public static string WrapUOHtmlColor(this string str, KnownColor color, Color reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), reset, close);
		}

		public static string WrapUOHtmlColor(this string str, KnownColor color, Color555 reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), reset.ToColor(), close);
		}

		public static string WrapUOHtmlColor(this string str, KnownColor color, KnownColor reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color.ToColor(), reset.ToColor(), close);
		}

		public static string WrapUOHtmlColor(this string str, Color color, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color, Color.White, close);
		}

		public static string WrapUOHtmlColor(this string str, Color color, KnownColor reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color, reset.ToColor(), close);
		}

		public static string WrapUOHtmlColor(this string str, Color color, Color555 reset, bool close = true)
		{
			return WrapUOHtmlColor(str ?? String.Empty, color, reset.ToColor(), close);
		}

		public static string WrapUOHtmlColor(this string str, Color color, Color reset, bool close = true)
		{
			color = color.FixBlackTransparency();

			if (close)
			{
				reset = reset.FixBlackTransparency();

				return String.Format("<basefont color=#{0:X6}>{2}<basefont color=#{1:X6}>", color.ToRgb(), reset.ToRgb(), str);
			}

			return String.Format("<basefont color=#{0:X6}>{1}", color.ToRgb(), str);
		}

		public static string WrapUOHtmlBG(this string str, Color555 color)
		{
			return WrapUOHtmlBG(str ?? String.Empty, color.ToColor());
		}

		public static string WrapUOHtmlBG(this string str, KnownColor color)
		{
			return WrapUOHtmlBG(str ?? String.Empty, Color.FromKnownColor(color));
		}

		public static string WrapUOHtmlBG(this string str, Color color)
		{
			color = color.FixBlackTransparency();

			return String.Format("<bodybgcolor=#{0:X6}>{1}", color.ToRgb(), str);
		}

		public static string WrapChars(this string str, int cols)
		{
			return ToWrappedString(str, " ", cols);
		}

		public static string WrapWords(this string str, int cols)
		{
			return ToWrappedString(str.Split(' ').Not(String.IsNullOrEmpty), " ", cols);
		}

		public static string ToWrappedString<T>(this IEnumerable<T> source, string sep, int wrap)
		{
			source = source.Ensure();

			if (sep == null)
			{
				sep = String.Empty;
			}

			if (wrap <= 0)
			{
				return String.Join(sep, source);
			}

			var strings = ListPool<string>.AcquireObject();
			var values = ObjectPool<StringBuilder>.AcquireObject();

			try
			{
				strings.AddRange(source.Select(o => Convert.ToString(o)));

				if (strings.Count == 0)
				{
					return String.Empty;
				}

				var capacity = strings.Sum(s => s.Length + sep.Length) + (strings.Count / wrap);

				values.EnsureCapacity(capacity);

				var i = 0;

				foreach (var t in strings)
				{
					values.Append(t);

					if (++i < strings.Count)
					{
						values.Append(sep);

						if (i % wrap == 0)
						{
							values.Append('\n');
						}
					}
				}

				return values.ToString();
			}
			catch
			{
				return String.Empty;
			}
			finally
			{
				ObjectPool.Free(ref strings);
				ObjectPool.Free(ref values);
			}
		}

		public static void AppendLine(this StringBuilder sb, string format, params object[] args)
		{
			sb.AppendLine(String.Format(format ?? String.Empty, args));
		}

		public static void Log(this StringBuilder sb)
		{
			Log(sb, VitaNexCore.LogFile);
		}

		public static void Log(this StringBuilder sb, string file)
		{
			var root = String.IsNullOrWhiteSpace(file)
				? VitaNexCore.LogFile
				: IOUtility.EnsureFile(VitaNexCore.LogsDirectory + "/" + file);

			Log(sb, root);
		}

		public static void Log(this StringBuilder sb, FileInfo file)
		{
			if (sb != null)
			{
				LockedLog(file ?? VitaNexCore.LogFile, sb.ToString());
			}
		}

		public static void Log(this string text)
		{
			Log(text, VitaNexCore.LogFile);
		}

		public static void Log(this string text, string file)
		{
			FileInfo root;

			if (String.IsNullOrWhiteSpace(file))
			{
				root = VitaNexCore.LogFile;
			}
			else if (Path.IsPathRooted(file))
			{
				root = IOUtility.EnsureFile(file);
			}
			else
			{
				root = IOUtility.EnsureFile(VitaNexCore.LogsDirectory + "/" + file);
			}

			Log(text, root);
		}

		public static void Log(this string text, FileInfo file)
		{
			if (text != null)
			{
				LockedLog(file ?? VitaNexCore.LogFile, text);
			}
		}

		public static void ToConsole(this StringBuilder sb, bool log = false)
		{
			if (sb == null)
			{
				return;
			}

			lock (VitaNexCore.ConsoleLock)
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine(sb.ToString());
				Utility.PopColor();
			}

			if (log)
			{
				Log(sb);
			}
		}

		public static void ToConsole(this string text, bool log = false)
		{
			if (text == null)
			{
				return;
			}

			lock (VitaNexCore.ConsoleLock)
			{
				Utility.PushColor(ConsoleColor.Yellow);
				Console.WriteLine(text);
				Utility.PopColor();
			}

			if (log)
			{
				Log(text);
			}
		}

		private static readonly Type _ParsableAttribute = typeof(ParsableAttribute);

		private static readonly Type[] _ParsableTryParams = { typeof(string), null };
		private static readonly object[] _ParsableTryArgs = { null, null };
		private static readonly object _ParsableTryLock = new object();

		private static readonly Type[] _ParsableParams = { typeof(string) };
		private static readonly object[] _ParsableArgs = { null };
		private static readonly object _ParsableLock = new object();

		public static bool TryParse<T>(this string text, out T value)
		{
			var type = typeof(T);

			if (SimpleType.IsSimpleType(type))
			{
				return SimpleType.TryParse(text, out value);
			}

			value = default(T);

			if (!type.IsDefined(_ParsableAttribute, false))
			{
				return false;
			}

			lock (_ParsableTryLock)
			{
				try
				{
					_ParsableTryParams[1] = type;

					var tryParse = type.GetMethod("TryParse", _ParsableTryParams);

					_ParsableTryParams[1] = null;

					_ParsableTryArgs[0] = text;
					_ParsableTryArgs[1] = value;

					var val = false;

					if (tryParse != null)
					{
						val = (bool)tryParse.Invoke(null, _ParsableTryArgs);
					}

					value = (T)_ParsableTryArgs[1];

					_ParsableTryArgs[0] = null;
					_ParsableTryArgs[1] = null;

					if (val)
					{
						return true;
					}
				}
				catch
				{
					_ParsableTryArgs[0] = null;
					_ParsableTryArgs[1] = null;
					_ParsableTryParams[1] = null;
				}
			}

			lock (_ParsableLock)
			{
				try
				{
					var parse = type.GetMethod("Parse", _ParsableParams);

					_ParsableArgs[0] = text;

					if (parse != null)
					{
						value = (T)parse.Invoke(null, _ParsableArgs);
					}

					_ParsableArgs[0] = null;

					return true;
				}
				catch
				{
					_ParsableArgs[0] = null;
				}
			}

			value = default(T);

			return false;
		}

		public static bool EqualsAny(this string text, params string[] values)
		{
			return EqualsAny(text, false, values);
		}

		public static bool EqualsAny(this string text, bool ignoreCase, params string[] values)
		{
			return EqualsAny(text, values, ignoreCase);
		}

		public static bool EqualsAny(this string text, IEnumerable<string> values, StringComparison comparisonType)
		{
			return values.Any(value => text.Equals(value, comparisonType));
		}

		public static bool EqualsAny(this string text, IEnumerable<string> values)
		{
			return EqualsAny(text, values, false);
		}

		public static bool EqualsAny(this string text, IEnumerable<string> values, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return values.Any(value => Insensitive.Equals(text, value));
			}

			return values.Any(value => String.Equals(text, value));
		}

		public static bool StartsWithAny(this string text, params string[] values)
		{
			return StartsWithAny(text, false, values);
		}

		public static bool StartsWithAny(this string text, bool ignoreCase, params string[] values)
		{
			return StartsWithAny(text, values, ignoreCase);
		}

		public static bool StartsWithAny(this string text, IEnumerable<string> values, bool ignoreCase)
		{
			return StartsWithAny(text, values, ignoreCase, CultureInfo.CurrentCulture);
		}

		public static bool StartsWithAny(this string text, IEnumerable<string> values, bool ignoreCase, CultureInfo culture)
		{
			return values.Any(value => text.StartsWith(value, ignoreCase, culture));
		}

		public static bool StartsWithAny(this string text, IEnumerable<string> values, StringComparison comparisonType)
		{
			return values.Any(value => text.StartsWith(value, comparisonType));
		}

		public static bool StartsWithAny(this string text, IEnumerable<string> values)
		{
			return values.Any(text.StartsWith);
		}

		public static bool EndsWithAny(this string text, params string[] values)
		{
			return EndsWithAny(text, false, values);
		}

		public static bool EndsWithAny(this string text, bool ignoreCase, params string[] values)
		{
			return EndsWithAny(text, values, ignoreCase);
		}

		public static bool EndsWithAny(this string text, IEnumerable<string> values, bool ignoreCase)
		{
			return EndsWithAny(text, values, ignoreCase, CultureInfo.CurrentCulture);
		}

		public static bool EndsWithAny(this string text, IEnumerable<string> values, bool ignoreCase, CultureInfo culture)
		{
			return values.Any(value => text.EndsWith(value, ignoreCase, culture));
		}

		public static bool EndsWithAny(this string text, IEnumerable<string> values, StringComparison comparisonType)
		{
			return values.Any(value => text.EndsWith(value, comparisonType));
		}

		public static bool EndsWithAny(this string text, IEnumerable<string> values)
		{
			return values.Any(text.EndsWith);
		}

		public static bool ContainsAny(this string text, params char[] values)
		{
			return ContainsAny(text, false, values);
		}

		public static bool ContainsAny(this string text, bool ignoreCase, params char[] values)
		{
			return ContainsAny(text, values, ignoreCase);
		}

		public static bool ContainsAny(this string text, params string[] values)
		{
			return ContainsAny(text, false, values);
		}

		public static bool ContainsAny(this string text, bool ignoreCase, params string[] values)
		{
			return ContainsAny(text, values, ignoreCase);
		}

		public static bool ContainsAny(this string text, IEnumerable<char> values, IEqualityComparer<char> comparer)
		{
			return values.Any(value => text.Contains(value, comparer));
		}

		public static bool ContainsAny(this string text, IEnumerable<char> values)
		{
			return ContainsAny(text, values, false);
		}

		public static bool ContainsAny(this string text, IEnumerable<char> values, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return values.Any(value => Insensitive.Contains(text, value.ToString()));
			}

			return values.Any(text.Contains);
		}

		public static bool ContainsAny(this string text, IEnumerable<string> values)
		{
			return ContainsAny(text, values, false);
		}

		public static bool ContainsAny(this string text, IEnumerable<string> values, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return values.Any(value => Insensitive.Contains(text, value));
			}

			return values.Any(text.Contains);
		}

		public static bool ContainsAll(this string text, params char[] values)
		{
			return ContainsAll(text, false, values);
		}

		public static bool ContainsAll(this string text, bool ignoreCase, params char[] values)
		{
			return ContainsAll(text, values, ignoreCase);
		}

		public static bool ContainsAll(this string text, params string[] values)
		{
			return ContainsAll(text, false, values);
		}

		public static bool ContainsAll(this string text, bool ignoreCase, params string[] values)
		{
			return ContainsAll(text, values, ignoreCase);
		}

		public static bool ContainsAll(this string text, IEnumerable<char> values, IEqualityComparer<char> comparer)
		{
			return values.All(value => text.Contains(value, comparer));
		}

		public static bool ContainsAll(this string text, IEnumerable<char> values)
		{
			return ContainsAll(text, values, false);
		}

		public static bool ContainsAll(this string text, IEnumerable<char> values, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return values.All(value => Insensitive.Contains(text, value.ToString()));
			}

			return values.All(text.Contains);
		}

		public static bool ContainsAll(this string text, IEnumerable<string> values)
		{
			return ContainsAll(text, values, false);
		}

		public static bool ContainsAll(this string text, IEnumerable<string> values, bool ignoreCase)
		{
			if (ignoreCase)
			{
				return values.All(value => Insensitive.Contains(text, value));
			}

			return values.All(text.Contains);
		}
	}
}
