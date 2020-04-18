using Server.Mobiles;
using System;

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

        public override Type[] TypeReferenceTable => m_TypeReferenceTable;
        public override object Name => 1063022; // Haochi's Trials
        public override object OfferMessage => 1063023;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15D7;
    }
}