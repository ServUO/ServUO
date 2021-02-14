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

        public ExploringTheDeepBaseChain(Type currentQuest)
        {
            m_CurrentQuest = currentQuest;
        }

        public Type CurrentQuest { get => m_CurrentQuest; set => m_CurrentQuest = value; }
    }
}
