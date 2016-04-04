using System;
using Server.Gumps;

namespace Server.Engines.Quests
{
    public class MondainResignGump : Gump
    {
        private readonly BaseQuest m_Quest;
        public MondainResignGump(BaseQuest quest)
            : base(120, 50)
        {
            if (quest == null)
                return;
		
            this.m_Quest = quest;
		
            this.Closable = false;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;
			
            this.AddPage(0);
			
            this.AddImageTiled(0, 0, 348, 262, 0xA8E);
            this.AddAlphaRegion(0, 0, 348, 262);
            this.AddImage(0, 15, 0x27A8);
            this.AddImageTiled(0, 30, 17, 200, 0x27A7);
            this.AddImage(0, 230, 0x27AA);
            this.AddImage(15, 0, 0x280C);
            this.AddImageTiled(30, 0, 300, 17, 0x280A);
            this.AddImage(315, 0, 0x280E);
            this.AddImage(15, 244, 0x280C);
            this.AddImageTiled(30, 244, 300, 17, 0x280A);
            this.AddImage(315, 244, 0x280E);
            this.AddImage(330, 15, 0x27A8);
            this.AddImageTiled(330, 30, 17, 200, 0x27A7);
            this.AddImage(330, 230, 0x27AA);
            this.AddImage(333, 2, 0x2716);
            this.AddImage(333, 248, 0x2716);
            this.AddImage(2, 248, 0x216);
            this.AddImage(2, 2, 0x2716);
			
            this.AddHtmlLocalized(25, 22, 200, 20, 1049000, 0x7D00, false, false); // Confirm Quest Cancellation
            this.AddImage(25, 40, 0xBBF);
            this.AddHtmlLocalized(25, 55, 300, 120, 1060836, 0xFFFFFF, false, false); // This quest will give you valuable information, skills and equipment that will help you advance in the game at a quicker pace.<BR><BR>Are you certain you wish to cancel at this time?
			
            if (quest.ChainID != QuestChain.None)
            {
                this.AddRadio(25, 145, 0x25F8, 0x25FB, false, (int)Radios.Chain);
                this.AddHtmlLocalized(60, 150, 280, 20, 1075023, 0xFFFFFF, false, false); // Yes, I want to quit this entire chain!
            }
			
            this.AddRadio(25, 180, 0x25F8, 0x25FB, true, (int)Radios.Quest);
            this.AddHtmlLocalized(60, 185, 280, 20, 1049005, 0xFFFFFF, false, false); // Yes, I really want to quit this quest!
            this.AddRadio(25, 215, 0x25F8, 0x25FB, false, (int)Radios.None);
            this.AddHtmlLocalized(60, 220, 280, 20, 1049006, 0xFFFFFF, false, false); // No, I don't want to quit.
            this.AddButton(265, 220, 0xF7, 0xF8, (int)Buttons.Okay, GumpButtonType.Reply, 0); // okay
        }

        private enum Buttons
        {
            Close,
            Okay,
        }

        private enum Radios
        {
            Chain,
            Quest,
            None,			
        }
        public override void OnResponse(Server.Network.NetState state, RelayInfo info)
        {
            if (info.ButtonID != (int)Buttons.Okay || this.m_Quest == null)
                return;
			
            if (this.m_Quest.ChainID != QuestChain.None && info.IsSwitched((int)Radios.Chain))
            { 
                this.m_Quest.OnResign(true);
            }
						
            if (info.IsSwitched((int)Radios.Quest))
            {
                this.m_Quest.OnResign(false);
            }
						
            if (info.IsSwitched((int)Radios.None) && this.m_Quest.Owner != null)
                this.m_Quest.Owner.SendGump(new MondainQuestGump(this.m_Quest.Owner));
        }
    }
}