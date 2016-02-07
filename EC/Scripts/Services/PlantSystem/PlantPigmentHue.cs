using System;
using System.Collections;

namespace Server.Engines.Plants
{
    [Flags]
    public enum PlantPigmentHue
    {
        None = 0,

        Plain = 0x1,

        Red = 0x2,
        Blue = 0x4,
        Yellow = 0x8,

        Purple = Red | Blue,
        Green = Blue | Yellow,
        Orange = Red | Yellow,

        Black = 0x10,
        White = 0x20,

        Pink = 0x40,
        Magenta = 0x80,
        Aqua = 0x100,
        FireRed = 0x200,

        BrightRed = Red | Bright,
        BrightBlue = Blue | Bright,
        BrightYellow = Yellow | Bright,

        BrightPurple = Purple | Bright,
        BrightGreen = Green | Bright,
        BrightOrange = Orange | Bright,

        DarkRed = Red | Dark,
        DarkBlue = Blue | Dark,
        DarkYellow = Yellow | Dark,

        DarkPurple = Purple | Dark,
        DarkGreen = Green | Dark,
        DarkOrange = Orange | Dark,

        IceRed = Red | Ice,
        IceBlue = Blue | Ice,
        IceYellow = Yellow | Ice,

        IcePurple = Purple | Ice,
        IceGreen = Green | Ice,
        IceOrange = Orange | Ice,

        IceBlack = Black | Ice,
        OffWhite = White | Bright,
        Metal = Black | Bright,

        Ice = 0x2000000,
        Dark = 0x4000000,
        Bright = 0x8000000
    }

    public class PlantPigmentHueInfo
    {
        private static Hashtable m_Table;
        private readonly int m_Hue;
        private readonly int m_Name;
        private readonly PlantPigmentHue m_PlantPigmentHue;
        static PlantPigmentHueInfo()
        {
            m_Table = new Hashtable();

            m_Table[PlantPigmentHue.Plain] = new PlantPigmentHueInfo(2101,	1060813, PlantPigmentHue.Plain);
            m_Table[PlantPigmentHue.Red] = new PlantPigmentHueInfo(1652,	1060814, PlantPigmentHue.Red);
            m_Table[PlantPigmentHue.Blue] = new PlantPigmentHueInfo(2122,	1060815, PlantPigmentHue.Blue);
            m_Table[PlantPigmentHue.Yellow] = new PlantPigmentHueInfo(2125,	1060818, PlantPigmentHue.Yellow);
            m_Table[PlantPigmentHue.BrightRed] = new PlantPigmentHueInfo(1646,	1060814, PlantPigmentHue.BrightRed);
            m_Table[PlantPigmentHue.BrightBlue] = new PlantPigmentHueInfo(1310, 1060815, PlantPigmentHue.BrightBlue);
            m_Table[PlantPigmentHue.BrightYellow] = new PlantPigmentHueInfo(253, 1060818, PlantPigmentHue.BrightYellow);
            m_Table[PlantPigmentHue.DarkRed] = new PlantPigmentHueInfo(1141,	1112162, PlantPigmentHue.DarkRed);
            m_Table[PlantPigmentHue.DarkBlue] = new PlantPigmentHueInfo(1317, 1112164, PlantPigmentHue.DarkBlue);
            m_Table[PlantPigmentHue.DarkYellow] = new PlantPigmentHueInfo(2217,	1112165, PlantPigmentHue.DarkYellow);
            m_Table[PlantPigmentHue.IceRed] = new PlantPigmentHueInfo(335, 1112169, PlantPigmentHue.IceRed);
            m_Table[PlantPigmentHue.IceBlue] = new PlantPigmentHueInfo(1154, 1112168, PlantPigmentHue.IceBlue);
            m_Table[PlantPigmentHue.IceYellow] = new PlantPigmentHueInfo(56, 1112171, PlantPigmentHue.IceYellow);
            m_Table[PlantPigmentHue.Purple] = new PlantPigmentHueInfo(15, 1060816, PlantPigmentHue.Purple);
            m_Table[PlantPigmentHue.Green] = new PlantPigmentHueInfo(2128,	1060819, PlantPigmentHue.Green);
            m_Table[PlantPigmentHue.Orange] = new PlantPigmentHueInfo(1128,	1060817, PlantPigmentHue.Orange);
            m_Table[PlantPigmentHue.BrightPurple] = new PlantPigmentHueInfo(316, 1060816, PlantPigmentHue.BrightPurple);
            m_Table[PlantPigmentHue.BrightGreen] = new PlantPigmentHueInfo(671, 1060819, PlantPigmentHue.BrightGreen);
            m_Table[PlantPigmentHue.BrightOrange] = new PlantPigmentHueInfo(1501,	1060817, PlantPigmentHue.BrightOrange);
            m_Table[PlantPigmentHue.DarkPurple] = new PlantPigmentHueInfo(1254,	1113166, PlantPigmentHue.DarkPurple);
            m_Table[PlantPigmentHue.DarkGreen] = new PlantPigmentHueInfo(1425,	1112163, PlantPigmentHue.DarkGreen);
            m_Table[PlantPigmentHue.DarkOrange] = new PlantPigmentHueInfo(1509,	1112161, PlantPigmentHue.DarkOrange);
            m_Table[PlantPigmentHue.IcePurple] = new PlantPigmentHueInfo(511, 1112172, PlantPigmentHue.IcePurple);
            m_Table[PlantPigmentHue.IceGreen] = new PlantPigmentHueInfo(261, 1112167, PlantPigmentHue.IceGreen);
            m_Table[PlantPigmentHue.IceOrange] = new PlantPigmentHueInfo(346, 1112170, PlantPigmentHue.IceOrange);
            m_Table[PlantPigmentHue.Black] = new PlantPigmentHueInfo(1175,	1060820, PlantPigmentHue.Black);
            m_Table[PlantPigmentHue.White] = new PlantPigmentHueInfo(1150,	1060821, PlantPigmentHue.White);
            m_Table[PlantPigmentHue.IceBlack] = new PlantPigmentHueInfo(2422,	1112988, PlantPigmentHue.IceBlack);
            m_Table[PlantPigmentHue.OffWhite] = new PlantPigmentHueInfo(746, 1112224, PlantPigmentHue.OffWhite);
            m_Table[PlantPigmentHue.Metal] = new PlantPigmentHueInfo(1105,	1015046, PlantPigmentHue.Metal);
            m_Table[PlantPigmentHue.Pink] = new PlantPigmentHueInfo(341, 1061854, PlantPigmentHue.Pink);
            m_Table[PlantPigmentHue.Magenta] = new PlantPigmentHueInfo(1163,	1061852, PlantPigmentHue.Magenta);
            m_Table[PlantPigmentHue.Aqua] = new PlantPigmentHueInfo(391, 1061853, PlantPigmentHue.Aqua);
            m_Table[PlantPigmentHue.FireRed] = new PlantPigmentHueInfo(1358,	1061855, PlantPigmentHue.FireRed);
        }

