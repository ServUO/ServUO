using System;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using Server.Items;

namespace Server.Commands
{
    public class Categorization
    {
        private static readonly Type typeofItem = typeof(Item);
        private static readonly Type typeofMobile = typeof(Mobile);
        private static readonly Type typeofConstructable = typeof(ConstructableAttribute);
        private static CategoryEntry m_RootItems, m_RootMobiles;
        public static CategoryEntry Items
        {
            get
            {
                if (m_RootItems == null)
                    Load();

                return m_RootItems;
            }
        }
        public static CategoryEntry Mobiles
        {
            get
            {
                if (m_RootMobiles == null)
                    Load();

                return m_RootMobiles;
            }
        }
        public static void Initialize()
        {
            CommandSystem.Register("RebuildCategorization", AccessLevel.Administrator, new CommandEventHandler(RebuildCategorization_OnCommand));
        }

        [Usage("RebuildCategorization")]
        [Description("Rebuilds the categorization data file used by the Add command.")]
        public static void RebuildCategorization_OnCommand(CommandEventArgs e)
        {
            CategoryEntry root = new CategoryEntry(null, "Add Menu", new CategoryEntry[] { Items, Mobiles });

            Export(root, "Data/objects.xml", "Objects");

            e.Mobile.SendMessage("Categorization menu rebuilt.");
        }

        public static void RecurseFindCategories(CategoryEntry ce, ArrayList list)
        {
            list.Add(ce);

            for (int i = 0; i < ce.SubCategories.Length; ++i)
                RecurseFindCategories(ce.SubCategories[i], list);
        }

        public static void Export(CategoryEntry ce, string fileName, string title)
        {
            XmlTextWriter xml = new XmlTextWriter(fileName, System.Text.Encoding.UTF8);

            xml.Indentation = 1;
            xml.IndentChar = '\t';
            xml.Formatting = Formatting.Indented;

            xml.WriteStartDocument(true);

            RecurseExport(xml, ce);

            xml.Flush();
            xml.Close();
        }

        public static void RecurseExport(XmlTextWriter xml, CategoryEntry ce)
        {
            xml.WriteStartElement("category");

            xml.WriteAttributeString("title", ce.Title);

            ArrayList subCats = new ArrayList(ce.SubCategories);

            subCats.Sort(new CategorySorter());

            for (int i = 0; i < subCats.Count; ++i)
                RecurseExport(xml, (CategoryEntry)subCats[i]);

            ce.Matched.Sort(new CategorySorter());

            for (int i = 0; i < ce.Matched.Count; ++i)
            {
                CategoryTypeEntry cte = (CategoryTypeEntry)ce.Matched[i];

                xml.WriteStartElement("object");

                xml.WriteAttributeString("type", cte.Type.ToString());

                object obj = cte.Object;

                if (obj is Item)
                {
                    Item item = (Item)obj;

                    int itemID = item.ItemID;

                    if (item is BaseAddon && ((BaseAddon)item).Components.Count == 1)
                        itemID = ((AddonComponent)(((BaseAddon)item).Components[0])).ItemID;

                    if (itemID > TileData.MaxItemValue)
                        itemID = 1;

                    xml.WriteAttributeString("gfx", XmlConvert.ToString(itemID));

                    int hue = item.Hue & 0x7FFF;

                    if ((hue & 0x4000) != 0)
                        hue = 0;

                    if (hue != 0)
                        xml.WriteAttributeString("hue", XmlConvert.ToString(hue));

                    item.Delete();
                }
                else if (obj is Mobile)
                {
                    Mobile mob = (Mobile)obj;

                    int itemID = ShrinkTable.Lookup(mob, 1);

                    xml.WriteAttributeString("gfx", XmlConvert.ToString(itemID));

                    int hue = mob.Hue & 0x7FFF;

                    if ((hue & 0x4000) != 0)
                        hue = 0;

                    if (hue != 0)
                        xml.WriteAttributeString("hue", XmlConvert.ToString(hue));

                    mob.Delete();
                }

                xml.WriteEndElement();
            }

            xml.WriteEndElement();
        }

        public static void Load()
        {
            ArrayList types = new ArrayList();

            AddTypes(Core.Assembly, types);

            for (int i = 0; i < ScriptCompiler.Assemblies.Length; ++i)
                AddTypes(ScriptCompiler.Assemblies[i], types);

            m_RootItems = Load(types, "Data/items.cfg");
            m_RootMobiles = Load(types, "Data/mobiles.cfg");
        }

        private static CategoryEntry Load(ArrayList types, string config)
        {
            CategoryLine[] lines = CategoryLine.Load(config);

            if (lines.Length > 0)
            {
                int index = 0;
                CategoryEntry root = new CategoryEntry(null, lines, ref index);

                Fill(root, types);

                return root;
            }

            return new CategoryEntry();
        }

        private static bool IsConstructable(Type type)
        {
            if (!type.IsSubclassOf(typeofItem) && !type.IsSubclassOf(typeofMobile))
                return false;

            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);

            return (ctor != null && ctor.IsDefined(typeofConstructable, false));
        }

        private static void AddTypes(Assembly asm, ArrayList types)
        {
            Type[] allTypes = asm.GetTypes();

            for (int i = 0; i < allTypes.Length; ++i)
            {
                Type type = allTypes[i];

                if (type.IsAbstract)
                    continue;

                if (IsConstructable(type))
                    types.Add(type);
            }
        }

