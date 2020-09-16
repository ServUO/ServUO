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

            m_Quest = quest;

            Closable = false;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage(0);

            AddImageTiled(0, 0, 348, 262, 0xA8E);
            AddAlphaRegion(0, 0, 348, 262);
            AddImage(0, 15, 0x27A8);
            AddImageTiled(0, 30, 17, 200, 0x27A7);
            AddImage(0, 230, 0x27AA);
            AddImage(15, 0, 0x280C);
            AddImageTiled(30, 0, 300, 17, 0x280A);
            AddImage(315, 0, 0x280E);
            AddImage(15, 244, 0x280C);
            AddImageTiled(30, 244, 300, 17, 0x280A);
            AddImage(315, 244, 0x280E);
            AddImage(330, 15, 0x27A8);
            AddImageTiled(330, 30, 17, 200, 0x27A7);
            AddImage(330, 230, 0x27AA);
            AddImage(333, 2, 0x2716);
            AddImage(333, 248, 0x2716);
            AddImage(2, 248, 0x216);
            AddImage(2, 2, 0x2716);

            AddHtmlLocalized(25, 22, 200, 20, 1049000, 0x7D00, false, false); // Confirm Quest Cancellation
            AddImage(25, 40, 0xBBF);
            AddHtmlLocalized(25, 55, 300, 120, 1060836, 0xFFFFFF, false, false); // This quest will give you valuable information, skills and equipment that will help you advance in the game at a quicker pace.<BR><BR>Are you certain you wish to cancel at this time?

            if (quest.ChainID != QuestChain.None)
            {
                AddRadio(25, 145, 0x25F8, 0x25FB, false, (int)Radios.Chain);
                AddHtmlLocalized(60, 150, 280, 20, 1075023, 0xFFFFFF, false, false); // Yes, I want to quit this entire chain!
            }

            AddRadio(25, 180, 0x25F8, 0x25FB, true, (int)Radios.Quest);
            AddHtmlLocalized(60, 185, 280, 20, 1049005, 0xFFFFFF, false, false); // Yes, I really want to quit this quest!
            AddRadio(25, 215, 0x25F8, 0x25FB, false, (int)Radios.None);
            AddHtmlLocalized(60, 220, 280, 20, 1049006, 0xFFFFFF, false, false); // No, I don't want to quit.
            AddButton(265, 220, 0xF7, 0xF8, (int)Buttons.Okay, GumpButtonType.Reply, 0); // okay
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
        public override void OnResponse(Network.NetState state, RelayInfo info)
        {
            if (info.ButtonID != (int)Buttons.Okay || m_Quest == null)
                return;

            if (m_Quest.ChainID != QuestChain.None && info.IsSwitched((int)Radios.Chain))
            {
                m_Quest.OnResign(true);
            }

            if (info.IsSwitched((int)Radios.Quest))
            {
                m_Quest.OnResign(false);
            }

            if (info.IsSwitched((int)Radios.None) && m_Quest.Owner != null && m_Quest.Owner == state.Mobile)
                m_Quest.Owner.SendGump(new MondainQuestGump(m_Quest.Owner));
        }
    }
}
