using System;

namespace Server.Engines.Quests
{
    public enum ExploringTheDeepQuestChain
    {
        None,
        HeplerPaulson,
        HeplerPaulsonComplete,
        CusteauPerronHouse,
        CusteauPerron,
        Sorcerers,
        CollectTheComponent,
        CollectTheComponentComplete
    }

    public class ExploringTheDeepBaseChain
    {
        private Type m_CurrentQuest;

        public ExploringTheDeepBaseChain(Type currentQuest, Type quester)
        {
            this.m_CurrentQuest = currentQuest;
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
    }
}