        private PlantPigmentHueInfo(int hue, int name, PlantPigmentHue pigmentHue)
        {
            this.m_Hue = hue;
            this.m_Name = name;
            this.m_PlantPigmentHue = pigmentHue;
        }

        public int Hue
        {
            get
            {
                return this.m_Hue;
            }
        }
        public int Name
        {
            get
            {
                return this.m_Name;
            }
        }
        public PlantPigmentHue PlantPigmentHue
        {
            get
            {
                return this.m_PlantPigmentHue;
            }
        }
        public static PlantPigmentHue HueFromPlantHue(PlantHue hue) 
        {
            return (PlantPigmentHue)(hue & ~PlantHue.Crossable);
        }

        public static PlantPigmentHueInfo GetInfo(PlantPigmentHue PlantPigmentHue)
        {
            PlantPigmentHueInfo info = m_Table[PlantPigmentHue] as PlantPigmentHueInfo;

            if (info != null)
                return info;
            else
                return (PlantPigmentHueInfo)m_Table[PlantPigmentHue.Plain];
        }

        public static bool IsMixable(PlantPigmentHue hue)
        {
            return (hue <= PlantPigmentHue.White && hue != PlantPigmentHue.None);
        }

        public static bool IsBright(PlantPigmentHue hue)
        {
            return (hue & PlantPigmentHue.Bright) != PlantPigmentHue.None;
        }

        public static bool IsPrimary(PlantPigmentHue hue)
        {
            return hue == PlantPigmentHue.Red || hue == PlantPigmentHue.Blue || hue == PlantPigmentHue.Yellow;
        }

        public static PlantPigmentHue Mix(PlantPigmentHue first, PlantPigmentHue second)
        {
            if (!IsMixable(first) || !IsMixable(second))
                return PlantPigmentHue.None;

            if (first == second && (PlantPigmentHue.Plain == first || PlantPigmentHue.Black == first || PlantPigmentHue.White == first))
                return PlantPigmentHue.None;

            if (first == second)
                return second | PlantPigmentHue.Bright;

            if (first == PlantPigmentHue.Plain)
                return second | PlantPigmentHue.Bright;
            else if (second == PlantPigmentHue.Plain)
                return first | PlantPigmentHue.Bright;

            if (first == PlantPigmentHue.White)
                return second | PlantPigmentHue.Ice;
            else if (second == PlantPigmentHue.White)
                return first | PlantPigmentHue.Ice;

            if (first == PlantPigmentHue.Black)
                return second | PlantPigmentHue.Dark;
            else if (second == PlantPigmentHue.Black)
                return first | PlantPigmentHue.Dark;

            bool firstPrimary = IsPrimary(first);
            bool secondPrimary = IsPrimary(second);

            if (firstPrimary && secondPrimary)
                return first | second;

            //
            // not sure after this point
            // 
            // the remaining combinations to precess are (orange,purple,green with
            // any of red, blue, yellow, orange, purple, green)
            // the code below is temporary until proper mixed hues can be confirmed
            // 
            // mixing table on stratics seems incorrect because the table is not symmetrical
            // 

            if (firstPrimary && !secondPrimary)
                return first;

            if (!firstPrimary && secondPrimary)
                return second;

            return first & second;
        }

        public bool IsMixable()
        {
            return IsMixable(this.m_PlantPigmentHue);
        }

        public bool IsBright()
        {
            return IsBright(this.m_PlantPigmentHue);
        }

        public bool IsPrimary()
        {
            return IsPrimary(this.m_PlantPigmentHue);
        }
    }
}