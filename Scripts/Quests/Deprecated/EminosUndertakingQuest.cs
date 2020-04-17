using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Ninja
{
    public class EminosUndertakingQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {

        };

        public EminosUndertakingQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public EminosUndertakingQuest()
        {
        }

        public override Type[] TypeReferenceTable => m_TypeReferenceTable;
        public override object Name => 1063173; // Emino's Undertaking
        public override object OfferMessage => 1063174;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15D5;
    }
}