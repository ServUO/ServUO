using System;

namespace Services.Toolbar.Gumps
{
    public class GumpIDs
    {
        public enum MiscIDs
        {
            Background = 0,
            Color = 1,
            Buttonground = 2,
            ButtonOffset = 3,
        }

        public enum ButtonIDs
        {
            Minimize = 0,
            Maximize = 1,
            Customize = 2,
            SpecialCommand = 3,

            Send = 4,
            Teleport = 5,
            Gate = 6,
        }

        private int p_ID;
        public int ID { get { return p_ID; } set { p_ID = value; } }
        private int[,] p_Content;
        public int[,] Content { get { return p_Content; } set { p_Content = value; } }
        private string p_Name;
        public string Name { get { return p_Name; } set { p_Name = value; } }

        public GumpIDs(int iD, string name, int[,] content)
        {
            p_ID = iD;
            p_Content = content;
            p_Name = name;
        }

        private static string[] p_Fonts = new string[] { "", "<b>", "<i>", "<b><i>", "<small>", "<b><small>", "<i><small>", "<b><i><small>", "<big>", "<b><big>", "<i><big>", "<b><i><big>" };
        public static string[] Fonts { get { return p_Fonts; } set { p_Fonts = value; } }

        public static int Skins = 2;
        private static GumpIDs[] m_Misc = new GumpIDs[]
			{
				new GumpIDs( 0, "Background",		new int[2,1]{{ 9200 }, { 9270 }}),
				new GumpIDs( 1, "Color",			new int[2,1]{{ 0 }, { 0 }}),
				new GumpIDs( 2, "Buttonground",		new int[2,1]{{ 9200 }, { 9350 }}),
				new GumpIDs( 3, "ButtonOffset",		new int[2,1]{{ 5 }, { 7 }}),
			};
        public static GumpIDs[] Misc { get { return m_Misc; } set { m_Misc = value; } }

        private static GumpIDs[] m_Buttons = new GumpIDs[]
			{
				new GumpIDs( 0, "Minimize",			new int[2,4]{{ 5603, 5607, 16, 16 }, { 5537, 5539, 19, 21 }}),
				new GumpIDs( 1, "Maximize",			new int[2,4]{{ 5601, 5605, 16, 16 }, { 5540, 5542, 19, 21 }}),
				new GumpIDs( 2, "Customize",		new int[2,4]{{ 22153, 22155, 16, 16 }, { 5525, 5527, 62, 24 }}),
				new GumpIDs( 3, "SpecialCommand",	new int[2,4]{{ 2117, 2118, 15, 15 }, { 9720, 9722, 29, 29 }}),

				new GumpIDs( 4, "Send",				new int[2,4]{{ 2360, 2360, 11, 11 }, { 2360, 2360, 11, 11 }}),
				new GumpIDs( 5, "Teleport",			new int[2,4]{{ 2361, 2361, 11, 11 }, { 2361, 2361, 11, 11 }}),
				new GumpIDs( 6, "Gate",				new int[2,4]{{ 2362, 2362, 11, 11 }, { 2361, 2361, 11, 11 }}),
			};
        public static GumpIDs[] Buttons { get { return m_Buttons; } set { m_Buttons = value; } }
    }
}
