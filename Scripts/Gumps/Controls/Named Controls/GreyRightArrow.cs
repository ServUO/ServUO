using System;

namespace Server.Gumps
{
    public class GreyRightArrow : GumpButton
    {
        public GreyRightArrow(Int32 x, Int32 y) : this(x, y, null) { }
        public GreyRightArrow(Int32 x, Int32 y, GumpResponse callback) : this(x, y, GumpButtonType.Reply, 0, callback) { }
        public GreyRightArrow(Int32 x, Int32 y, GumpButtonType buttonType, Int32 param) : this(x, y, buttonType, param, null) { }
        public GreyRightArrow(Int32 x, Int32 y, GumpButtonType buttonType, Int32 param, GumpResponse callback) : base(x, y, 0x25E6, 0x25E7, -1, buttonType, param, callback) { }
    }
}
