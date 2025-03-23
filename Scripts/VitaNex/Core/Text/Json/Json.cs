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
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
#endregion

namespace VitaNex.Text
{
	public static class Json
	{
		public static Encoding DefaultEncoding = Encoding.UTF8;

		/// <summary>
		///     Input: IDictionary (Object), IEnumerable (Array), Simple Types
		/// </summary>
		public static string Encode(object value)
		{
			return Encode(value, out JsonException _);
		}

		/// <summary>
		///     Input: IDictionary (Object), IEnumerable (Array), Simple Types
		/// </summary>
		public static string Encode(object value, out JsonException e)
		{
			if (Encode(value, out var json, out e))
			{
				return json;
			}

			return null;
		}

		/// <summary>
		///     Input: IDictionary (Object), IEnumerable (Array), Simple Types
		/// </summary>
		public static bool Encode(object value, out string json)
		{
			return Encode(value, out json, out _);
		}

		/// <summary>
		///     Input: IDictionary (Object), IEnumerable (Array), Simple Types
		/// </summary>
		public static bool Encode(object value, out string json, out JsonException e)
		{
			json = null;
			e = null;

			try
			{
				var sb = new StringBuilder(0x800);

				if (SerializeValue(0, value, sb))
				{
					json = sb.ToString();

					return true;
				}

				int line;
				int col;

				var sub = sb.ToString();
				var idx = Math.Max(sub.LastIndexOf('\r'), sub.LastIndexOf('\n'));

				if (idx > -1)
				{
					line = 1 + sub.Count(c => c == '\r' || c == '\n');
					col = sub.Length - idx;
				}
				else
				{
					line = 1;
					col = sub.Length;
				}

				sub = sub.Substring(Math.Max(0, sub.Length - 16));

				e = new JsonException(String.Format("Encoding failed at line {0} col {1} near {2}", line, col, sub));
			}
			catch (Exception x)
			{
				if (x is JsonException)
				{
					e = (JsonException)x;
				}
				else
				{
					e = new JsonException("Encoding failed", x);
				}
			}

			return false;
		}

		/// <summary>
		///     Output: Dictionary[string, object] (Object), IEnumerable[object] (Array), Simple Types
		/// </summary>
		public static object Decode(string json)
		{
			return Decode(json, out JsonException _);
		}

		/// <summary>
		///     Output: Dictionary[string, object] (Object), IEnumerable[object] (Array), Simple Types
		/// </summary>
		public static object Decode(string json, out JsonException e)
		{
			if (Decode(json, out var value, out e))
			{
				return value;
			}

			return null;
		}

		/// <summary>
		///     Output: Dictionary[string, object] (Object), IEnumerable[object] (Array), Simple Types
		/// </summary>
		public static bool Decode(string json, out object value)
		{
			return Decode(json, out value, out _);
		}

		/// <summary>
		///     Output: Dictionary[string, object] (Object), IEnumerable[object] (Array), Simple Types
		/// </summary>
		public static bool Decode(string json, out object value, out JsonException e)
		{
			value = null;
			e = null;

			try
			{
				var index = 0;

				if (DeserializeValue(json, ref index, out value))
				{
					return true;
				}

				int line;
				int col;

				var sub = json.Substring(0, index);
				var idx = Math.Max(sub.LastIndexOf('\r'), sub.LastIndexOf('\n'));

				if (idx > -1)
				{
					line = 1 + sub.Count(c => c == '\r' || c == '\n');
					col = index - idx;
				}
				else
				{
					line = 1;
					col = index;
				}

				sub = json.Substring(index, Math.Min(16, json.Length - index));

				e = new JsonException(String.Format("Decoding failed at line {0} col {1} near {2}", line, col, sub));
			}
			catch (Exception x)
			{
				if (x is JsonException)
				{
					e = (JsonException)x;
				}
				else
				{
					e = new JsonException("Decoding failed", x);
				}
			}

			return false;
		}

		private static bool SerializeValue(int depth, object val, StringBuilder json)
		{
			if (val is string)
			{
				return SerializeString((string)val, json);
			}

			if (val is IDictionary)
			{
				return SerializeObject(depth, (IDictionary)val, json);
			}

			if (val is IEnumerable)
			{
				return SerializeArray(depth, (IEnumerable)val, json);
			}

			if (val is bool && (bool)val)
			{
				json.Append("true");

				return true;
			}

			if (val is bool && !(bool)val)
			{
				json.Append("false");

				return true;
			}

			if (val is char)
			{
				return SerializeString(val.ToString(), json);
			}

			if (val is ValueType)
			{
				if (!(val is IConvertible))
				{
					return SerializeString(val.ToString(), json);
				}

				return SerializeNumber(Convert.ToDouble(val), json);
			}

			if (val == null)
			{
				json.Append("null");

				return true;
			}

			return SerializeString(val.ToString(), json);
		}

