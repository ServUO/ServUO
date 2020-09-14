using Server.Gumps;
using System;
using System.Globalization;

namespace Server
{
    [Parsable]
    public class TextDefinition
    {
        private readonly int m_Number;
        private readonly string m_String;
        public TextDefinition()
            : this(0, null)
        {
        }

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
            m_Number = number;
            m_String = text;
        }

        public int Number => m_Number;

        public string String => m_String;

        public static void Serialize(GenericWriter writer, TextDefinition def)
        {
            if (def == null)
            {
                writer.WriteEncodedInt(3);
            }
            else if (def.m_Number > 0)
            {
                writer.WriteEncodedInt(1);
                writer.WriteEncodedInt(def.m_Number);
            }
            else if (def.m_String != null)
            {
                writer.WriteEncodedInt(2);
                writer.Write(def.m_String);
            }
            else
            {
                writer.WriteEncodedInt(0);
            }
        }

        public static TextDefinition Deserialize(GenericReader reader)
        {
            int type = reader.ReadEncodedInt();

            switch (type)
            {
                case 0:
                    return new TextDefinition();
                case 1:
                    return new TextDefinition(reader.ReadEncodedInt());
                case 2:
                    return new TextDefinition(reader.ReadString());
            }

            return null;
        }

        public static void AddTo(ObjectPropertyList list, TextDefinition def)
        {
            if (def == null)
                return;

            if (def.m_Number > 0)
                list.Add(def.m_Number);
            else if (def.m_String != null)
                list.Add(def.m_String);
        }

        public static void AddHtmlText(Gump g, int x, int y, int width, int height, TextDefinition def, bool back, bool scroll, int numberColor, int stringColor)
        {
            if (def == null)
                return;

            if (def.m_Number > 0)
            {
                if (numberColor >= 0)
                    g.AddHtmlLocalized(x, y, width, height, def.m_Number, numberColor, back, scroll);
                else
                    g.AddHtmlLocalized(x, y, width, height, def.m_Number, back, scroll);
            }
            else if (def.m_String != null)
            {
                if (stringColor >= 0)
                    g.AddHtml(x, y, width, height, string.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", stringColor, def.m_String), back, scroll);
                else
                    g.AddHtml(x, y, width, height, def.m_String, back, scroll);
            }
        }

        public static void AddHtmlText(Gump g, int x, int y, int width, int height, TextDefinition def, bool back, bool scroll)
        {
            AddHtmlText(g, x, y, width, height, def, back, scroll, -1, -1);
        }

        public static void AddTooltip(Gump g, TextDefinition def)
        {
            if (def == null)
                return;

            if (def.Number > 0)
                g.AddTooltip(def.Number);
            else if (def.String != null)
                g.AddTooltip(def.String);
        }

        public static void SendMessageTo(Mobile m, TextDefinition def)
        {
            if (def == null)
                return;

            if (def.m_Number > 0)
                m.SendLocalizedMessage(def.m_Number);
            else if (def.m_String != null)
                m.SendMessage(def.m_String);
        }

        public static TextDefinition Parse(string value)
        {
            if (value == null)
                return null;

            int i;
            bool isInteger;

            if (value.StartsWith("0x"))
                isInteger = int.TryParse(value.Substring(2), NumberStyles.HexNumber, null, out i);
            else
                isInteger = int.TryParse(value, out i);

            if (isInteger)
                return new TextDefinition(i);
            else
                return new TextDefinition(value);
        }

        public override string ToString()
        {
            if (m_Number > 0)
                return string.Concat("#", m_Number.ToString());
            else if (m_String != null)
                return m_String;

            return "";
        }

        public string Format(bool propsGump)
        {
            if (m_Number > 0)
                return string.Format("{0} (0x{0:X})", m_Number);
            else if (m_String != null)
                return string.Format("\"{0}\"", m_String);

            return propsGump ? "-empty-" : "empty";
        }

        public string GetValue()
        {
            if (m_Number > 0)
                return m_Number.ToString();
            else if (m_String != null)
                return m_String;

            return "";
        }

        public static implicit operator TextDefinition(int v)
        {
            return new TextDefinition(v);
        }

        public static implicit operator TextDefinition(string s)
        {
            return new TextDefinition(s);
        }

        public static implicit operator int(TextDefinition m)
        {
            if (m == null)
                return 0;

            return m.m_Number;
        }

        public static implicit operator string(TextDefinition m)
        {
            if (m == null)
                return null;

            return m.m_String;
        }
    }
}