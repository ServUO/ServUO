using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Necro
{
    public class DarkTidesQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {

        };
        public DarkTidesQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public DarkTidesQuest()
        {
        }

        public override Type[] TypeReferenceTable => m_TypeReferenceTable;
        public override object Name => 1060095; // Dark Tides
        public override object OfferMessage => 1060094;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15B5;
    }
}