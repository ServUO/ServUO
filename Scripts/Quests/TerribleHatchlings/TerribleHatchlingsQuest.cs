using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Zento
{
    public class TerribleHatchlingsQuest : QuestSystem
    {
        public TerribleHatchlingsQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public TerribleHatchlingsQuest()
        {
        }

        public override object Name =>
                // Terrible Hatchlings
                1063314;
        public override object OfferMessage =>
                /* The Deathwatch Beetle Hatchlings have trampled through my fields
* again, what a nuisance! Please help me get rid of the terrible
* hatchlings. If you kill 10 of them, you will be rewarded.
* The Deathwatch Beetle Hatchlings live in The Waste -
* the desert close to this city.<BR><BR>
* 
* Will you accept this challenge?
*/
                1063315;
        public override TimeSpan RestartDelay => TimeSpan.MaxValue;
        public override bool IsTutorial => true;
        public override int Picture => 0x15CF;
        public override void Accept()
        {
            base.Accept();

            AddConversation(new AcceptConversation());
        }
    }
}
