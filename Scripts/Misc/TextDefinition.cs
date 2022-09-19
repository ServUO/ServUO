using System;
using System.Globalization;

using Server.Gumps;

namespace Server
{
	[Parsable, PropertyObject]
	public readonly struct TextDefinition : IComparable, IComparable<TextDefinition>, IComparable<int>, IComparable<string>, IEquatable<TextDefinition>, IEquatable<int>, IEquatable<string>
	{
		public static readonly TextDefinition Empty = default;

		public static void AddTo(ObjectPropertyList list, TextDefinition def)
		{
			if (def.IsEmpty)
			{
				return;
			}

			if (def.Number > 0)
			{
				list.Add(def.Number);
			}
			else if (def.String != null)
			{
				list.Add(def.String);
			}
		}

		public static void AddHtmlText(Gump g, int x, int y, int width, int height, TextDefinition def, bool back, bool scroll, int numberColor, int stringColor)
		{
			if (def.IsEmpty)
			{
				return;
			}

			if (def.Number > 0)
			{
				if (numberColor >= 0)
				{
					g.AddHtmlLocalized(x, y, width, height, def.Number, numberColor, back, scroll);
				}
				else
				{
					g.AddHtmlLocalized(x, y, width, height, def.Number, back, scroll);
				}
			}
			else if (def.String != null)
			{
				if (stringColor >= 0)
				{
					g.AddHtml(x, y, width, height, $"<BASEFONT COLOR=#{stringColor:X6}>{def.String}</BASEFONT>", back, scroll);
				}
				else
				{
					g.AddHtml(x, y, width, height, def.String, back, scroll);
				}
			}
		}

		public static void AddHtmlText(Gump g, int x, int y, int width, int height, TextDefinition def, bool back, bool scroll)
		{
			AddHtmlText(g, x, y, width, height, def, back, scroll, -1, -1);
		}

		public static void AddTooltip(Gump gump, TextDefinition def)
		{
			if (def.IsEmpty)
			{
				return;
			}

			if (def.Number > 0)
			{
				gump.AddTooltip(def.Number);
			}
			else if (def.String != null)
			{
				gump.AddTooltip(def.String);
			}
		}

		public static void SendMessageTo(Mobile m, TextDefinition def)
		{
			if (def.IsEmpty)
			{
				return;
			}

			if (def.Number > 0)
			{
				if (!String.IsNullOrWhiteSpace(def.String))
				{
					m.SendLocalizedMessage(def.Number, def.String);
				}
				else
				{
					m.SendLocalizedMessage(def.Number);
				}
			}
			else if (!String.IsNullOrWhiteSpace(def.String))
			{
				m.SendMessage(def.String);
			}
		}

		public static TextDefinition Parse(string value)
		{
			if (value == null)
			{
				return Empty;
			}

			if (value.Length > 0)
			{
				int number;
				bool integer;

				if (value.StartsWith("0x"))
				{
					integer = Int32.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out number);
				}
				else if (value.StartsWith("#"))
				{
					integer = Int32.TryParse(value.Substring(1), out number);
				}
				else
				{
					integer = Int32.TryParse(value, out number);
				}

				if (integer)
				{
					return number;
				}

				return value;
			}

			return Empty;
		}

		public static bool TryParse(string input, out TextDefinition value)
		{
			try
			{
				value = Parse(input);

				return true;
			}
			catch
			{
				value = Empty;

				return false;
			}
		}

		public static void Serialize(GenericWriter writer, TextDefinition def)
		{
			if (def.Number > 0)
			{
				writer.WriteEncodedInt(1);
				writer.WriteEncodedInt(def.Number);
			}
			else if (def.String != null)
			{
				writer.WriteEncodedInt(2);
				writer.Write(def.String);
			}
			else
			{
				writer.WriteEncodedInt(0);
			}
		}

		public static TextDefinition Deserialize(GenericReader reader)
		{
			switch (reader.ReadEncodedInt())
			{
				case 1: return reader.ReadEncodedInt();
				case 2: return reader.ReadString();
			}

			return Empty;
		}

		[CommandProperty(AccessLevel.Counselor, true)]
		public int Number { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public string String { get; }

		[CommandProperty(AccessLevel.Counselor, true)]
		public bool IsEmpty => Number == 0 && String == null;

		public TextDefinition(int number)
			: this(number, null)
		{
		}

		public TextDefinition(string text)
			: this(0, text)
		{
		}

		public TextDefinition(int number, string text)
		{
			Number = number;
			String = text;
		}

		public string Format(bool propsGump)
		{
			if (Number > 0)
			{
				return $"{Number} (0x{Number:X})";
			}

			if (String != null)
			{
				return $"\"{String}\"";
			}

			return propsGump ? "-empty-" : "empty";
		}

		public override string ToString()
		{
			if (Number > 0)
			{
				return $"#{Number}";
			}

			if (String != null)
			{
				return String;
			}

			return String.Empty;
		}

		public string GetValue()
		{
			if (Number > 0)
			{
				return Number.ToString();
			}

			if (String != null)
			{
				return String;
			}

			return String.Empty;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hash = 0;

				if (Number > 0)
				{
					hash = (hash * 397) ^ Number.GetHashCode();
				}

				if (String != null)
				{
					hash = (hash * 397) ^ String.GetHashCode();
				}

				return hash;
			}
		}

		public int CompareTo(int num)
		{
			return Number.CompareTo(num);
		}

		public int CompareTo(string str)
		{
			return Insensitive.Compare(String, str);
		}

		public int CompareTo(TextDefinition other)
		{
			var res = IsEmpty.CompareTo(other.IsEmpty);

			if (res == 0)
			{
				res = CompareTo(other.Number);

				if (res == 0)
				{
					res = CompareTo(other.String);
				}
			}

			return res;
		}

		public int CompareTo(object obj)
		{
			if (obj is TextDefinition def)
			{
				return CompareTo(def);
			}

			if (obj is int num)
			{
				return Number.CompareTo(num);
			}

			if (obj is string str)
			{
				return Insensitive.Compare(String, str);
			}

			return 0;
		}

		public bool Equals(int num)
		{
			return Number == num;
		}

		public bool Equals(string str)
		{
			return Insensitive.Equals(String, str);
		}

		public bool Equals(TextDefinition def)
		{
			if (Number > 0 && def.Number > 0)
			{
				return Equals(def.Number);
			}

			if (String != null && def.String != null)
			{
				return Equals(def.String);
			}

			return Equals(IsEmpty, def.IsEmpty);
		}

		public override bool Equals(object obj)
		{
			if (obj is TextDefinition d)
			{
				return Equals(d);
			}

			if (obj is int num)
			{
				return Equals(num);
			}

			if (obj is string str)
			{
				return Equals(str);
			}

			return false;
		}

		public static implicit operator TextDefinition(int num)
		{
			return new TextDefinition(num);
		}

		public static implicit operator TextDefinition(string str)
		{
			return new TextDefinition(str);
		}

		public static implicit operator int(TextDefinition def)
		{
			return def.Number;
		}

		public static implicit operator string(TextDefinition def)
		{
			return def.String;
		}
	}
}
