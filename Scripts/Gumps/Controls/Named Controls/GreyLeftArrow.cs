using System;

namespace Server.Gumps
{
    public class GreyLeftArrow : GumpButton
    {
        public GreyLeftArrow(Int32 x, Int32 y) : this(x, y, null) { }
        public GreyLeftArrow(Int32 x, Int32 y, GumpResponse callback) : this(x, y, GumpButtonType.Reply, 0, callback) { }
        public GreyLeftArrow(Int32 x, Int32 y, GumpButtonType buttonType, Int32 param) : this(x, y, buttonType, param, null) { }
        public GreyLeftArrow(Int32 x, Int32 y, GumpButtonType buttonType, Int32 param, GumpResponse callback) : base(x, y, 0x25EA, 0x25EB, -1, buttonType, param, callback) { }
    }
}
