using System;

namespace Server.Gumps
{
    public class GoldRimmedFrame : Gumpling
    {
        public GoldRimmedFrame(Int32 x, Int32 y, Int32 width, Int32 height) : base(x, y)
        {
            Add(new GumpImageTiled(0, 0, width, 3, 0x23BF));
            Add(new GumpImageTiled(0, 3, 3, height - 6, 0x23C1));
            Add(new GumpImageTiled(width - 3, 3, 3, height - 6, 0x23C3));
            Add(new GumpImageTiled(0, height - 3, width, 3, 0x23C5));
        }
    }
}
