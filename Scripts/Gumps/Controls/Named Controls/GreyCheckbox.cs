using System;

namespace Server.Gumps
{
    public class GreyCheckbox : GumpCheck
    {
        public GreyCheckbox(Int32 x, Int32 y) : this(x, y, "", false, null) { }

        public GreyCheckbox(Int32 x, Int32 y, String name) : this(x, y, name, false, null) { }

        public GreyCheckbox(Int32 x, Int32 y, String name, Boolean initialState) : this(x, y, name, initialState, null) { }

        public GreyCheckbox(Int32 x, Int32 y, String name, Boolean initialState, GumpResponse callback)
            : base(x, y, 0x25F8, 0x25FC, initialState, callback, name)
        {
        }
    }
}
