using System;
using Server.Items;

namespace Server.Gumps
{
    public class AwaitingSmithApprovalGump : Gump
    {
        private readonly LevelUpScroll m_Scroll;
        private readonly Mobile m_From;
        public AwaitingSmithApprovalGump(LevelUpScroll scroll, Mobile from)
            : base(0, 0)
        {
            this.m_Scroll = scroll;
            this.m_From = from;

            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
            this.AddPage(0);

            this.AddBackground(25, 10, 420, 180, 5054);
            this.AddImageTiled(33, 20, 401, 160, 2624);
            this.AddAlphaRegion(33, 20, 401, 160);

            int value = this.m_Scroll.Value;

            if (value == 5)
                this.AddHtml(40, 20, 260, 20, String.Format("<basefont color=#FFFFFF>Wonderous scroll of leveling (+{0})</basefont>", value), (bool)false, (bool)false); 
            else if (value == 10)
                this.AddHtml(40, 20, 260, 20, String.Format("<basefont color=#FFFFFF>Exalted scroll of leveling (+{0})</basefont>", value), (bool)false, (bool)false); 
            else if (value == 15)
                this.AddHtml(40, 20, 260, 20, String.Format("<basefont color=#FFFFFF>Mythical scroll of leveling (+{0})</basefont>", value), (bool)false, (bool)false); 
            else if (value == 20)
                this.AddHtml(40, 20, 260, 20, String.Format("<basefont color=#FFFFFF>Legendary scroll of leveling (+{0})</basefont>", value), (bool)false, (bool)false); 
            else
                this.AddHtml(40, 20, 260, 20, String.Format("<basefont color=#FFFFFF>Scroll of leveling (+{0})</basefont>", value), false, false);

            this.AddHtml(40, 46, 387, 20, @"<CENTER><U><basefont color=#FF0000>Please Wait...</basefont></U></CENTER>", (bool)false, (bool)false);
            this.AddHtml(40, 70, 387, 100, @"The other player is validating your Level Increase scroll.<br><br>If the process is successful, you will then be given the option to apply the scroll immediately OR press ESC to apply the scroll at a later time.", (bool)true, (bool)false);
        }
    }
}