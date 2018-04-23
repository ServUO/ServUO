using System.Collections.Generic;
using Server.Gumps;

namespace Server.ACC.YS
{
    public class YardGumpEntry
    {
        private int m_ItemID;
        public int ItemID
        {
            get { return m_ItemID; }
            set { m_ItemID = value; }
        }

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private int m_Price;
        public int Price
        {
            get { return m_Price; }
            set { m_Price = value; }
        }

        public YardGumpEntry(int itemID, string name, int price)
        {
            ItemID = itemID;
            Price = price;
            Name = name;
        }

        public void AppendToGump(Gump g, int x, int y)
        {
            g.AddLabel(x, y, 1150, Name);
            g.AddButton(x - 18, y + 5, 5032, 2361, ItemID, GumpButtonType.Reply, 0);
        }
    }

    public class YardGumpCategory
    {
        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { m_Name = value; }
        }

        private List<Dictionary<int, YardGumpEntry>> m_Pages;
        public List<Dictionary<int, YardGumpEntry>> Pages
        {
            get
            {
                if (m_Pages == null)
                {
                    m_Pages = new List<Dictionary<int, YardGumpEntry>>();
                }
                return m_Pages;
            }
            set { m_Pages = value; }
        }

        public YardGumpCategory(string name)
        {
            Name = name;
            Pages = new List<Dictionary<int, YardGumpEntry>>();
        }

        public void AddEntry(YardGumpEntry entry)
        {
            if (Pages.Count == 0)
            {
                Pages.Add(new Dictionary<int, YardGumpEntry>());
                Pages[0].Add(entry.ItemID, entry);
            }
            else
            {
                if (Pages[Pages.Count - 1].Count >= 24)
                {
                    Pages.Add(new Dictionary<int, YardGumpEntry>());
                }

                Pages[Pages.Count - 1].Add(entry.ItemID, entry);
            }
        }

