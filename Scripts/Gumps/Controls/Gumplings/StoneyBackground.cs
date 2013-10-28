using System;

namespace Server.Gumps
{
    public class StoneyBackground : Gumpling
    {
        public StoneyBackground(Int32 width, Int32 height) : base(0, 0)
        {
            Add(new GumpPage(0));
            Add(new GumpBackground(0, 0, width, height, 0x13EC));
        }
    }
}
