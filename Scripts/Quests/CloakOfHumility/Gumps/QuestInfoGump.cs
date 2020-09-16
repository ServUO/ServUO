using Server.Gumps;

namespace Server.Engines.Quests
{
    public class QuestInfoGump : BaseQuestGump
    {
        private readonly int m_Cliloc;

        public QuestInfoGump(int cliloc) : base(0, 0)
        {
            m_Cliloc = cliloc;
            AddPage(0);

            AddImageTiled(50, 20, 400, 400, 0x1404);
            AddImageTiled(50, 29, 30, 390, 0x28DC);
            AddImageTiled(34, 140, 17, 279, 0x242F);
            AddImage(48, 135, 0x28AB);
            AddImage(-16, 285, 0x28A2);
            AddImage(0, 10, 0x28B5);
            AddImage(25, 0, 0x28B4);
            AddImageTiled(83, 15, 350, 15, 0x280A);
            AddImage(34, 419, 0x2842);
            AddImage(442, 419, 0x2840);
            AddImageTiled(51, 419, 392, 17, 0x2775);
            AddImageTiled(415, 29, 44, 390, 0xA2D);
            AddImageTiled(415, 29, 30, 390, 0x28DC);
            AddImage(370, 50, 0x589);
            AddImage(379, 60, 0x15A9);
            AddImage(425, 0, 0x28C9);
            AddImage(90, 33, 0x232D);
            AddImageTiled(130, 65, 175, 1, 0x238D);

            AddHtmlObject(160, 108, 330, 16, 1075781, DarkGreen, false, false);            // Test of Humility

            AddHtmlObject(98, 156, 312, 180, cliloc, LightGreen, false, true);

            AddButton(313, 395, 0x2EEC, 0x2EEE, 0, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(Network.NetState state, RelayInfo info)
        {
            if (m_Cliloc == 1075783)
                state.Mobile.SendLocalizedMessage(1075787); // I feel that thou hast yet more to learn about Humility... Please ponder these things further, and visit me again on the 'morrow.
        }
    }
}