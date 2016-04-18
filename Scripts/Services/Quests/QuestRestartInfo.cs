using System;

namespace Server.Engines.Quests
{
    public class QuestRestartInfo
    {
        private Type m_QuestType;
        private DateTime m_RestartTime;
        public QuestRestartInfo(Type questType, TimeSpan restartDelay)
        {
            this.m_QuestType = questType;
            this.Reset(restartDelay);
        }

        public QuestRestartInfo(Type questType, DateTime restartTime)
        {
            this.m_QuestType = questType;
            this.m_RestartTime = restartTime;
        }

        public Type QuestType
        {
            get
            {
                return this.m_QuestType;
            }
            set
            {
                this.m_QuestType = value;
            }
        }
        public DateTime RestartTime
        {
            get
            {
                return this.m_RestartTime;
            }
            set
            {
                this.m_RestartTime = value;
            }
        }
        public void Reset(TimeSpan restartDelay)
        {
            if (restartDelay < TimeSpan.MaxValue)
                this.m_RestartTime = DateTime.UtcNow + restartDelay;
            else
                this.m_RestartTime = DateTime.MaxValue;
        }
    }
}