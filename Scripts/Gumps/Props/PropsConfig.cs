namespace Server.Gumps
{
    public class PropsConfig
    {
        public static readonly bool OldStyle = false;

        public static readonly int GumpOffsetX = 100;
        public static readonly int GumpOffsetY = 30;

        public static readonly int TextHue = 0;
        public static readonly int TextOffsetX = 2;

        public static readonly int OffsetGumpID = 0x0A40; // Pure black

        // Light offwhite, textured : Dark navy blue, textured
        public static readonly int HeaderGumpID = OldStyle ? 0x0BBC : 0x0E14;

        public static readonly int EntryGumpID = 0x0BBC; // Light offwhite, textured
        public static readonly int BackGumpID = 0x13BE; // Gray slate/stoney
        public static readonly int SetGumpID = OldStyle ? 0x0000 : 0x0E14; // Empty : Dark navy blue, textured

        public static readonly int SetWidth = 20;
        public static readonly int SetOffsetX = OldStyle ? 4 : 2, SetOffsetY = 2;
        public static readonly int SetButtonID1 = 0x15E1; // Arrow pointing right
        public static readonly int SetButtonID2 = 0x15E5; // " pressed

        public static readonly int PrevWidth = 20;
        public static readonly int PrevOffsetX = 2, PrevOffsetY = 2;
        public static readonly int PrevButtonID1 = 0x15E3; // Arrow pointing left
        public static readonly int PrevButtonID2 = 0x15E7; // " pressed

        public static readonly int NextWidth = 20;
        public static readonly int NextOffsetX = 2, NextOffsetY = 2;
        public static readonly int NextButtonID1 = 0x15E1; // Arrow pointing right
        public static readonly int NextButtonID2 = 0x15E5; // " pressed

        public static readonly int OffsetSize = 1;

        public static readonly int EntryHeight = 20;
        public static readonly int BorderSize = 10;
    }
}