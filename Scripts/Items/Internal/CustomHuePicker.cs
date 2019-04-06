using System;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    public delegate void CustomHuePickerCallback(Mobile from, object state, int hue);

    public class CustomHueGroup
    {
        private readonly int m_Name;
        private readonly string m_NameString;
        private readonly int[] m_Hues;
        private readonly int m_Item;
        private readonly int m_ItemHues;

        public CustomHueGroup(int name, int[] hues)
        {
            m_Name = name;
            m_Hues = hues;
        }

        public CustomHueGroup(string name, int[] hues)
        {
            m_NameString = name;
            m_Hues = hues;
        }

        public CustomHueGroup(int item, int itemhues, int[] hues)
        {
            m_Item = item;
            m_ItemHues = itemhues;
            m_Hues = hues;
        }

        public int Name { get { return m_Name; } }
        public string NameString { get { return m_NameString; } }
        public int[] Hues { get { return m_Hues; } }
        public int Item { get { return m_Item; } }
        public int ItemHues { get { return m_ItemHues; } }
    }

    public class CustomHuePicker
    {
        public static readonly CustomHuePicker SpecialDyeTub = new CustomHuePicker(new CustomHueGroup[]
        {
            /* Violet */
            new CustomHueGroup(1018345, new int[] { 1230, 1231, 1232, 1233, 1234, 1235 }),
            /* Tan */
            new CustomHueGroup(1018346, new int[] { 1501, 1502, 1503, 1504, 1505, 1506, 1507, 1508 }),
            /* Brown */
            new CustomHueGroup(1018347, new int[] { 2012, 2013, 2014, 2015, 2016, 2017 }),
            /* Dark Blue */
            new CustomHueGroup(1018348, new int[] { 1303, 1304, 1305, 1306, 1307, 1308 }),
            /* Forest Green */
            new CustomHueGroup(1018349, new int[] { 1420, 1421, 1422, 1423, 1424, 1425, 1426 }),
            /* Pink */
            new CustomHueGroup(1018350, new int[] { 1619, 1620, 1621, 1622, 1623, 1624, 1625, 1626 }),
            /* Red */
            new CustomHueGroup(1018351, new int[] { 1640, 1641, 1642, 1643, 1644 }),
            /* Olive */
            new CustomHueGroup(1018352, new int[] { 2001, 2002, 2003, 2004, 2005 })
        }, false, 1018344);
        public static readonly CustomHuePicker LeatherDyeTub = new CustomHuePicker(new CustomHueGroup[]
        {
            /* Dull Copper */
            new CustomHueGroup(1018332, new int[] { 2419, 2420, 2421, 2422, 2423, 2424 }),
            /* Shadow Iron */
            new CustomHueGroup(1018333, new int[] { 2406, 2407, 2408, 2409, 2410, 2411, 2412 }),
            /* Copper */
            new CustomHueGroup(1018334, new int[] { 2413, 2414, 2415, 2416, 2417, 2418 }),
            /* Bronze */
            new CustomHueGroup(1018335, new int[] { 2414, 2415, 2416, 2417, 2418 }),
            /* Glden */
            new CustomHueGroup(1018336, new int[] { 2213, 2214, 2215, 2216, 2217, 2218 }),
            /* Agapite */
            new CustomHueGroup(1018337, new int[] { 2425, 2426, 2427, 2428, 2429, 2430 }),
            /* Verite */
            new CustomHueGroup(1018338, new int[] { 2207, 2208, 2209, 2210, 2211, 2212 }),
            /* Valorite */
            new CustomHueGroup(1018339, new int[] { 2219, 2220, 2221, 2222, 2223, 2224 }),
            /* Reds */
            new CustomHueGroup(1018340, new int[] { 2113, 2114, 2115, 2116, 2117, 2118 }),
            /* Blues */
            new CustomHueGroup(1018341, new int[] { 2119, 2120, 2121, 2122, 2123, 2124 }),
            /* Greens */
            new CustomHueGroup(1018342, new int[] { 2126, 2127, 2128, 2129, 2130 }),
            /* Yellows */
            new CustomHueGroup(1018343, new int[] { 2213, 2214, 2215, 2216, 2217, 2218 })
        }, true);

        public static readonly CustomHuePicker MetallicDyeTub = new CustomHuePicker(new CustomHueGroup[]
        {
            
            new CustomHueGroup(5138, 545, new int[] { 2501, 2502, 2503, 2504, 2505, 2506, 2507, 2508, 2509, 2510, 2511, 2512 }),
            
            new CustomHueGroup(5138, 550, new int[] { 2513, 2514, 2515, 2516, 2517, 2518, 2519, 2520, 2521, 2522, 2523, 2524 }),
           
            new CustomHueGroup(5138, 555, new int[] { 2525, 2526, 2527, 2528, 2529, 2530, 2531, 2532, 2533, 2534, 2535, 2536 }),
            
            new CustomHueGroup(5138, 560, new int[] { 2537, 2538, 2539, 2540, 2541, 2542, 2543, 2544, 2545, 2546, 2547, 2548 }),
            
            new CustomHueGroup(5138, 565, new int[] { 2549, 2550, 2551, 2552, 2553, 2554, 2555, 2556, 2557, 2558, 2559, 2560 }),
            
            new CustomHueGroup(5138, 570, new int[] { 2561, 2562, 2563, 2564, 2565, 2566, 2567, 2568, 2569, 2570, 2571, 2572 }),
            
            new CustomHueGroup(5138, 595, new int[] { 2573, 2574, 2575, 2576, 2577, 2578, 2579, 2580, 2581, 2582, 2583, 2584 }),
            
            new CustomHueGroup(5138, 601, new int[] { 2585, 2586, 2587, 2588, 2589, 2590, 2591, 2592, 2593, 2594, 2595, 2596 }),
            
            new CustomHueGroup(5138, 606, new int[] { 2597, 2598, 2599, 2600, 2601, 2602, 2603, 2604, 2605, 2606, 2607, 2608 }),
            
            new CustomHueGroup(5138, 726, new int[] { 2609, 2610, 2611, 2612, 2613, 2614, 2615, 2616, 2617, 2618, 2619, 2620 }),
            
            new CustomHueGroup(5138, 730, new int[] { 2621, 2622, 2623, 2624, 2625, 2626, 2627, 2628, 2629, 2630, 2631, 2632 }),
            
            new CustomHueGroup(5138, 735, new int[] { 2633, 2634, 2635, 2636, 2637, 2638, 2639, 2640, 2641, 2642, 2643, 2644 }),

            new CustomHueGroup(5138, 805, new int[] { 2651, 2652, 2653, 2654, 2655, 2656, 2657, 2658, 2659, 2660, 2661, 2662 })

        }, true);

        private readonly CustomHueGroup[] m_Groups;
        private readonly bool m_DefaultSupported;
        private readonly int m_Title;
        private readonly string m_TitleString;

        public CustomHuePicker(CustomHueGroup[] groups, bool defaultSupported)
        {
            m_Groups = groups;
            m_DefaultSupported = defaultSupported;
        }

        public CustomHuePicker(CustomHueGroup[] groups, bool defaultSupported, int title)
        {
            m_Groups = groups;
            m_DefaultSupported = defaultSupported;
            m_Title = title;
        }

        public CustomHuePicker(CustomHueGroup[] groups, bool defaultSupported, string title)
        {
            m_Groups = groups;
            m_DefaultSupported = defaultSupported;
            m_TitleString = title;
        }

        public bool DefaultSupported { get { return m_DefaultSupported; } }
        public CustomHueGroup[] Groups { get { return m_Groups; } }
        public int Title { get { return m_Title; } }
        public string TitleString { get { return m_TitleString; } }
    }

    public class CustomHuePickerGump : Gump
    {
        private readonly Mobile m_From;
        private readonly CustomHuePicker m_Definition;
        private readonly CustomHuePickerCallback m_Callback;
        private readonly object m_State;

        public CustomHuePickerGump(Mobile from, CustomHuePicker definition, CustomHuePickerCallback callback, object state)
            : base(50, 50)
        {
            m_From = from;
            m_Definition = definition;
            m_Callback = callback;
            m_State = state;

            if (definition == CustomHuePicker.MetallicDyeTub)
            {
                RenderMetallicBackground();
                RenderMetallicCategories();
            }
            else
            {
                RenderBackground();
                RenderCategories();
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            switch ( info.ButtonID )
            {
                case 1: // Okay
                    {
                        int[] switches = info.Switches;

                        if (switches.Length > 0)
                        {
                            int index = switches[0];

                            int group = index % m_Definition.Groups.Length;
                            index /= m_Definition.Groups.Length;

                            if (group >= 0 && group < m_Definition.Groups.Length)
                            {
                                int[] hues = m_Definition.Groups[group].Hues;

                                if (index >= 0 && index < hues.Length)
                                    m_Callback(m_From, m_State, hues[index]);
                            }
                        }

                        break;
                    }
                case 2: // Default
                    {
                        if (m_Definition.DefaultSupported)
                            m_Callback(m_From, m_State, 0);

                        break;
                    }
            }
        }

        private int GetRadioID(int group, int index)
        {
            return (index * m_Definition.Groups.Length) + group;
        }

        private void RenderBackground()
        {
            AddPage(0);

            AddBackground(0, 0, 450, 450, 5054);
            AddBackground(10, 10, 430, 430, 3000);

            if (m_Definition.TitleString != null)
                AddHtml(20, 30, 400, 25, m_Definition.TitleString, false, false);
            else if (m_Definition.Title > 0)
                AddHtmlLocalized(20, 30, 400, 25, m_Definition.Title, false, false);

            AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 400, 200, 25, 1011036, false, false); // OKAY

            if (m_Definition.DefaultSupported)
            {
                AddButton(200, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(235, 400, 0, "DEFAULT");
            }
        }        

        private void RenderCategories()
        {
            CustomHueGroup[] groups = m_Definition.Groups;

            for (int i = 0; i < groups.Length; ++i)
            {
                AddButton(30, 85 + (i * 25), 5224, 5224, 0, GumpButtonType.Page, 1 + i);

                if (groups[i].NameString != null)
                    AddHtml(55, 85 + (i * 25), 200, 25, groups[i].NameString, false, false);
                else
                    AddHtmlLocalized(55, 85 + (i * 25), 200, 25, groups[i].Name, false, false);
            }

            for (int i = 0; i < groups.Length; ++i)
            {
                AddPage(1 + i);

                int[] hues = groups[i].Hues;

                for (int j = 0; j < hues.Length; ++j)
                {
                    AddRadio(260, 90 + (j * 25), 210, 211, false, GetRadioID(i, j));
                    AddLabel(278, 90 + (j * 25), hues[j] - 1, "*****");
                }
            }
        }

        private void RenderMetallicBackground()
        {
            AddPage(0);

            AddBackground(0, 0, 450, 450, 5054);
            AddBackground(10, 10, 430, 430, 3000);

            AddHtmlLocalized(60, 20, 400, 25, 1150063, false, false); // Base/Shadow Color
            AddHtmlLocalized(260, 20, 400, 25, 1150064, false, false); // Highlight Color

            AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            AddHtmlLocalized(55, 400, 200, 25, 1011036, false, false); // OKAY

            if (m_Definition.DefaultSupported)
            {
                AddButton(200, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
                AddLabel(235, 400, 0, "DEFAULT");
            }
        }

        private void RenderMetallicCategories()
        {
            CustomHueGroup[] groups = m_Definition.Groups;

            for (int i = 0; i < groups.Length; ++i)
            {
                AddButton(30, 65 + (i * 25), 5224, 5224, 0, GumpButtonType.Page, 1 + i);
                AddItem(55, 65 + (i * 25), groups[i].Item, groups[i].ItemHues);
            }

            for (int i = 0; i < groups.Length; ++i)
            {
                AddPage(1 + i);

                int[] hues = groups[i].Hues;

                for (int j = 0; j < hues.Length; ++j)
                {
                    AddRadio(260, 70 + (j * 25), 210, 211, false, GetRadioID(i, j));
                    AddItem(278, 70 + (j * 25), 5138, hues[j]);
                }
            }
        }
    }
}