using System;
using System.Collections.Generic;

namespace Server.Engines.Plants
{
    public interface IPlantHue
    {
        PlantHue PlantHue { get; set; }
        void InvalidatePlantHue();
    }

    public interface IPigmentHue
    {
        PlantPigmentHue PigmentHue { get; set; }
    }

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
        Bright = 0x8000000,
    }

    public class PlantPigmentHueInfo
    {
        private static readonly Dictionary<PlantPigmentHue, PlantPigmentHueInfo> m_Table;
        private readonly PlantHue m_PlantHue;
        private readonly int m_Hue;
        private readonly int m_Name;
        private readonly PlantPigmentHue m_PlantPigmentHue;
        static PlantPigmentHueInfo()
        {
            m_Table = new Dictionary<PlantPigmentHue, PlantPigmentHueInfo>();

            m_Table[PlantPigmentHue.Plain] = new PlantPigmentHueInfo(PlantHue.Plain, 2101, 1060813, PlantPigmentHue.Plain);
            m_Table[PlantPigmentHue.Red] = new PlantPigmentHueInfo(PlantHue.Red, 1652, 1060814, PlantPigmentHue.Red);
            m_Table[PlantPigmentHue.Blue] = new PlantPigmentHueInfo(PlantHue.Blue, 2122, 1060815, PlantPigmentHue.Blue);
            m_Table[PlantPigmentHue.Yellow] = new PlantPigmentHueInfo(PlantHue.Yellow, 2125, 1060818, PlantPigmentHue.Yellow);
            m_Table[PlantPigmentHue.BrightRed] = new PlantPigmentHueInfo(PlantHue.BrightRed, 1646, 1060814, PlantPigmentHue.BrightRed);
            m_Table[PlantPigmentHue.BrightBlue] = new PlantPigmentHueInfo(PlantHue.BrightBlue, 1310, 1060815, PlantPigmentHue.BrightBlue);
            m_Table[PlantPigmentHue.BrightYellow] = new PlantPigmentHueInfo(PlantHue.BrightYellow, 253, 1060818, PlantPigmentHue.BrightYellow);
            m_Table[PlantPigmentHue.DarkRed] = new PlantPigmentHueInfo(PlantHue.Plain, 1141, 1112162, PlantPigmentHue.DarkRed);
            m_Table[PlantPigmentHue.DarkBlue] = new PlantPigmentHueInfo(PlantHue.Plain, 1317, 1112164, PlantPigmentHue.DarkBlue);
            m_Table[PlantPigmentHue.DarkYellow] = new PlantPigmentHueInfo(PlantHue.Plain, 2217, 1112165, PlantPigmentHue.DarkYellow);
            m_Table[PlantPigmentHue.IceRed] = new PlantPigmentHueInfo(PlantHue.Plain, 335, 1112169, PlantPigmentHue.IceRed);
            m_Table[PlantPigmentHue.IceBlue] = new PlantPigmentHueInfo(PlantHue.Plain, 1154, 1112168, PlantPigmentHue.IceBlue);
            m_Table[PlantPigmentHue.IceYellow] = new PlantPigmentHueInfo(PlantHue.Plain, 56, 1112171, PlantPigmentHue.IceYellow);
            m_Table[PlantPigmentHue.Purple] = new PlantPigmentHueInfo(PlantHue.Purple, 15, 1060816, PlantPigmentHue.Purple);
            m_Table[PlantPigmentHue.Green] = new PlantPigmentHueInfo(PlantHue.Green, 2128, 1060819, PlantPigmentHue.Green);
            m_Table[PlantPigmentHue.Orange] = new PlantPigmentHueInfo(PlantHue.Orange, 1128, 1060817, PlantPigmentHue.Orange);
            m_Table[PlantPigmentHue.BrightPurple] = new PlantPigmentHueInfo(PlantHue.BrightPurple, 316, 1060816, PlantPigmentHue.BrightPurple);
            m_Table[PlantPigmentHue.BrightGreen] = new PlantPigmentHueInfo(PlantHue.BrightGreen, 671, 1060819, PlantPigmentHue.BrightGreen);
            m_Table[PlantPigmentHue.BrightOrange] = new PlantPigmentHueInfo(PlantHue.BrightOrange, 1501, 1060817, PlantPigmentHue.BrightOrange);
            m_Table[PlantPigmentHue.DarkPurple] = new PlantPigmentHueInfo(PlantHue.Plain, 1254, 1113166, PlantPigmentHue.DarkPurple);
            m_Table[PlantPigmentHue.DarkGreen] = new PlantPigmentHueInfo(PlantHue.Plain, 1425, 1112163, PlantPigmentHue.DarkGreen);
            m_Table[PlantPigmentHue.DarkOrange] = new PlantPigmentHueInfo(PlantHue.Plain, 1509, 1112161, PlantPigmentHue.DarkOrange);
            m_Table[PlantPigmentHue.IcePurple] = new PlantPigmentHueInfo(PlantHue.Plain, 511, 1112172, PlantPigmentHue.IcePurple);
            m_Table[PlantPigmentHue.IceGreen] = new PlantPigmentHueInfo(PlantHue.Plain, 261, 1112167, PlantPigmentHue.IceGreen);
            m_Table[PlantPigmentHue.IceOrange] = new PlantPigmentHueInfo(PlantHue.Plain, 346, 1112170, PlantPigmentHue.IceOrange);
            m_Table[PlantPigmentHue.Black] = new PlantPigmentHueInfo(PlantHue.Black, 1175, 1060820, PlantPigmentHue.Black);
            m_Table[PlantPigmentHue.White] = new PlantPigmentHueInfo(PlantHue.White, 1150, 1060821, PlantPigmentHue.White);
            m_Table[PlantPigmentHue.IceBlack] = new PlantPigmentHueInfo(PlantHue.Plain, 2422, 1112988, PlantPigmentHue.IceBlack);
            m_Table[PlantPigmentHue.OffWhite] = new PlantPigmentHueInfo(PlantHue.Plain, 746, 1112224, PlantPigmentHue.OffWhite);
            m_Table[PlantPigmentHue.Metal] = new PlantPigmentHueInfo(PlantHue.Plain, 1105, 1015046, PlantPigmentHue.Metal);
            m_Table[PlantPigmentHue.Pink] = new PlantPigmentHueInfo(PlantHue.Pink, 341, 1061854, PlantPigmentHue.Pink);
            m_Table[PlantPigmentHue.Magenta] = new PlantPigmentHueInfo(PlantHue.Magenta, 1163, 1061852, PlantPigmentHue.Magenta);
            m_Table[PlantPigmentHue.Aqua] = new PlantPigmentHueInfo(PlantHue.Aqua, 391, 1061853, PlantPigmentHue.Aqua);
            m_Table[PlantPigmentHue.FireRed] = new PlantPigmentHueInfo(PlantHue.FireRed, 1358, 1061855, PlantPigmentHue.FireRed);
        }

        private PlantPigmentHueInfo(PlantHue planthue, int hue, int name, PlantPigmentHue pigmentHue)
        {
            m_PlantHue = planthue;
            m_Hue = hue;
            m_Name = name;
            m_PlantPigmentHue = pigmentHue;
        }

        public PlantHue PlantHue => m_PlantHue;
        public int Hue => m_Hue;
        public int Name => m_Name;
        public PlantPigmentHue PlantPigmentHue => m_PlantPigmentHue;
        public static PlantPigmentHue HueFromPlantHue(PlantHue hue)
        {
            if (hue == PlantHue.None || hue == PlantHue.Plain)
                return PlantPigmentHue.Plain;

            foreach (KeyValuePair<PlantPigmentHue, PlantPigmentHueInfo> kvp in m_Table)
            {
                if (kvp.Value.PlantHue == hue)
                    return kvp.Key;
            }

            return PlantPigmentHue.Plain;
        }

        public static PlantPigmentHueInfo GetInfo(PlantPigmentHue hue)
        {
            if (!m_Table.ContainsKey(hue))
                return m_Table[PlantPigmentHue.Plain];

            return m_Table[hue];
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
            return IsMixable(m_PlantPigmentHue);
        }

        public bool IsBright()
        {
            return IsBright(m_PlantPigmentHue);
        }

        public bool IsPrimary()
        {
            return IsPrimary(m_PlantPigmentHue);
        }
    }
}