		private static bool DeserializeValue(string json, ref int index, out object value)
		{
			value = null;

			switch (PeekToken(json, index))
			{
				case JsonToken.Number:
					return DeserializeNumber(json, ref index, out value);
				case JsonToken.String:
					return DeserializeString(json, ref index, out value);
				case JsonToken.ArrayOpen:
					return DeserializeArray(json, ref index, out value);
				case JsonToken.ObjectOpen:
					return DeserializeObject(json, ref index, out value);
				case JsonToken.True:
				{
					PeekToken(json, ref index);
					value = true;
				}
				return true;
				case JsonToken.False:
				{
					PeekToken(json, ref index);
					value = false;
				}
				return true;
				case JsonToken.Null:
					PeekToken(json, ref index);
					return true;
				case JsonToken.None:
					break;
			}

			return false;
		}

		private static bool SerializeNumber(double num, StringBuilder json)
		{
			json.Append(Convert.ToString(num, CultureInfo.InvariantCulture));

			return true;
		}

		private static bool DeserializeNumber(string json, ref int index, out object value)
		{
			value = null;

			index = SkipWhiteSpace(json, index);

			index = PeekNumber(json, index, out var len);

			var val = json.Substring(index, len);

			if (Double.TryParse(val, NumberStyles.Any, CultureInfo.InvariantCulture, out var num))
			{
				index += len;
				value = num;
				return true;
			}

			return false;
		}

		private static bool SerializeString(string str, StringBuilder json)
		{
			json.Append('"');

			foreach (var c in str)
			{
				switch (c)
				{
					case '"':
						json.Append("\\\"");
						break;
					case '\\':
						json.Append("\\\\");
						break;
					case '\b':
						json.Append("\\b");
						break;
					case '\f':
						json.Append("\\f");
						break;
					case '\n':
						json.Append("\\n");
						break;
					case '\r':
						json.Append("\\r");
						break;
					case '\t':
						json.Append("\\t");
						break;
					default:
					{
						var sur = Convert.ToInt32(c);

						if (sur >= 32 && sur <= 126)
						{
							json.Append(c);
						}
						else
						{
							json.Append("\\u" + Convert.ToString(sur, 16).PadLeft(4, '0'));
						}
					}
					break;
				}
			}

			json.Append('"');

			return true;
		}

		private static bool DeserializeString(string json, ref int index, out object value)
		{
			value = null;

			PeekToken(json, ref index); // " or '

			var o = json[index - 1];

			var str = new StringBuilder(0x20);

			char c;

			while (index < json.Length)
			{
				c = json[index++];

				if (c == o)
				{
					value = str.ToString();
					return true;
				}

				if (c == '\\')
				{
					if (index == json.Length)
					{
						break;
					}

					c = json[index++];

					var found = true;

					switch (c)
					{
						case '"':
							str.Append('"');
							break;
						case '\\':
							str.Append('\\');
							break;
						case '/':
							str.Append('/');
							break;
						case 'b':
							str.Append('\b');
							break;
						case 'f':
							str.Append('\f');
							break;
						case 'n':
							str.Append('\n');
							break;
						case 'r':
							str.Append('\r');
							break;
						case 't':
							str.Append('\t');
							break;
						default:
							found = false;
							break;
					}

					if (!found && c == 'u')
					{
						var length = json.Length - index;

						if (length < 4)
						{
							break;
						}

						var sub = json.Substring(index, 4);

						if (!UInt32.TryParse(sub, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var sur))
						{
							return false;
						}

						str.Append(Char.ConvertFromUtf32((int)sur));
						index += 4;
					}
				}
				else
				{
					str.Append(c);
				}
			}

			return false;
		}

		private static bool SerializeArray(int depth, IEnumerable arr, StringBuilder json)
		{
			var tabs = new string('\t', depth++);
			var line = "\r\n" + tabs;

			var success = true;

			json.Append((depth > 1 ? line : String.Empty) + '[');

			var first = true;

			foreach (var value in arr)
			{
				if (!first)
				{
					json.Append(", ");
				}

				json.Append(line + '\t');

				if (!SerializeValue(depth, value, json))
				{
					success = false;
					break;
				}

				first = false;
			}

			if (success)
			{
				json.Append(line + ']');
			}

			return success;
		}

