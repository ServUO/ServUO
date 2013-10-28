using System;
using Server.Mobiles;

namespace Server.Engines.Quests.Zento
{
    public class TerribleHatchlingsQuest : QuestSystem
    {
        private static readonly Type[] m_TypeReferenceTable = new Type[]
        {
            typeof(AcceptConversation),
            typeof(DirectionConversation),
            typeof(TakeCareConversation),
            typeof(EndConversation),
            typeof(FirstKillObjective),
            typeof(SecondKillObjective),
            typeof(ThirdKillObjective),
            typeof(ReturnObjective)
        };
        public TerribleHatchlingsQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public TerribleHatchlingsQuest()
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
                // Terrible Hatchlings
                return 1063314;
            }
        }
        public override object OfferMessage
        {
            get
            {
                /* The Deathwatch Beetle Hatchlings have trampled through my fields
                * again, what a nuisance! Please help me get rid of the terrible
                * hatchlings. If you kill 10 of them, you will be rewarded.
                * The Deathwatch Beetle Hatchlings live in The Waste -
                * the desert close to this city.<BR><BR>
                * 
                * Will you accept this challenge?
                */
                return 1063315;
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
                return 0x15CF;
            }
        }
        public override void Accept()
        {
            base.Accept();

            this.AddConversation(new AcceptConversation());
        }
    }
}