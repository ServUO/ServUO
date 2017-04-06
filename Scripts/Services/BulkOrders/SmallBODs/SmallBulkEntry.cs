using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Server.Engines.BulkOrders
{
    public class SmallBulkEntry
    {
        private static Hashtable m_Cache;
        private readonly Type m_Type;
        private readonly int m_Number;
        private readonly int m_Graphic;
        private readonly int m_Hue;

        public SmallBulkEntry(Type type, int number, int graphic, int hue)
        {
            this.m_Type = type;
            this.m_Number = number;
            this.m_Graphic = graphic;
            this.m_Hue = hue;
        }

        public static SmallBulkEntry[] BlacksmithWeapons
        {
            get
            {
                return GetEntries("Blacksmith", "weapons");
            }
        }
        public static SmallBulkEntry[] BlacksmithArmor
        {
            get
            {
                return GetEntries("Blacksmith", "armor");
            }
        }
        public static SmallBulkEntry[] TailorCloth
        {
            get
            {
                return GetEntries("Tailoring", "cloth");
            }
        }
        public static SmallBulkEntry[] TailorLeather
        {
            get
            {
                return GetEntries("Tailoring", "leather");
            }
        }
        #region Publish 95 BODs
        public static SmallBulkEntry[] TinkeringSmalls
        {
            get
            {
                return GetEntries("Tinkering", "smalls");
            }
        }
        public static SmallBulkEntry[] TinkeringSmallsRegular
        {
            get
            {
                return GetEntries("Tinkering", "smallsregular");
            }
        }
        public static SmallBulkEntry[] CarpentrySmalls
        {
            get
            {
                return GetEntries("Carpentry", "smalls");
            }
        }
        public static SmallBulkEntry[] InscriptionSmalls
        {
            get
            {
                return GetEntries("Inscription", "smalls");
            }
        }
        public static SmallBulkEntry[] CookingSmalls
        {
            get
            {
                return GetEntries("Cooking", "smalls");
            }
        }
        public static SmallBulkEntry[] CookingSmallsRegular
        {
            get
            {
                return GetEntries("Cooking", "smallsregular");
            }
        }
        public static SmallBulkEntry[] FletchingSmalls
        {
            get
            {
                return GetEntries("Fletching", "smalls");
            }
        }
        public static SmallBulkEntry[] FletchingSmallsRegular
        {
            get
            {
                return GetEntries("Fletching", "smallsregular");
            }
        }
        public static SmallBulkEntry[] AlchemySmalls
        {
            get
            {
                return GetEntries("Alchemy", "smalls");
            }
        }
        #endregion
        public Type Type
        {
            get
            {
                return this.m_Type;
            }
        }
        public int Number
        {
            get
            {
                return this.m_Number;
            }
        }
        public int Graphic
        {
            get
            {
                return this.m_Graphic;
            }
        }
        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
        }
        public static SmallBulkEntry[] GetEntries(string type, string name)
        {
            if (m_Cache == null)
                m_Cache = new Hashtable();

            Hashtable table = (Hashtable)m_Cache[type];

            if (table == null)
                m_Cache[type] = table = new Hashtable();

            SmallBulkEntry[] entries = (SmallBulkEntry[])table[name];

            if (entries == null)
                table[name] = entries = LoadEntries(type, name);

            return entries;
        }

        public static SmallBulkEntry[] LoadEntries(string type, string name)
        {
            return LoadEntries(String.Format("Data/Bulk Orders/{0}/{1}.cfg", type, name));
        }

        public static SmallBulkEntry[] LoadEntries(string path)
        {
            path = Path.Combine(Core.BaseDirectory, path);

            List<SmallBulkEntry> list = new List<SmallBulkEntry>();

            if (File.Exists(path))
            {
                using (StreamReader ip = new StreamReader(path))
                {
                    string line;

                    while ((line = ip.ReadLine()) != null)
                    {
                        /* arg 1 - Type
                         * arg 2 - ItemID
                         * arg 3 - Cliloc
                         * arg 4 - hue
                         */

                        if (line.Length == 0 || line.StartsWith("#"))
                            continue;

                        try
                        {
                            string[] split = line.Split(',');

                            if (split.Length <= 2)
                            {
                                Type type = ScriptCompiler.FindTypeByName(split[0]);
                                int graphic = Utility.ToInt32(split[1]);

                                if (type != null && graphic > 0)
                                {
                                    list.Add(new SmallBulkEntry(type, graphic < 0x4000 ? 1020000 + graphic : 1078872 + graphic, graphic, 0));
                                }
                                else
                                {
                                    Console.WriteLine("Error Loading BOD Entry at {2}, [Type: {0}], [graphic: {1}]", split[0], graphic.ToString(), path);
                                }
                            }
                            else if (split.Length >= 3)
                            {
                                int name, hue;

                                Type type = ScriptCompiler.FindTypeByName(split[0]);
                                int graphic = Utility.ToInt32(split[1]);

                                name = Utility.ToInt32(split[2]);

                                if (split.Length >= 4)
                                {
                                    hue = Utility.ToInt32(split[3]);
                                }
                                else
                                {
                                    hue = 0;
                                }

                                if (type != null && graphic > 0)
                                {
                                    list.Add(new SmallBulkEntry(type, name, graphic, hue));
                                }
                                else
                                {
                                    Console.WriteLine("Error Loading BOD Entry at {2}, [Type: {0}], [graphic: {1}]", split[0], graphic.ToString(), path);
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }

            return list.ToArray();
        }
    }
}
