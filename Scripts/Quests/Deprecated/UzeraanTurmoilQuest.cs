using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Haven
{
    public class UzeraanTurmoilQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {

        };

        public UzeraanTurmoilQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public UzeraanTurmoilQuest()
        {
        }

        public override Type[] TypeReferenceTable => m_TypeReferenceTable;
        public override object Name => 1049007; // "Uzeraan's Turmoil"
        public override object OfferMessage => 1049008;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15C9; // warrior
    }
}