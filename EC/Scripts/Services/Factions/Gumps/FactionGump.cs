using System;
using Server.Gumps;

namespace Server.Factions
{
    public abstract class FactionGump : Gump
    {
        public FactionGump(int x, int y)
            : base(x, y)
        {
        }

        public virtual int ButtonTypes
        {
            get
            {
                return 10;
            }
        }
        public static bool Exists(Mobile mob)
        {
            return (mob.FindGump(typeof(FactionGump)) != null);
        }

        public int ToButtonID(int type, int index)
        {
            return 1 + (index * this.ButtonTypes) + type;
        }

        public bool FromButtonID(int buttonID, out int type, out int index)
        {
            int offset = buttonID - 1;

            if (offset >= 0)
            {
                type = offset % this.ButtonTypes;
                index = offset / this.ButtonTypes;
                return true;
            }
            else
            {
                type = index = 0;
                return false;
            }
        }

        public void AddHtmlText(int x, int y, int width, int height, TextDefinition text, bool back, bool scroll)
        {
            if (text != null && text.Number > 0)
                this.AddHtmlLocalized(x, y, width, height, text.Number, back, scroll);
            else if (text != null && text.String != null)
                this.AddHtml(x, y, width, height, text.String, back, scroll);
        }
    }
}