		private static bool DeserializeArray(string json, ref int index, out object value)
		{
			value = null;

			PeekToken(json, ref index); // [

			var arr = new List<object>();

			while (index < json.Length)
			{
				switch (PeekToken(json, index))
				{
					case JsonToken.None:
						return false;
					case JsonToken.Comma:
						PeekToken(json, ref index); // ,
						break;
					case JsonToken.ArrayClose:
					{
						PeekToken(json, ref index); // ]
						value = arr;
					}
					return true;
					default:
					{
						if (!DeserializeValue(json, ref index, out var val))
						{
							return false;
						}

						arr.Add(val);
					}
					break;
				}
			}

			return false;
		}

		private static bool SerializeObject(int depth, IDictionary obj, StringBuilder json)
		{
			var tabs = new string('\t', depth++);
			var line = "\r\n" + tabs;

			var success = true;

			json.Append((depth > 1 ? line : String.Empty) + '{');

			var first = true;

			var e = obj.GetEnumerator();

			while (e.MoveNext())
			{
				var key = e.Key?.ToString() ?? String.Empty;
				var value = e.Value;

				if (!first)
				{
					json.Append(", ");
				}

				json.Append(line + '\t');

				if (!SerializeString(key, json))
				{
					success = false;
					break;
				}

				json.Append(": ");

				if (!SerializeValue(depth, value, json))
				{
					success = false;
					break;
				}

				first = false;
			}

			if (success)
			{
				json.Append(line + '}');
			}

			return success;
		}

		private static bool DeserializeObject(string json, ref int index, out object value)
		{
			value = null;

			PeekToken(json, ref index); // {

			var obj = new Dictionary<string, object>();

			while (index < json.Length)
			{
				switch (PeekToken(json, index))
				{
					case JsonToken.None:
						return false;
					case JsonToken.Comma:
						PeekToken(json, ref index); // ,
						break;
					case JsonToken.ObjectClose:
					{
						PeekToken(json, ref index); // }
						value = obj;
					}
					return true;
					default:
					{
						if (!DeserializeString(json, ref index, out var name))
						{
							return false;
						}

						if (PeekToken(json, ref index) != JsonToken.Colon) // :
						{
							return false;
						}

						if (!DeserializeValue(json, ref index, out var val))
						{
							return false;
						}

						obj[(string)name] = val;
					}
					break;
				}
			}

			return false;
		}

		private static int SkipWhiteSpace(string json, int index)
		{
			while (index < json.Length)
			{
				if (Char.IsWhiteSpace(json[index]) || json[index] == '\r' || json[index] == '\n')
				{
					++index;
				}
				else
				{
					break;
				}
			}

			return index;
		}

		private static int PeekNumber(string json, int index, out int length)
		{
			length = 0;

			while (index < json.Length)
			{
				var c = json[index];

				if (c == '-' && length > 0)
				{
					break;
				}

				if (!Char.IsDigit(c) && c != '-')
				{
					if ((c == 'e' || c == 'E') && index + 1 < json.Length && json[index + 1] == '+')
					{
						length += 2;
						index += 2;
						continue;
					}

					if (c == '.' && index + 1 < json.Length && Char.IsDigit(json[index + 1]))
					{
						length += 2;
						index += 2;
						continue;
					}

					break;
				}

				++length;
				++index;
			}

			return index - length;
		}

		private static JsonToken PeekToken(string json, int index)
		{
			return PeekToken(json, ref index);
		}

		private static JsonToken PeekToken(string json, ref int index)
		{
			index = SkipWhiteSpace(json, index);

			if (index >= json.Length)
			{
				return JsonToken.None;
			}

			var c = json[index++];

			switch (c)
			{
				case '{':
					return JsonToken.ObjectOpen;
				case '}':
					return JsonToken.ObjectClose;
				case '[':
					return JsonToken.ArrayOpen;
				case ']':
					return JsonToken.ArrayClose;
				case ',':
					return JsonToken.Comma;
				case '"':
				case '\'':
					return JsonToken.String;
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return JsonToken.Number;
				case ':':
					return JsonToken.Colon;
			}

			--index;

			var length = json.Length - index;

			// false
			if (length >= 5 && json.IndexOf("false", index, StringComparison.OrdinalIgnoreCase) == index)
			{
				index += 5;
				return JsonToken.False;
			}

			// true
			if (length >= 4 && json.IndexOf("true", index, StringComparison.OrdinalIgnoreCase) == index)
			{
				index += 4;
				return JsonToken.True;
			}

			// null
			if (length >= 4 && json.IndexOf("null", index, StringComparison.OrdinalIgnoreCase) == index)
			{
				index += 4;
				return JsonToken.Null;
			}

			return JsonToken.None;
		}
	}
}