        public YardGumpEntry GetEntry(int itemID)
        {
            if (Pages.Count == 0)
            {
                return null;
            }

            foreach (Dictionary<int, YardGumpEntry> item in Pages)
            {
                if (item.ContainsKey(itemID) && item[itemID] != null)
                {
                    return item[itemID];
                }
            }

            return null;
        }
    }

    class YardRegistry
    {
        public static Dictionary<int, List<YardMultiInfo>> YardMultiIDs;

        /* This dictionary keeps track of the directions for each primary stair ID
         * When a YardStair is double clicked, it changes the ItemID to the next in the list
         * which changes the direction of the stair.
         */
        public static Dictionary<int, int[]> YardStairIDGroups;

        public static Dictionary<string, YardGumpCategory> Categories = new Dictionary<string, YardGumpCategory>();

        public static void RegisterCategory(string category)
        {
            if (Categories == null)
            {
                Categories = new Dictionary<string, YardGumpCategory>();
            }

            if (Categories.ContainsKey(category))
            {
                return;
            }

            Categories.Add(category, new YardGumpCategory(category));
        }

        public static YardGumpCategory GetRegisteredCategory(string category)
        {
            if (!Categories.ContainsKey(category))
            {
                RegisterCategory(category);
            }

            return Categories[category];
        }

        public static void RegisterEntry(string categoryName, int itemID, string name, int price)
        {
            YardGumpCategory category = GetRegisteredCategory(categoryName);
            if (category == null)
            {
                return;
            }

            YardGumpEntry entry = new YardGumpEntry(itemID, name, price);

            category.AddEntry(entry);
        }

        public static void Configure()
        {
            RegisterItems();
            RegisterMultis();
            RegisterStairs();
        }

        public static void RegisterItems()
        {
            //Each category will hold 24 entries per page in order of their registration.

            //Fences & Gates
            //For adding new gates/doors, please see YardGate.cs in the Items folder for examples.
            RegisterEntry("Fences & Gates", 2081, "T Iron N", 150);
            RegisterEntry("Fences & Gates", 2083, "T Iron E", 150);
            RegisterEntry("Fences & Gates", 2082, "T Iron C", 150);
            RegisterEntry("Fences & Gates", 2084, "T Iron Gate SDW", 150);
            RegisterEntry("Fences & Gates", 2086, "T Iron Gate SDE", 150);
            RegisterEntry("Fences & Gates", 2088, "T Iron Gate SUW", 150);
            RegisterEntry("Fences & Gates", 2090, "T Iron Gate SUE", 150);
            RegisterEntry("Fences & Gates", 2092, "T Iron Gate EUE", 150);
            RegisterEntry("Fences & Gates", 2094, "T Iron Gate EDE", 150);
            RegisterEntry("Fences & Gates", 2096, "T Iron Gate EDW", 150);
            RegisterEntry("Fences & Gates", 2098, "T Iron Gate EUW", 150);
            RegisterEntry("Fences & Gates", 2121, "S Iron N", 150);
            RegisterEntry("Fences & Gates", 2123, "S Iron E", 150);
            RegisterEntry("Fences & Gates", 2122, "S Iron C", 150);
            RegisterEntry("Fences & Gates", 2124, "S Iron Gate SDW", 150);
            RegisterEntry("Fences & Gates", 2126, "S Iron Gate SDE", 150);
            RegisterEntry("Fences & Gates", 2128, "S Iron Gate SUW", 150);
            RegisterEntry("Fences & Gates", 2130, "S Iron Gate SUE", 150);
            RegisterEntry("Fences & Gates", 2132, "S Iron Gate EUE", 150);
            RegisterEntry("Fences & Gates", 2134, "S Iron Gate EDE", 150);
            RegisterEntry("Fences & Gates", 2136, "S Iron Gate EDW", 150);
            RegisterEntry("Fences & Gates", 2138, "S Iron Gate EUW", 150);
            RegisterEntry("Fences & Gates", 2103, "Wood 1N", 150);
            RegisterEntry("Fences & Gates", 2102, "Wood 1E", 150);
            RegisterEntry("Fences & Gates", 2101, "Wood 1C", 150);
            RegisterEntry("Fences & Gates", 2104, "Wood 1T", 150);
            RegisterEntry("Fences & Gates", 2142, "Wood 2N", 150);
            RegisterEntry("Fences & Gates", 2141, "Wood 2E", 150);
            RegisterEntry("Fences & Gates", 2140, "Wood 2C", 150);
            RegisterEntry("Fences & Gates", 2143, "Wood 2T", 150);
            RegisterEntry("Fences & Gates", 2145, "Wood 2NLink", 150);
            RegisterEntry("Fences & Gates", 2144, "Wood 2ELink", 150);
            RegisterEntry("Fences & Gates", 2148, "Wood 3N", 150);
            RegisterEntry("Fences & Gates", 2147, "Wood 3E", 150);
            RegisterEntry("Fences & Gates", 2146, "Wood 3C", 150);
            RegisterEntry("Fences & Gates", 2149, "Wood 3T", 150);
            RegisterEntry("Fences & Gates", 2105, "L Wood Gate SDW", 150);
            RegisterEntry("Fences & Gates", 2107, "L Wood Gate SDE", 150);
            RegisterEntry("Fences & Gates", 2109, "L Wood Gate SUW", 150);
            RegisterEntry("Fences & Gates", 2111, "L Wood Gate SUE", 150);
            RegisterEntry("Fences & Gates", 2113, "L Wood Gate EUE", 150);
            RegisterEntry("Fences & Gates", 2115, "L Wood Gate EDE", 150);
            RegisterEntry("Fences & Gates", 2117, "L Wood Gate EDW", 150);
            RegisterEntry("Fences & Gates", 2119, "L Wood Gate EUW", 150); 
            RegisterEntry("Fences & Gates", 2150, "D Wood Gate SDW", 150);
            RegisterEntry("Fences & Gates", 2152, "D Wood Gate SDE", 150);
            RegisterEntry("Fences & Gates", 2154, "D Wood Gate SUW", 150);
            RegisterEntry("Fences & Gates", 2156, "D Wood Gate SUE", 150);
            RegisterEntry("Fences & Gates", 2158, "D Wood Gate EUE", 150);
            RegisterEntry("Fences & Gates", 2160, "D Wood Gate EDE", 150);
            RegisterEntry("Fences & Gates", 2162, "D Wood Gate EDW", 150);
            RegisterEntry("Fences & Gates", 2164, "D Wood Gate EUW", 150);

            //Ground"
            RegisterEntry("Ground", 3512, "Tall Bush", 150);
            RegisterEntry("Ground", 3215, "Short Bush 1", 150);
            RegisterEntry("Ground", 3217, "Short Bush 2", 150);
            RegisterEntry("Ground", 4963, "Rock 1", 50);
            RegisterEntry("Ground", 4964, "Rock 2", 50);
            RegisterEntry("Ground", 4965, "Rock 3", 50);
            RegisterEntry("Ground", 4966, "Rock 4", 50);
            RegisterEntry("Ground", 4967, "Rock 5", 50);
            RegisterEntry("Ground", 4968, "Rock 6", 50);
            RegisterEntry("Ground", 4969, "Rock 7", 50);
            RegisterEntry("Ground", 4970, "Rock 8", 50);
            RegisterEntry("Ground", 4971, "Rock 9", 50);
            RegisterEntry("Ground", 4972, "Rock 10", 50);
            RegisterEntry("Ground", 4973, "Rock 11", 50);
            RegisterEntry("Ground", 6001, "Rock 12", 50);
            RegisterEntry("Ground", 6002, "Rock 13", 50);
            RegisterEntry("Ground", 6003, "Rock 14", 50);
            RegisterEntry("Ground", 6004, "Rock 15", 50);
            RegisterEntry("Ground", 6005, "Rock 16", 50);
            RegisterEntry("Ground", 6006, "Rock 17", 50);
            RegisterEntry("Ground", 6007, "Rock 18", 50);
            RegisterEntry("Ground", 6008, "Rock 19", 50);
            RegisterEntry("Ground", 6009, "Rock 20", 50);
            RegisterEntry("Ground", 6010, "Rock 21", 50);
            RegisterEntry("Ground", 6011, "Rock 22", 50);
            RegisterEntry("Ground", 6012, "Rock 23", 50);
            RegisterEntry("Ground", 4534, "Rotating Rock", 100);

            //Stairs"
            RegisterEntry("Stairs", 1006, "S1 Block", 50);
            RegisterEntry("Stairs", 1007, "S1 Stair", 50);
            RegisterEntry("Stairs", 1011, "S1 Corner", 50);
            RegisterEntry("Stairs", 1015, "S1 Curved", 50);
            RegisterEntry("Stairs", 1019, "S1 Invert", 50);
            RegisterEntry("Stairs", 1023, "S1 ICurved", 50);
            RegisterEntry("Stairs", 1801, "M Block", 50);
            RegisterEntry("Stairs", 1802, "M Stair", 50);
            RegisterEntry("Stairs", 1806, "M Corner", 50);
            RegisterEntry("Stairs", 1810, "M Curved", 50);
            RegisterEntry("Stairs", 1814, "M Invert", 50);
            RegisterEntry("Stairs", 1818, "M ICurved", 50);
            RegisterEntry("Stairs", 1822, "S2 Block", 50);
            RegisterEntry("Stairs", 1823, "S2 Stair", 50);
            RegisterEntry("Stairs", 1866, "S2 Corner", 50);
            RegisterEntry("Stairs", 1870, "S2 Curved", 50);
            RegisterEntry("Stairs", 1952, "S2 Invert", 50);
            RegisterEntry("Stairs", 2015, "S2 ICurved", 50);
            RegisterEntry("Stairs", 1825, "W1 Block", 50);
            RegisterEntry("Stairs", 1826, "W1 Stair", 50);
            RegisterEntry("Stairs", 1830, "W1 Corner", 50);
            RegisterEntry("Stairs", 1834, "W1 Curved", 50);
            RegisterEntry("Stairs", 1838, "W1 Invert", 50);
            RegisterEntry("Stairs", 1842, "W1 ICurved", 50);

            //Stairs"
            RegisterEntry("Stairs", 1848, "W2 Block", 50);
            RegisterEntry("Stairs", 1849, "W2 Stair", 50);
            RegisterEntry("Stairs", 1853, "W2 Corner", 50);
            RegisterEntry("Stairs", 1861, "W2 Curved", 50);
            RegisterEntry("Stairs", 1857, "W2 Invert", 50);
            RegisterEntry("Stairs", 2170, "Wood Ramp", 50);
            RegisterEntry("Stairs", 1872, "S3 Block", 50);
            RegisterEntry("Stairs", 1873, "S3 Stair", 50);
            RegisterEntry("Stairs", 1877, "S3 Corner", 50);
            RegisterEntry("Stairs", 1881, "S3 Curved", 50);
            RegisterEntry("Stairs", 1885, "S3 Invert", 50);
            RegisterEntry("Stairs", 1889, "S3 ICurved", 50);
            RegisterEntry("Stairs", 1900, "SS1 Block", 50);
            RegisterEntry("Stairs", 1901, "SS1 Stair", 50);
            RegisterEntry("Stairs", 1905, "SS1 Corner", 50);
            RegisterEntry("Stairs", 1909, "SS1 Curved", 50);
            RegisterEntry("Stairs", 1913, "SS1 Invert", 50);
            RegisterEntry("Stairs", 1917, "SS1 ICurved", 50);
            RegisterEntry("Stairs", 1928, "S4 Block", 50);
            RegisterEntry("Stairs", 1929, "S4 Stair", 50);
            RegisterEntry("Stairs", 1933, "S4 Corner", 50);
            RegisterEntry("Stairs", 1937, "S4 Curved", 50);
            RegisterEntry("Stairs", 1941, "S4 Invert", 50);
            RegisterEntry("Stairs", 1945, "S4 ICurved", 50);

            //Stairs"
            RegisterEntry("Stairs", 1955, "S5 Block", 50);
            RegisterEntry("Stairs", 1956, "S5 Stair", 50);
            RegisterEntry("Stairs", 1960, "S5 Corner", 50);
            RegisterEntry("Stairs", 1964, "S5 Invert", 50);
            RegisterEntry("Stairs", 1978, "Red Block", 50);
            RegisterEntry("Stairs", 1979, "Red Stair", 50);
            RegisterEntry("Stairs", 1991, "Red Curved", 50);
            RegisterEntry("Stairs", 1981, "Platform 1", 50);
            RegisterEntry("Stairs", 1983, "Platform 2", 50);
            RegisterEntry("Stairs", 1987, "Platform 3", 50);
            RegisterEntry("Stairs", 1993, "Wood Planks 1", 50);
            RegisterEntry("Stairs", 1997, "Wood Planks 2", 50);
            RegisterEntry("Stairs", 1173, "Floor 1", 50);
            RegisterEntry("Stairs", 1193, "Floor 2", 50);
            RegisterEntry("Stairs", 1250, "Floor 3", 50);
            RegisterEntry("Stairs", 1289, "Floor 4", 50);
            RegisterEntry("Stairs", 1294, "Floor 5", 50);
            RegisterEntry("Stairs", 1301, "Floor 6", 50);
            RegisterEntry("Stairs", 1035, "Dirt 1", 50);
            RegisterEntry("Stairs", 1039, "Dirt 2", 50);
            RegisterEntry("Stairs", 1043, "Dirt 3", 50);
            RegisterEntry("Stairs", 1047, "Dirt 4", 50);
            RegisterEntry("Stairs", 1051, "Dirt 5", 50);
            RegisterEntry("Stairs", 12789, "Dirt 6", 50);

            //Water"
            RegisterEntry("Water", 13422, "Water East", 100);
            RegisterEntry("Water", 13460, "Water South", 100);
            RegisterEntry("Water", 6039, "Water Stagnant", 100);
            RegisterEntry("Water", 13493, "Whirlpool", 100);
            RegisterEntry("Water", 13555, "Waterfall E1", 100);
            RegisterEntry("Water", 13549, "Waterfall E2", 100);
            RegisterEntry("Water", 13561, "Waterfall E3", 100);
            RegisterEntry("Water", 13567, "Waterfall E4", 100);
            RegisterEntry("Water", 13573, "Waterfall E5", 100);
            RegisterEntry("Water", 13585, "Waterfall S1", 100);
            RegisterEntry("Water", 13579, "Waterfall S2", 100);
            RegisterEntry("Water", 13591, "Waterfall S3", 100);
            RegisterEntry("Water", 13597, "Waterfall S4", 100);
            RegisterEntry("Water", 13603, "Waterfall S5", 100);
            RegisterEntry("Water", 13446, "Large Rock E1", 100);
            RegisterEntry("Water", 13451, "Large Rock E2", 100);
            RegisterEntry("Water", 13345, "Large Rock S", 100);
            RegisterEntry("Water", 13356, "Small Rock E1", 100);
            RegisterEntry("Water", 13484, "Small Rock E2", 100);
            RegisterEntry("Water", 13488, "Small Rock S1", 100);
            RegisterEntry("Water", 13350, "Small Rock S2", 100);
            RegisterEntry("Water", 942, "Post", 100);
            RegisterEntry("Water", 5952, "Fountain 1", 500);
            RegisterEntry("Water", 6610, "Fountain 2", 500);

            //Water"
            RegisterEntry("Water", 8099, "Small Wave N", 100);
            RegisterEntry("Water", 8104, "Small Wave W", 100);
            RegisterEntry("Water", 8109, "Small Wave E", 100);
            RegisterEntry("Water", 8114, "Small Wave S", 100);
            RegisterEntry("Water", 8119, "Large Wave N", 100);
            RegisterEntry("Water", 8124, "Large Wave W", 100);
            RegisterEntry("Water", 8129, "Large Wave E", 100);
            RegisterEntry("Water", 8134, "Large Wave S", 100);
            RegisterEntry("Water", 6045, "Edging 1", 50);
            RegisterEntry("Water", 6046, "Edging 2", 50);
            RegisterEntry("Water", 6047, "Edging 3", 50);
            RegisterEntry("Water", 6048, "Edging 4", 50);
            RegisterEntry("Water", 6049, "Edging 5", 50);
            RegisterEntry("Water", 6050, "Edging 6", 50);
            RegisterEntry("Water", 6051, "Edging 7", 50);
            RegisterEntry("Water", 6052, "Edging 8", 50);
            RegisterEntry("Water", 6053, "Edging 9", 50);
            RegisterEntry("Water", 6054, "Edging 10", 50);
            RegisterEntry("Water", 6055, "Edging 11", 50);
            RegisterEntry("Water", 6056, "Edging 12", 50);
            RegisterEntry("Water", 6057, "Edging 13", 50);
            RegisterEntry("Water", 6058, "Edging 14", 50);
            RegisterEntry("Water", 6059, "Edging 15", 50);
            RegisterEntry("Water", 6060, "Edging 16", 50);

            //Lava"
            RegisterEntry("Lava", 4846, "Lava East 1", 100);
            RegisterEntry("Lava", 4852, "Lava East 2", 100);
            RegisterEntry("Lava", 4858, "Lava East 3", 100);
            RegisterEntry("Lava", 4864, "Lava East 4", 100);
            RegisterEntry("Lava", 4870, "Lava South 1", 100);
            RegisterEntry("Lava", 4876, "Lava South 2", 100);
            RegisterEntry("Lava", 4882, "Lava South 3", 100);
            RegisterEntry("Lava", 4888, "Lava South 4", 100);
            RegisterEntry("Lava", 4894, "Lava Edge 1", 100);
            RegisterEntry("Lava", 4897, "Lava Edge 2", 100);
            RegisterEntry("Lava", 4900, "Lava Edge 3", 100);
            RegisterEntry("Lava", 4903, "Lava Edge 4", 100);
            RegisterEntry("Lava", 4906, "Lava Edge 5", 100);
            RegisterEntry("Lava", 4909, "Lava Edge 6", 100);
            RegisterEntry("Lava", 4912, "Lava Edge 7", 100);
            RegisterEntry("Lava", 4915, "Lava Edge 8", 100);
            RegisterEntry("Lava", 4918, "Lava Edge 9", 100);
            RegisterEntry("Lava", 4921, "Lava Edge 10", 100);
            RegisterEntry("Lava", 4924, "Lava Edge 11", 100);
            RegisterEntry("Lava", 4927, "Lava Edge 12", 100);
            RegisterEntry("Lava", 4930, "Lava Edge 13", 100);
            RegisterEntry("Lava", 4933, "Lava Edge 14", 100);
            RegisterEntry("Lava", 4936, "Lava Edge 15", 100);
            RegisterEntry("Lava", 4939, "Lava Edge 16", 100);

            //Lava"
            RegisterEntry("Lava", 6681, "Lavafall East 1", 100);
            RegisterEntry("Lava", 6686, "Lavafall East 2", 100);
            RegisterEntry("Lava", 6691, "Lavafall East 3", 100);
            RegisterEntry("Lava", 6696, "Lavafall East 4", 100);
            RegisterEntry("Lava", 6701, "Lavafall East 5", 100);
            RegisterEntry("Lava", 6706, "Lavafall East 6", 100);
            RegisterEntry("Lava", 6711, "Lavafall East 7", 100);
            RegisterEntry("Lava", 6715, "Lavafall East 8", 100);
            RegisterEntry("Lava", 6719, "Lavafall East 9", 100);
            RegisterEntry("Lava", 6723, "Lavafall East 10", 100);
            RegisterEntry("Lava", 13410, "Lava Stagnant 1", 100);
            RegisterEntry("Lava", 13371, "Bubble 1", 100);
            RegisterEntry("Lava", 13401, "Bubble 2", 100);
            RegisterEntry("Lava", 6727, "Lavafall South 1", 100);
            RegisterEntry("Lava", 6732, "Lavafall South 2", 100);
            RegisterEntry("Lava", 6737, "Lavafall South 3", 100);
            RegisterEntry("Lava", 6742, "Lavafall South 4", 100);
            RegisterEntry("Lava", 6747, "Lavafall South 5", 100);
            RegisterEntry("Lava", 6752, "Lavafall South 6", 100);
            RegisterEntry("Lava", 6757, "Lavafall South 7", 100);
            RegisterEntry("Lava", 6761, "Lavafall South 8", 100);
            RegisterEntry("Lava", 6765, "Lavafall South 9", 100);
            RegisterEntry("Lava", 6769, "Lavafall South 10", 100);
            RegisterEntry("Lava", 13416, "Lava Stagnant 2", 100);
            RegisterEntry("Lava", 13390, "Bubble 3", 100);

            //Swamp"
            RegisterEntry("Swamp", 12813, "Swamp 1", 100);
            RegisterEntry("Swamp", 12819, "Swamp 2", 100);
            RegisterEntry("Swamp", 12826, "Swamp 3", 100);
            RegisterEntry("Swamp", 12832, "Swamp 4", 100);
            RegisterEntry("Swamp", 12838, "Swamp 5", 100);
            RegisterEntry("Swamp", 12844, "Bubble 1", 100);
            RegisterEntry("Swamp", 12854, "Bubble 2", 100);
            RegisterEntry("Swamp", 12865, "Bubble 3", 100);
            RegisterEntry("Swamp", 12876, "Stump 1", 100);
            RegisterEntry("Swamp", 12877, "Stump 2", 100);
            RegisterEntry("Swamp", 12878, "Log N1", 100);
            RegisterEntry("Swamp", 12879, "Log N2", 100);
            RegisterEntry("Swamp", 12880, "Log E1", 100);
            RegisterEntry("Swamp", 12881, "Log E2", 100);
            RegisterEntry("Swamp", 12888, "Edging 1", 100);
            RegisterEntry("Swamp", 12889, "Edging 2", 100);
            RegisterEntry("Swamp", 12890, "Edging 3", 100);
            RegisterEntry("Swamp", 12891, "Edging 4", 100);
            RegisterEntry("Swamp", 12892, "Edging 5", 100);
            RegisterEntry("Swamp", 12893, "Edging 6", 100);
            RegisterEntry("Swamp", 12894, "Edging 7", 100);
            RegisterEntry("Swamp", 12895, "Edging 8", 100);
            RegisterEntry("Swamp", 12896, "Edging 9", 100);
            RegisterEntry("Swamp", 12897, "Edging 10", 100);

            //Swamp"
            RegisterEntry("Swamp", 12898, "Edging 11", 100);
            RegisterEntry("Swamp", 12899, "Edging 12", 100);
            RegisterEntry("Swamp", 12900, "Edging 13", 100);
            RegisterEntry("Swamp", 12901, "Edging 14", 100);
            RegisterEntry("Swamp", 12902, "Edging 15", 100);
            RegisterEntry("Swamp", 12903, "Edging 16", 100);
            RegisterEntry("Swamp", 12904, "Edging 17", 100);
            RegisterEntry("Swamp", 12912, "Edging 18", 50);
            RegisterEntry("Swamp", 12913, "Edging 19", 50);
            RegisterEntry("Swamp", 12914, "Edging 20", 50);
            RegisterEntry("Swamp", 12915, "Edging 21", 50);
            RegisterEntry("Swamp", 12916, "Edging 22", 50);
            RegisterEntry("Swamp", 12917, "Edging 23", 50);
            RegisterEntry("Swamp", 12918, "Edging 24", 50);
            RegisterEntry("Swamp", 12919, "Edging 25", 50);
            RegisterEntry("Swamp", 12920, "Edging 26", 50);
            RegisterEntry("Swamp", 12921, "Edging 27", 50);
            RegisterEntry("Swamp", 12922, "Edging 28", 50);
            RegisterEntry("Swamp", 12923, "Edging 29", 50);
            RegisterEntry("Swamp", 12924, "Edging 30", 50);
            RegisterEntry("Swamp", 12925, "Edging 31", 50);
            RegisterEntry("Swamp", 12926, "Edging 32", 50);
            RegisterEntry("Swamp", 12927, "Edging 33", 50);

            //Plants"
            RegisterEntry("Plants", 3203, "Campion Flowers 1", 100);
            RegisterEntry("Plants", 3204, "Foxglove Flowers 1", 100);
            RegisterEntry("Plants", 3205, "Orfluer Flower", 100);
            RegisterEntry("Plants", 3206, "Red Poppies", 100);
            RegisterEntry("Plants", 3207, "Campion Flowers 2", 100);
            RegisterEntry("Plants", 3208, "Snowdrops 1", 100);
            RegisterEntry("Plants", 3209, "Campion Flowers 3", 100);
            RegisterEntry("Plants", 3210, "Foxglove Flowers 2", 100);
            RegisterEntry("Plants", 3211, "White Flowers 1", 100);
            RegisterEntry("Plants", 3212, "White Flowers 2", 100);
            RegisterEntry("Plants", 3213, "White Poppies", 100);
            RegisterEntry("Plants", 3214, "Snowdrops 2", 100);
            RegisterEntry("Plants", 3219, "Blade Plant", 100);
            RegisterEntry("Plants", 3220, "Bulrushes", 100);
            RegisterEntry("Plants", 3221, "Coconut Palm", 100);
            RegisterEntry("Plants", 3222, "Date Palm", 100);
            RegisterEntry("Plants", 3223, "Elephant Ear", 100);
            RegisterEntry("Plants", 3224, "Fan Plant", 100);
            RegisterEntry("Plants", 3225, "Small Palm 1", 100);
            RegisterEntry("Plants", 3226, "Small Palm 2", 100);
            RegisterEntry("Plants", 3227, "Small Palm 3", 100);
            RegisterEntry("Plants", 3228, "Small Palm 4", 100);
            RegisterEntry("Plants", 3229, "Small Palm 5", 100);
            RegisterEntry("Plants", 3230, "O'hii Tree", 100);

            //Plants"
            RegisterEntry("Plants", 3244, "Grasses 1", 50);
            RegisterEntry("Plants", 3245, "Grasses 2", 50);
            RegisterEntry("Plants", 3246, "Grasses 3", 50);
            RegisterEntry("Plants", 3247, "Grasses 4", 50);
            RegisterEntry("Plants", 3248, "Grasses 5", 50);
            RegisterEntry("Plants", 3249, "Grasses 6", 50);
            RegisterEntry("Plants", 3250, "Grasses 7", 50);
            RegisterEntry("Plants", 3251, "Grasses 8", 50);
            RegisterEntry("Plants", 3252, "Grasses 9", 50);
            RegisterEntry("Plants", 3253, "Grasses 10", 50);
            RegisterEntry("Plants", 3254, "Grasses 11", 50);
            RegisterEntry("Plants", 3257, "Grasses 12", 50);
            RegisterEntry("Plants", 3258, "Grasses 13", 50);
            RegisterEntry("Plants", 3259, "Grasses 14", 50);
            RegisterEntry("Plants", 3260, "Grasses 15", 50);
            RegisterEntry("Plants", 3261, "Grasses 16", 50);
            RegisterEntry("Plants", 3269, "Grasses 17", 50);
            RegisterEntry("Plants", 3270, "Grasses 18", 50);
            RegisterEntry("Plants", 3278, "Grasses 19", 50);
            RegisterEntry("Plants", 3279, "Grasses 20", 50);
            RegisterEntry("Plants", 3255, "Cattails 1", 50);
            RegisterEntry("Plants", 3256, "Cattails 2", 50);
            RegisterEntry("Plants", 3262, "Poppies 1", 50);
            RegisterEntry("Plants", 3263, "Poppies 2", 50);

            //Plants"
            RegisterEntry("Plants", 3264, "Orfluer Flowers 1", 100);
            RegisterEntry("Plants", 3265, "Orfluer Flowers 2", 100);
            RegisterEntry("Plants", 3237, "Pampas Grass 1", 100);
            RegisterEntry("Plants", 3276, "Century Plant 1", 150);
            RegisterEntry("Plants", 3277, "Century Plant 2", 150);
            RegisterEntry("Plants", 3283, "Yucca", 150);
            RegisterEntry("Plants", 3268, "Pampas Grass 2", 100);
            RegisterEntry("Plants", 3238, "Ponytail Palm", 100);
            RegisterEntry("Plants", 3239, "Rushes", 100);
            RegisterEntry("Plants", 3240, "Small Banana Tree", 150);
            RegisterEntry("Plants", 3241, "Snake Plant", 100);
            RegisterEntry("Plants", 3242, "Banana Tree", 150);
            RegisterEntry("Plants", 3231, "Fern 1", 100);
            RegisterEntry("Plants", 3232, "Fern 2", 100);
            RegisterEntry("Plants", 3233, "Fern 3", 100);
            RegisterEntry("Plants", 3234, "Fern 4", 100);
            RegisterEntry("Plants", 3235, "Fern 5", 100);
            RegisterEntry("Plants", 3236, "Fern 6", 100);
            RegisterEntry("Plants", 3273, "Spider Tree", 150);
            RegisterEntry("Plants", 3305, "Sapling 1", 150);
            RegisterEntry("Plants", 3306, "Sapling 2", 150);
            RegisterEntry("Plants", 3267, "Muck", 50);
            RegisterEntry("Plants", 3271, "Weed", 50);
            RegisterEntry("Plants", 3272, "Juniper Bush", 150);

            //Plants"
            RegisterEntry("Plants", 3332, "Water Plant", 100);
            RegisterEntry("Plants", 3333, "Reeds", 100);
            RegisterEntry("Plants", 3334, "Lilypad 1", 50);
            RegisterEntry("Plants", 3335, "Lilypad 2", 50);
            RegisterEntry("Plants", 3336, "Lilypad 3", 50);
            RegisterEntry("Plants", 3337, "Lilypad 4", 50);
            RegisterEntry("Plants", 3338, "Lilypad 5", 50);
            RegisterEntry("Plants", 3339, "Lilypads", 100);
            RegisterEntry("Plants", 3381, "Pipe Cactus", 150);
            RegisterEntry("Plants", 3365, "Cactus 1", 100);
            RegisterEntry("Plants", 3366, "Cactus 2", 100);
            RegisterEntry("Plants", 3367, "Cactus 3", 100);
            RegisterEntry("Plants", 3368, "Cactus 4", 100);
            RegisterEntry("Plants", 3370, "Cactus 5", 100);
            RegisterEntry("Plants", 3372, "Cactus 6", 100);
            RegisterEntry("Plants", 3374, "Cactus 7", 100);
            RegisterEntry("Plants", 3342, "Mushrooms 1", 50);
            RegisterEntry("Plants", 3343, "Mushrooms 2", 50);
            RegisterEntry("Plants", 3344, "Mushrooms 3", 50);
            RegisterEntry("Plants", 3347, "Mushrooms 4", 50);
            RegisterEntry("Plants", 3348, "Mushrooms 5", 50);
            RegisterEntry("Plants", 3349, "Mushrooms 6", 50);
            RegisterEntry("Plants", 3350, "Mushrooms 7", 50);
            RegisterEntry("Plants", 3351, "Mushrooms 8", 50);

            //Plants"
            RegisterEntry("Plants", 3307, "Vines 1", 100);
            RegisterEntry("Plants", 3308, "Vines 2", 100);
            RegisterEntry("Plants", 3309, "Vines 3", 100);
            RegisterEntry("Plants", 3310, "Vines 4", 100);
            RegisterEntry("Plants", 3311, "Vines 5", 100);
            RegisterEntry("Plants", 3312, "Vines 6", 100);
            RegisterEntry("Plants", 3313, "Vines 7", 100);
            RegisterEntry("Plants", 3314, "Vines 8", 100);
            RegisterEntry("Plants", 3380, "Morning Glories", 50);
            RegisterEntry("Plants", 3355, "Grapevines 1", 100);
            RegisterEntry("Plants", 3356, "Grapevines 2", 100);
            RegisterEntry("Plants", 3357, "Grapevines 3", 100);
            RegisterEntry("Plants", 3358, "Grapevines 4", 100);
            RegisterEntry("Plants", 3359, "Grapevines 5", 100);
            RegisterEntry("Plants", 3360, "Grapevines 6", 100);
            RegisterEntry("Plants", 3361, "Grapevines 7", 100);
            RegisterEntry("Plants", 3362, "Grapevines 8", 100);
            RegisterEntry("Plants", 3363, "Grapevines 9", 100);
            RegisterEntry("Plants", 3364, "Grapevines 10", 100);
            RegisterEntry("Plants", 3315, "Log Piece 1", 50);
            RegisterEntry("Plants", 3316, "Log Piece 2", 50);
            RegisterEntry("Plants", 3317, "Log Piece 3", 33);
            RegisterEntry("Plants", 3318, "Log Piece 4", 33);
            RegisterEntry("Plants", 3319, "Log Piece 5", 33);

            //Trees"
            RegisterEntry("Trees", 3277, "Tree 1T", 200);
            RegisterEntry("Trees", 3278, "Leaves 1N ", 50);
            RegisterEntry("Trees", 3279, "Leaves 1F", 50);
            RegisterEntry("Trees", 3280, "Tree 2T", 200);
            RegisterEntry("Trees", 3281, "Leaves 2N", 50);
            RegisterEntry("Trees", 3282, "Leaves 2F", 50);
            RegisterEntry("Trees", 3283, "Tree 3T", 200);
            RegisterEntry("Trees", 3284, "Leaves 3N", 50);
            RegisterEntry("Trees", 3285, "Leaves 3F", 50);
            RegisterEntry("Trees", 3290, "Tree 4T", 200);
            RegisterEntry("Trees", 3291, "Leaves 4N", 50);
            RegisterEntry("Trees", 3292, "Leaves 4F", 50);
            RegisterEntry("Trees", 3293, "Tree 5T", 200);
            RegisterEntry("Trees", 3294, "Leaves 5N", 50);
            RegisterEntry("Trees", 3295, "Leaves 5F", 50);
            RegisterEntry("Trees", 3296, "Tree 6T", 200);
            RegisterEntry("Trees", 3297, "Leaves 6N", 50);
            RegisterEntry("Trees", 3298, "Leaves 6F", 50);
            RegisterEntry("Trees", 3299, "Tree 7T", 200);
            RegisterEntry("Trees", 3300, "Leaves 7N", 50);
            RegisterEntry("Trees", 3301, "Leaves 7F", 50);
            RegisterEntry("Trees", 3302, "Tree 8T", 200);
            RegisterEntry("Trees", 3303, "Leaves 8N", 50);
            RegisterEntry("Trees", 3304, "Leaves 8F", 50);

            //Trees"
            RegisterEntry("Trees", 3476, "Tree 9T", 200);
            RegisterEntry("Trees", 3477, "Leaves 9N", 50);
            RegisterEntry("Trees", 3478, "Leaves 9O", 50);
            RegisterEntry("Trees", 3479, "Leaves 9F", 50);
            RegisterEntry("Trees", 3480, "Tree 10T", 200);
            RegisterEntry("Trees", 3481, "Leaves 10N", 50);
            RegisterEntry("Trees", 3482, "Leaves 10O", 50);
            RegisterEntry("Trees", 3483, "Leaves 10F", 50);
            RegisterEntry("Trees", 3484, "Tree 11T", 200);
            RegisterEntry("Trees", 3485, "Leaves 11N", 50);
            RegisterEntry("Trees", 3486, "Leaves 11O", 50);
            RegisterEntry("Trees", 3487, "Leaves 11F", 50);
            RegisterEntry("Trees", 3488, "Tree 12T", 200);
            RegisterEntry("Trees", 3489, "Leaves 12N", 50);
            RegisterEntry("Trees", 3490, "Leaves 12O", 50);
            RegisterEntry("Trees", 3491, "Leaves 12F", 50);
            RegisterEntry("Trees", 3492, "Tree 13T", 200);
            RegisterEntry("Trees", 3493, "Leaves 13N", 50);
            RegisterEntry("Trees", 3494, "Leaves 13O", 50);
            RegisterEntry("Trees", 3495, "Leaves 13F", 50);
            RegisterEntry("Trees", 3496, "Tree 14T", 200);
            RegisterEntry("Trees", 3497, "Leaves 14N", 50);
            RegisterEntry("Trees", 3498, "Leaves 14O", 50);
            RegisterEntry("Trees", 3499, "Leaves 14F", 50);

            //Trees"
            RegisterEntry("Trees", 3286, "Tree 15T", 200);
            RegisterEntry("Trees", 3287, "Leaves 15N", 100);
            RegisterEntry("Trees", 3288, "Tree 16T", 200);
            RegisterEntry("Trees", 3289, "Leaves 16N", 100);
            RegisterEntry("Trees", 3395, "Jungle 1T", 400);
            RegisterEntry("Trees", 3401, "Leaves 1N", 200);
            RegisterEntry("Trees", 3408, "Leaves 1O", 200);
            RegisterEntry("Trees", 3417, "Jungle 2T", 400);
            RegisterEntry("Trees", 3423, "Leaves 2N", 200);
            RegisterEntry("Trees", 3430, "Leaves 2O", 200);
            RegisterEntry("Trees", 3440, "Jungle 3T", 400);
            RegisterEntry("Trees", 3446, "Leaves 3N", 200);
            RegisterEntry("Trees", 3453, "Leaves 3O", 200);
            RegisterEntry("Trees", 3461, "Jungle 4T", 400);
            RegisterEntry("Trees", 3465, "Leaves 4N", 200);
            RegisterEntry("Trees", 3470, "Leaves 4O", 200);
            RegisterEntry("Trees", 4793, "Yew Tree T", 1000);
            RegisterEntry("Trees", 4802, "Yew Tree L", 500);
            RegisterEntry("Trees", 3413, "Vines 1", 100);
            RegisterEntry("Trees", 3457, "Vines 3", 100);
            RegisterEntry("Trees", 3436, "Vines 2", 100);
            RegisterEntry("Trees", 3474, "Vines 4", 100);
        }

        public static void RegisterMultis()
        {
            YardMultiIDs = new Dictionary<int, List<YardMultiInfo>>();
            int locationID;
            List<YardMultiInfo> infos;

            #region Fountains
            //Sand
            infos = new List<YardMultiInfo>();
            locationID = 5946;
            infos.Add(new YardMultiInfo(locationID - 9, new Point3D(-2, 1, 0)));
            infos.Add(new YardMultiInfo(locationID - 8, new Point3D(-1, 1, 0)));
            infos.Add(new YardMultiInfo(locationID - 7, new Point3D(-0, 1, 0)));
            infos.Add(new YardMultiInfo(locationID - 6, new Point3D(+1, 1, 0)));

            infos.Add(new YardMultiInfo(locationID - 5, new Point3D(+1, +0, 0)));
            infos.Add(new YardMultiInfo(locationID - 4, new Point3D(+1, -1, 0)));
            infos.Add(new YardMultiInfo(locationID - 3, new Point3D(+1, -2, 0)));

            infos.Add(new YardMultiInfo(locationID - 2, new Point3D(+0, -2, 0)));
            infos.Add(new YardMultiInfo(locationID - 1, new Point3D(+0, -1, 0)));

            infos.Add(new YardMultiInfo(locationID + 1, new Point3D(-1, +0, 0)));
            infos.Add(new YardMultiInfo(locationID + 2, new Point3D(-2, +0, 0)));

            infos.Add(new YardMultiInfo(locationID + 3, new Point3D(-2, -1, 0)));
            infos.Add(new YardMultiInfo(locationID + 4, new Point3D(-1, -1, 0)));

            infos.Add(new YardMultiInfo(locationID + 5, new Point3D(-1, -2, 0)));
            infos.Add(new YardMultiInfo(locationID + 6, new Point3D(-2, -2, 0)));
            YardMultiIDs.Add(locationID, infos);

            //Stone
            infos = new List<YardMultiInfo>();
            locationID = 6604;
            infos.Add(new YardMultiInfo(locationID - 9, new Point3D(-2, 1, 0)));
            infos.Add(new YardMultiInfo(locationID - 8, new Point3D(-1, 1, 0)));
            infos.Add(new YardMultiInfo(locationID - 7, new Point3D(-0, 1, 0)));
            infos.Add(new YardMultiInfo(locationID - 6, new Point3D(+1, 1, 0)));

            infos.Add(new YardMultiInfo(locationID - 5, new Point3D(+1, +0, 0)));
            infos.Add(new YardMultiInfo(locationID - 4, new Point3D(+1, -1, 0)));
            infos.Add(new YardMultiInfo(locationID - 3, new Point3D(+1, -2, 0)));

            infos.Add(new YardMultiInfo(locationID - 2, new Point3D(+0, -2, 0)));
            infos.Add(new YardMultiInfo(locationID - 1, new Point3D(+0, -1, 0)));

            infos.Add(new YardMultiInfo(locationID + 1, new Point3D(-1, +0, 0)));
            infos.Add(new YardMultiInfo(locationID + 2, new Point3D(-2, +0, 0)));

            infos.Add(new YardMultiInfo(locationID + 3, new Point3D(-2, -1, 0)));
            infos.Add(new YardMultiInfo(locationID + 4, new Point3D(-1, -1, 0)));

            infos.Add(new YardMultiInfo(locationID + 5, new Point3D(-1, -2, 0)));
            infos.Add(new YardMultiInfo(locationID + 6, new Point3D(-2, -2, 0)));
            YardMultiIDs.Add(locationID, infos);
            #endregion

            #region Trees
            AddTreeInfo(3395, 2, 1, out infos);
            YardMultiIDs.Add(3395, infos);

            AddTreeInfo(3401, 4, 3, out infos);
            YardMultiIDs.Add(3401, infos);

            AddTreeInfo(3408, 3, 3, out infos);
            YardMultiIDs.Add(3408, infos);

            AddTreeInfo(3417, 2, 2, out infos);
            YardMultiIDs.Add(3417, infos);

            AddTreeInfo(3423, 3, 3, out infos);
            YardMultiIDs.Add(3423, infos);

            AddTreeInfo(3430, 3, 3, out infos);
            YardMultiIDs.Add(3430, infos);

            AddTreeInfo(3440, 2, 2, out infos);
            YardMultiIDs.Add(3440, infos);

            AddTreeInfo(3446, 2, 2, out infos);
            YardMultiIDs.Add(3446, infos);

            AddTreeInfo(3453, 3, 2, out infos);
            YardMultiIDs.Add(3453, infos);

            AddTreeInfo(3461, 1, 1, out infos);
            YardMultiIDs.Add(3461, infos);

            AddTreeInfo(3465, 2, 2, out infos);
            YardMultiIDs.Add(3465, infos);

            AddTreeInfo(3470, 2, 2, out infos);
            YardMultiIDs.Add(3470, infos);

            AddTreeInfo(4793, 3, 4, out infos);
            YardMultiIDs.Add(4793, infos);

            AddTreeInfo(4802, 4, 5, out infos);
            YardMultiIDs.Add(4802, infos);

            AddTreeInfo(3413, 1, 1, out infos);
            YardMultiIDs.Add(3413, infos);

            AddTreeInfo(3436, -2, -1, out infos);
            YardMultiIDs.Add(3436, infos);

            AddTreeInfo(3457, 1, 2, out infos);
            YardMultiIDs.Add(3457, infos);

            AddTreeInfo(3474, 1, 1, out infos);
            YardMultiIDs.Add(3474, infos);
            #endregion
        }

        public static void AddTreeInfo(int locationID, int lowRange, int highRange, out List<YardMultiInfo> infos)
        {//Used while registering any trees that contain multiple itemIDs
            infos = new List<YardMultiInfo>();
            while (lowRange > 0)
            {
                infos.Add(new YardMultiInfo(locationID - lowRange, new Point3D(-lowRange, +lowRange, 0)));
                lowRange--;
            }
            while (highRange > 0)
            {
                infos.Add(new YardMultiInfo(locationID + highRange, new Point3D(+highRange, -highRange, 0)));
                highRange--;
            }
        }
        #region YardStairIDGroups

        public static void RegisterStairs()
        {
            YardStairIDGroups = new Dictionary<int, int[]>();

            YardStairIDGroups.Add(1006, new int[] { 1006, 1006, 1006, 1006 });
            YardStairIDGroups.Add(1007, new int[] { 1007, 1008, 1009, 1010 });
            YardStairIDGroups.Add(1011, new int[] { 1011, 1012, 1013, 1014 });
            YardStairIDGroups.Add(1015, new int[] { 1015, 1016, 1017, 1018 });
            YardStairIDGroups.Add(1019, new int[] { 1019, 1020, 1021, 1022 });
            YardStairIDGroups.Add(1023, new int[] { 1023, 1024, 1025, 1026 });
            YardStairIDGroups.Add(1035, new int[] { 1035, 1036, 1037, 1038 });
            YardStairIDGroups.Add(1039, new int[] { 1039, 1040, 1041, 1042 });
            YardStairIDGroups.Add(1043, new int[] { 1043, 1044, 1045, 1046 });
            YardStairIDGroups.Add(1047, new int[] { 1047, 1048, 1049, 1051 });
            YardStairIDGroups.Add(1051, new int[] { 1051, 1052, 1053, 1054 });
            YardStairIDGroups.Add(1173, new int[] { 1173, 1179, 1180, 1181 });
            YardStairIDGroups.Add(1193, new int[] { 1193, 1194, 1205, 1206 });
            YardStairIDGroups.Add(1250, new int[] { 1250, 1276, 1317, 1327 });
            YardStairIDGroups.Add(1289, new int[] { 1289, 1290, 1291, 1292 });
            YardStairIDGroups.Add(1294, new int[] { 1294, 1295, 1297, 1299 });
            YardStairIDGroups.Add(1301, new int[] { 1301, 1374, 1397, 1401 });
            YardStairIDGroups.Add(1801, new int[] { 1801, 1801, 1801, 1801 });
            YardStairIDGroups.Add(1802, new int[] { 1802, 1803, 1804, 1805 });
            YardStairIDGroups.Add(1806, new int[] { 1806, 1807, 1808, 1809 });
            YardStairIDGroups.Add(1810, new int[] { 1810, 1811, 1812, 1813 });
            YardStairIDGroups.Add(1814, new int[] { 1814, 1815, 1816, 1817 });
            YardStairIDGroups.Add(1818, new int[] { 1818, 1819, 1820, 1821 });
            YardStairIDGroups.Add(1822, new int[] { 1822, 1822, 1822, 1822 });
            YardStairIDGroups.Add(1823, new int[] { 1823, 1846, 1847, 1865 });
            YardStairIDGroups.Add(1825, new int[] { 1825, 1825, 1825, 1825 });
            YardStairIDGroups.Add(1826, new int[] { 1826, 1827, 1828, 1829 });
            YardStairIDGroups.Add(1830, new int[] { 1830, 1831, 1832, 1833 });
            YardStairIDGroups.Add(1834, new int[] { 1834, 1835, 1836, 1837 });
            YardStairIDGroups.Add(1838, new int[] { 1838, 1839, 1840, 1841 });
            YardStairIDGroups.Add(1842, new int[] { 1842, 1843, 1844, 1845 });
            YardStairIDGroups.Add(1848, new int[] { 1848, 1848, 1848, 1848 });
            YardStairIDGroups.Add(1849, new int[] { 1849, 1850, 1851, 1852 });
            YardStairIDGroups.Add(1853, new int[] { 1853, 1854, 1855, 1856 });
            YardStairIDGroups.Add(1857, new int[] { 1857, 1858, 1859, 1860 });
            YardStairIDGroups.Add(1861, new int[] { 1861, 1862, 1863, 1864 });
            YardStairIDGroups.Add(1866, new int[] { 1866, 1867, 1868, 1869 });
            YardStairIDGroups.Add(1870, new int[] { 1870, 1871, 1922, 1923 });
            YardStairIDGroups.Add(1872, new int[] { 1872, 1872, 1872, 1872 });
            YardStairIDGroups.Add(1873, new int[] { 1873, 1874, 1875, 1876 });
            YardStairIDGroups.Add(1877, new int[] { 1877, 1878, 1879, 1880 });
            YardStairIDGroups.Add(1881, new int[] { 1881, 1882, 1883, 1884 });
            YardStairIDGroups.Add(1885, new int[] { 1885, 1886, 1887, 1888 });
            YardStairIDGroups.Add(1889, new int[] { 1889, 1890, 1891, 1892 });
            YardStairIDGroups.Add(1900, new int[] { 1900, 1900, 1900, 1900 });
            YardStairIDGroups.Add(1901, new int[] { 1901, 1902, 1903, 1904 });
            YardStairIDGroups.Add(1905, new int[] { 1905, 1906, 1907, 1908 });
            YardStairIDGroups.Add(1909, new int[] { 1909, 1910, 1911, 1912 });
            YardStairIDGroups.Add(1913, new int[] { 1913, 1914, 1915, 1916 });
            YardStairIDGroups.Add(1917, new int[] { 1917, 1918, 1919, 1920 });
            YardStairIDGroups.Add(1928, new int[] { 1928, 1928, 1928, 1928 });
            YardStairIDGroups.Add(1929, new int[] { 1929, 1930, 1931, 1932 });
            YardStairIDGroups.Add(1933, new int[] { 1933, 1934, 1935, 1936 });
            YardStairIDGroups.Add(1937, new int[] { 1937, 1938, 1939, 1940 });
            YardStairIDGroups.Add(1941, new int[] { 1941, 1942, 1943, 1944 });
            YardStairIDGroups.Add(1945, new int[] { 1945, 1946, 1947, 1948 });
            YardStairIDGroups.Add(1952, new int[] { 1952, 1953, 1954, 2010 });
            YardStairIDGroups.Add(1955, new int[] { 1955, 1955, 1955, 1955 });
            YardStairIDGroups.Add(1956, new int[] { 1956, 1957, 1958, 1959 });
            YardStairIDGroups.Add(1960, new int[] { 1960, 1961, 1962, 1963 });
            YardStairIDGroups.Add(1964, new int[] { 1964, 1965, 1966, 1967 });
            YardStairIDGroups.Add(1978, new int[] { 1978, 1978, 1978, 1978 });
            YardStairIDGroups.Add(1979, new int[] { 1979, 1980, 1979, 1980 });
            YardStairIDGroups.Add(1981, new int[] { 1981, 1982, 1981, 1982 });
            YardStairIDGroups.Add(1983, new int[] { 1983, 1984, 1985, 1986 });
            YardStairIDGroups.Add(1987, new int[] { 1987, 1988, 1989, 1990 });
            YardStairIDGroups.Add(1991, new int[] { 1991, 1992, 1991, 1992 });
            YardStairIDGroups.Add(1993, new int[] { 1993, 1994, 1995, 1996 });
            YardStairIDGroups.Add(1997, new int[] { 1997, 1998, 1999, 2000 });
            YardStairIDGroups.Add(2015, new int[] { 2015, 2016, 2100, 2166 });
            YardStairIDGroups.Add(2170, new int[] { 2170, 2171, 2172, 2173 });
            YardStairIDGroups.Add(12789, new int[] { 12789, 12793, 12794, 12795 });
        }
        #endregion
    }
}
