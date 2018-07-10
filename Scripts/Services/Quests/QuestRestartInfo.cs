using System;

namespace Server.Engines.Quests
{
    public class QuestRestartInfo
    {
        private Type m_QuestType;
        private DateTime m_RestartTime;

        public QuestRestartInfo(Type questType, TimeSpan restartDelay)
        {
            m_QuestType = questType;
            Reset(restartDelay);
        }

        public QuestRestartInfo(Type questType, DateTime restartTime)
        {
            m_QuestType = questType;
            m_RestartTime = restartTime;
        }

        public Type QuestType
        {
            get
            {
                return m_QuestType;
            }
            set
            {
                m_QuestType = value;
            }
        }
        public DateTime RestartTime
        {
            get
            {
                return m_RestartTime;
            }
            set
            {
                m_RestartTime = value;
            }
        }
        public void Reset(TimeSpan restartDelay)
        {
            if (restartDelay < TimeSpan.MaxValue)
                m_RestartTime = DateTime.UtcNow + restartDelay;
            else
                m_RestartTime = DateTime.MaxValue;
        }
    }
}