using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Server
{
    public class StringList
    {
        public static StringList Localization { get; }

        static StringList()
        {
            Localization = new StringList();
        }

        public List<StringEntry> Entries { get; set; }

        public Dictionary<int, string> StringTable;
        private readonly Dictionary<int, StringEntry> EntryTable;

        public string Language { get; private set; }

        public string this[int number]
        {
            get
            {
                if (StringTable.ContainsKey(number))
                    return StringTable[number];
                else
                    return null;
            }
        }

        public StringList()
            : this("enu")
        {
        }

        public StringList(string language)
            : this(language, true)
        {
        }

        public StringList(string language, bool format)
        {
            Language = language;            

            string path = Core.FindDataFile(string.Format("Cliloc.{0}", language));

            if (path == null)
            {
                Console.WriteLine("Cliloc.{0} not found", language);
                Entries = new List<StringEntry>(0);
                return;
            }

            StringTable = new Dictionary<int, string>();
            EntryTable = new Dictionary<int, StringEntry>();
            Entries = new List<StringEntry>();

            using (BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                byte[] buffer = new byte[1024];

                bin.ReadInt32();
                bin.ReadInt16();

                while (bin.BaseStream.Length != bin.BaseStream.Position)
                {
                    int number = bin.ReadInt32();
                    bin.ReadByte(); // flag
                    int length = bin.ReadInt16();

                    if (length > buffer.Length)
                        buffer = new byte[(length + 1023) & ~1023];

                    bin.Read(buffer, 0, length);
                    string text = Encoding.UTF8.GetString(buffer, 0, length);

                    StringEntry se = new StringEntry(number, text);
                    Entries.Add(se);

                    StringTable[number] = text;
                    EntryTable[number] = se;
                }
            }
        }

        //C# argument support
        public static Regex FormatExpression = new Regex(@"~(\d)+_.*?~", RegexOptions.IgnoreCase);

        public static string MatchComparison(Match m)
        {
            return "{" + (Utility.ToInt32(m.Groups[1].Value) - 1) + "}";
        }

        public static string FormatArguments(string entry)
        {
            return FormatExpression.Replace(entry, new MatchEvaluator(MatchComparison));
        }

        //UO tabbed argument conversion
        public static string CombineArguments(string str, string args)
        {
            if (string.IsNullOrEmpty(args))
                return str;
            else
                return CombineArguments(str, args.Split(new char[] { '\t' }));
        }

        public static string CombineArguments(string str, params object[] args)
        {
            return string.Format(str, args);
        }

        public static string CombineArguments(int number, string args)
        {
            return CombineArguments(number, args.Split(new char[] { '\t' }));
        }

        public static string CombineArguments(int number, params object[] args)
        {
            return string.Format(Localization[number], args);
        }

        public StringEntry GetEntry(int number)
        {
            if (EntryTable == null || !EntryTable.ContainsKey(number))
            {
                return null;
            }

            return EntryTable[number];
        }

        public string GetString(int number)
        {
            if (StringTable == null || !StringTable.ContainsKey(number))
            {
                return null;
            }

            return StringTable[number];
        }
    }

    public class StringEntry
    {
        public int Number { get; private set; }
        public string Text { get; private set; }

        public StringEntry(int number, string text)
        {
            Number = number;
            Text = text;
        }

        private string m_FmtTxt;

        private static readonly Regex m_RegEx = new Regex(
            @"~(\d+)[_\w]+~",
            RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant);

        private static readonly object[] m_Args = new object[] { "", "", "", "", "", "", "", "", "", "", "" };

        public string Format(params object[] args)
        {
            if (m_FmtTxt == null)
            {
                m_FmtTxt = m_RegEx.Replace(Text, @"{$1}");
            }
            for (int i = 0; i < args.Length && i < 10; i++)
            {
                m_Args[i + 1] = args[i];
            }
            return string.Format(m_FmtTxt, m_Args);
        }
    }
}
