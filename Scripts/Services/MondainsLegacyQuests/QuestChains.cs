using System;

namespace Server.Engines.Quests
{
    public enum QuestChain
    {
        None,
        Aemaeth,
        AncientWorld,
        BlightedGrove,
        CovetousGhost,
        GemkeeperWarriors,
        HonestBeggar,
        LibraryFriends,
        Marauders,
        MiniBoss,
        SummonFey,
        SummonFiend,
        TuitionReimbursement,
        Spellweaving,
        SpellweavingS,
        UnfadingMemories,
        PercolemTheHunter,
        KingVernixQuests,
        DoughtyWarriors,
        HonorOfDeBoors,
        LaifemTheWeaver,
        CloakOfHumility
    }

    public class BaseChain
    {
        private Type m_CurrentQuest;
        private Type m_Quester;
        public BaseChain(Type currentQuest, Type quester)
        {
            this.m_CurrentQuest = currentQuest;
            this.m_Quester = quester;
        }

        public Type CurrentQuest
        {
            get
            {
                return this.m_CurrentQuest;
            }
            set
            {
                this.m_CurrentQuest = value;
            }
        }
        public Type Quester
        {
            get
            {
                return this.m_Quester;
            }
            set
            {
                this.m_Quester = value;
            }
        }
    }
}