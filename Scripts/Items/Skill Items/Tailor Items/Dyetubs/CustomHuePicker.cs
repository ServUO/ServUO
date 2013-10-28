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
        public CustomHueGroup(int name, int[] hues)
        {
            this.m_Name = name;
            this.m_Hues = hues;
        }

        public CustomHueGroup(string name, int[] hues)
        {
            this.m_NameString = name;
            this.m_Hues = hues;
        }

        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public string NameString
        {
            get
            {
                return this.m_NameString;
            }
        }
        public int[] Hues
        {
            get
            {
                return this.m_Hues;
            }
        }
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
        private readonly CustomHueGroup[] m_Groups;
        private readonly bool m_DefaultSupported;
        private readonly int m_Title;
        private readonly string m_TitleString;
        public CustomHuePicker(CustomHueGroup[] groups, bool defaultSupported)
        {
            this.m_Groups = groups;
            this.m_DefaultSupported = defaultSupported;
        }

        public CustomHuePicker(CustomHueGroup[] groups, bool defaultSupported, int title)
        {
            this.m_Groups = groups;
            this.m_DefaultSupported = defaultSupported;
            this.m_Title = title;
        }

        public CustomHuePicker(CustomHueGroup[] groups, bool defaultSupported, string title)
        {
            this.m_Groups = groups;
            this.m_DefaultSupported = defaultSupported;
            this.m_TitleString = title;
        }

        public bool DefaultSupported
        {
            get
            {
                return this.m_DefaultSupported;
            }
        }
        public CustomHueGroup[] Groups
        {
            get
            {
                return this.m_Groups;
            }
        }
        public int Title
        {
            get
            {
                return this.m_Title;
            }
        }
        public string TitleString
        {
            get
            {
                return this.m_TitleString;
            }
        }
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
            this.m_From = from;
            this.m_Definition = definition;
            this.m_Callback = callback;
            this.m_State = state;

            this.RenderBackground();
            this.RenderCategories();
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

                            int group = index % this.m_Definition.Groups.Length;
                            index /= this.m_Definition.Groups.Length;

                            if (group >= 0 && group < this.m_Definition.Groups.Length)
                            {
                                int[] hues = this.m_Definition.Groups[group].Hues;

                                if (index >= 0 && index < hues.Length)
                                    this.m_Callback(this.m_From, this.m_State, hues[index]);
                            }
                        }

                        break;
                    }
                case 2: // Default
                    {
                        if (this.m_Definition.DefaultSupported)
                            this.m_Callback(this.m_From, this.m_State, 0);

                        break;
                    }
            }
        }

        private int GetRadioID(int group, int index)
        {
            return (index * this.m_Definition.Groups.Length) + group;
        }

        private void RenderBackground()
        {
            this.AddPage(0);

            this.AddBackground(0, 0, 450, 450, 5054);
            this.AddBackground(10, 10, 430, 430, 3000);

            if (this.m_Definition.TitleString != null)
                this.AddHtml(20, 30, 400, 25, this.m_Definition.TitleString, false, false);
            else if (this.m_Definition.Title > 0)
                this.AddHtmlLocalized(20, 30, 400, 25, this.m_Definition.Title, false, false);

            this.AddButton(20, 400, 4005, 4007, 1, GumpButtonType.Reply, 0);
            this.AddHtmlLocalized(55, 400, 200, 25, 1011036, false, false); // OKAY

            if (this.m_Definition.DefaultSupported)
            {
                this.AddButton(200, 400, 4005, 4007, 2, GumpButtonType.Reply, 0);
                this.AddLabel(235, 400, 0, "DEFAULT");
            }
        }

        private void RenderCategories()
        {
            CustomHueGroup[] groups = this.m_Definition.Groups;

            for (int i = 0; i < groups.Length; ++i)
            {
                this.AddButton(30, 85 + (i * 25), 5224, 5224, 0, GumpButtonType.Page, 1 + i);

                if (groups[i].NameString != null)
                    this.AddHtml(55, 85 + (i * 25), 200, 25, groups[i].NameString, false, false);
                else
                    this.AddHtmlLocalized(55, 85 + (i * 25), 200, 25, groups[i].Name, false, false);
            }

            for (int i = 0; i < groups.Length; ++i)
            {
                this.AddPage(1 + i);

                int[] hues = groups[i].Hues;

                for (int j = 0; j < hues.Length; ++j)
                {
                    this.AddRadio(260, 90 + (j * 25), 210, 211, false, this.GetRadioID(i, j));
                    this.AddLabel(278, 90 + (j * 25), hues[j] - 1, "*****");
                }
            }
        }
    }
}