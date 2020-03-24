using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Samurai
{
    public class HaochisTrialsQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            
        };

        public HaochisTrialsQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public HaochisTrialsQuest()
        {
        }

        public override Type[] TypeReferenceTable
        {
            get
            {
                return m_TypeReferenceTable;
            }
        }
        public override object Name
        {
            get
            {               
                return 1063022; // Haochi's Trials
            }
        }
        public override object OfferMessage
        {
            get
            {               
                return 1063023;
            }
        }
        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.MaxValue;
            }
        }
        public override bool IsTutorial
        {
            get
            {
                return true;
            }
        }
        public override int Picture
        {
            get
            {
                return 0x15D7;
            }
        }               
    }
}