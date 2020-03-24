using System;
using Server.Mobiles;

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
                return 1060095; // Dark Tides
            }
        }
        public override object OfferMessage
        {
            get
            {                
                return 1060094;
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
                return 0x15B5;
            }
        }        
    }
}