        private static void Fill(CategoryEntry root, ArrayList list)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                Type type = (Type)list[i];
                CategoryEntry match = GetDeepestMatch(root, type);

                if (match == null)
                    continue;

                try
                {
                    match.Matched.Add(new CategoryTypeEntry(type));
                }
                catch
                {
                }
            }
        }

        private static CategoryEntry GetDeepestMatch(CategoryEntry root, Type type)
        {
            if (!root.IsMatch(type))
                return null;

            for (int i = 0; i < root.SubCategories.Length; ++i)
            {
                CategoryEntry check = GetDeepestMatch(root.SubCategories[i], type);

                if (check != null)
                    return check;
            }

            return root;
        }
    }

    public class CategorySorter : IComparer
    {
        public int Compare(object x, object y)
        {
            string a = null, b = null;

            if (x is CategoryEntry)
                a = ((CategoryEntry)x).Title;
            else if (x is CategoryTypeEntry)
                a = ((CategoryTypeEntry)x).Type.Name;

            if (y is CategoryEntry)
                b = ((CategoryEntry)y).Title;
            else if (y is CategoryTypeEntry)
                b = ((CategoryTypeEntry)y).Type.Name;

            if (a == null && b == null)
                return 0;

            if (a == null)
                return 1;

            if (b == null)
                return -1;

            return a.CompareTo(b);
        }
    }

    public class CategoryTypeEntry
    {
        private readonly Type m_Type;
        private readonly object m_Object;
        public CategoryTypeEntry(Type type)
        {
            this.m_Type = type;
            this.m_Object = Activator.CreateInstance(type);
        }

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public object Object
        {
            get
            {
                return this.m_Object;
            }
        }
    }

    public class CategoryEntry
    {
        private readonly string m_Title;
        private readonly Type[] m_Matches;
        private readonly CategoryEntry[] m_SubCategories;
        private readonly CategoryEntry m_Parent;
        private readonly ArrayList m_Matched;
        public CategoryEntry()
        {
            this.m_Title = "(empty)";
            this.m_Matches = new Type[0];
            this.m_SubCategories = new CategoryEntry[0];
            this.m_Matched = new ArrayList();
        }

        public CategoryEntry(CategoryEntry parent, string title, CategoryEntry[] subCats)
        {
            this.m_Parent = parent;
            this.m_Title = title;
            this.m_SubCategories = subCats;
            this.m_Matches = new Type[0];
            this.m_Matched = new ArrayList();
        }

        public CategoryEntry(CategoryEntry parent, CategoryLine[] lines, ref int index)
        {
            this.m_Parent = parent;

            string text = lines[index].Text;

            int start = text.IndexOf('(');

            if (start < 0)
                throw new FormatException(String.Format("Input string not correctly formatted ('{0}')", text));

            this.m_Title = text.Substring(0, start).Trim();

            int end = text.IndexOf(')', ++start);

            if (end < start)
                throw new FormatException(String.Format("Input string not correctly formatted ('{0}')", text));

            text = text.Substring(start, end - start);
            string[] split = text.Split(';');

            ArrayList list = new ArrayList();

            for (int i = 0; i < split.Length; ++i)
            {
                Type type = ScriptCompiler.FindTypeByName(split[i].Trim());

                if (type == null)
                    Console.WriteLine("Match type not found ('{0}')", split[i].Trim());
                else
                    list.Add(type);
            }

            this.m_Matches = (Type[])list.ToArray(typeof(Type));
            list.Clear();

            int ourIndentation = lines[index].Indentation;

            ++index;

            while (index < lines.Length && lines[index].Indentation > ourIndentation)
                list.Add(new CategoryEntry(this, lines, ref index));

            this.m_SubCategories = (CategoryEntry[])list.ToArray(typeof(CategoryEntry));
            list.Clear();

            this.m_Matched = list;
        }

        public string Title
        {
            get
            {
                return this.m_Title;
            }
        }
        public Type[] Matches
        {
            get
            {
                return this.m_Matches;
            }
        }
        public CategoryEntry Parent
        {
            get
            {
                return this.m_Parent;
            }
        }
        public CategoryEntry[] SubCategories
        {
            get
            {
                return this.m_SubCategories;
            }
        }
        public ArrayList Matched
        {
            get
            {
                return this.m_Matched;
            }
        }
        public bool IsMatch(Type type)
        {
            bool isMatch = false;

            for (int i = 0; !isMatch && i < this.m_Matches.Length; ++i)
                isMatch = (type == this.m_Matches[i] || type.IsSubclassOf(this.m_Matches[i]));

            return isMatch;
        }
    }

    public class CategoryLine
    {
        private readonly int m_Indentation;
        private readonly string m_Text;
        public CategoryLine(string input)
        {
            int index;

            for (index = 0; index < input.Length; ++index)
            {
                if (Char.IsLetter(input, index))
                    break;
            }

            if (index >= input.Length)
                throw new FormatException(String.Format("Input string not correctly formatted ('{0}')", input));

            this.m_Indentation = index;
            this.m_Text = input.Substring(index);
        }

        public int Indentation
        {
            get
            {
                return this.m_Indentation;
            }
        }
        public string Text
        {
            get
            {
                return this.m_Text;
            }
        }
        public static CategoryLine[] Load(string path)
        {
            ArrayList list = new ArrayList();

            if (File.Exists(path))
            {
                using (StreamReader ip = new StreamReader(path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                        list.Add(new CategoryLine(line));
                }
            }

            return (CategoryLine[])list.ToArray(typeof(CategoryLine));
        }
    }
}