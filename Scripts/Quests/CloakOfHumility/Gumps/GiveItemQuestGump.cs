using Server.Gumps;
using Server.Network;

namespace Server.Engines.Quests
{
    public class HumilityItemQuestGump : Gump
    {
        private readonly HumilityQuestMobile m_Mobile;
        private readonly WhosMostHumbleQuest m_Quest;
        private readonly int m_NPCIndex;

        public HumilityItemQuestGump(HumilityQuestMobile mobile, WhosMostHumbleQuest quest, int index) : base(50, 50)
        {
            m_Mobile = mobile;
            m_Quest = quest;
            m_NPCIndex = index;

            AddBackground(0, 0, 350, 250, 2600);
            AddHtml(100, 25, 175, 16, string.Format("{0} {1}", mobile.Name, mobile.Title), false, false);

            AddHtmlLocalized(40, 60, 270, 140, mobile.Greeting + 1, 1, false, true);
        }

        public override void OnResponse(NetState state, RelayInfo info)
        {
            if (m_NPCIndex < 0 || m_NPCIndex >= m_Quest.Infos.Count)
                return;

            Mobile from = state.Mobile;

            int cliloc;
            string args;

            if (0.5 > Utility.RandomDouble() || m_NPCIndex == 6)
            {
                cliloc = m_Mobile.Greeting + 2;
                args = string.Format("#{0}", m_Quest.Infos[m_NPCIndex].NeedsLoc);
            }
            else
            {
                cliloc = m_Mobile.Greeting + 3;
                args = string.Format("#{0}", m_Quest.Infos[m_NPCIndex].GivesLoc);
            }

            m_Mobile.SayTo(from, cliloc, args);
        }
    }
}