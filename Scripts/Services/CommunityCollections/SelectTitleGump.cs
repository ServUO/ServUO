using System;
using Server.Mobiles;

namespace Server.Gumps
{
    public class SelectTitleGump : Gump
    { 
        private readonly PlayerMobile m_From;
        private readonly int m_Page;
        public SelectTitleGump(PlayerMobile from, int page)
            : base(50, 50)
        { 
            this.m_From = from;
            this.m_Page = page;
		
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
			
            this.AddPage(0);
            this.AddBackground(0, 0, 270, 120, 0x13BE);
            this.AddBackground(10, 10, 250, 100, 0xBB8);
			
            this.AddHtmlLocalized(20, 15, 230, 20, 1073994, 0x1, false, false); // Your title will be:
			
            if (page > -1 && page < from.RewardTitles.Count)
            {
                if (from.RewardTitles[page] is int)
                    this.AddHtmlLocalized(20, 35, 230, 40, (int)from.RewardTitles[page], 0x32, true, false);
                else if (from.RewardTitles[page] is string)
                    this.AddHtml(20, 35, 230, 40, String.Format("<BASEFONT COLOR=#{0:X6}>{1}</BASEFONT>", 0x32, (string)from.RewardTitles[page]), true, false);
            }
            else
                this.AddHtmlLocalized(20, 35, 230, 40, 1073995, 0x32, true, false);
				
            this.AddHtmlLocalized(55, 80, 75, 20, 1073996, 0x0, false, false); // ACCEPT
            this.AddHtmlLocalized(170, 80, 75, 20, 1073997, 0x0, false, false); // NEXT
			
            this.AddButton(20, 80, 0xFA5, 0xFA7, (int)Buttons.Accept, GumpButtonType.Reply, 0);
            this.AddButton(135, 80, 0xFA5, 0xFA7, (int)Buttons.Next, GumpButtonType.Reply, 0);
        }

        private enum Buttons
        {
            Cancel,
            Next,
            Accept,
        }
        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        { 
            switch ( info.ButtonID )
            {
                case (int)Buttons.Accept:
                    this.m_From.SelectRewardTitle(this.m_Page);
                    break;
                case (int)Buttons.Next: 
                    if (this.m_Page == this.m_From.RewardTitles.Count - 1)
                        this.m_From.SendGump(new SelectTitleGump(this.m_From, -1));
                    else if (this.m_Page < this.m_From.RewardTitles.Count - 1 && this.m_Page > - 1)
                        this.m_From.SendGump(new SelectTitleGump(this.m_From, this.m_Page + 1));
                    else
                        this.m_From.SendGump(new SelectTitleGump(this.m_From, 0));	
						
                    break;					
            }
        }
    }
}