using System;

namespace Server.Gumps
{
    public class EntryField : Gumpling
    {
        public EntryField(Int32 x, Int32 y, Int32 width, String name, Int32 textEntryHue, String defaultValue) : base(x, y)
        {
            Add(new GumpImageTiled(0, 0, width, 23, 0xA40));
            Add(new GumpImageTiled(1, 1, width - 2, 21, 0xBBC));
            Add(new GumpTextEntry(5, 1, width - 5, 21, textEntryHue, defaultValue, name));
        }
    }
}
