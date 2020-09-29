using Server.Mobiles;
using System;

namespace Server.Engines.Quests.Collector
{
    public class CollectorQuest : QuestSystem
    {
        public CollectorQuest(PlayerMobile from)
            : base(from)
        {
        }

        // Serialization
        public CollectorQuest()
        {
        }

        public override object Name => 1020549; // Fishing for Rainbow Pearls
        public override object OfferMessage =>
                /* <I>Elwood greets you warmly, like an old friend he's not
* quite sure he ever had.</I><BR><BR>
* 
* Hello. Yes. Sit down. Please. Good. Okay, stand. Up to you.<BR><BR>
* 
* So, what brings you to these parts... hey, wait. Just had a thought.
* Would you like to do me a favor? Yes, really. You know, for old
* times sake. The good ole days. You were always one of my best
* suppliers. Or maybe you weren't, who knows any more. Anyway,
* could use some help supplementing my stock. You know me. Always
* looking for something new to add to the collection. Or sometimes
* not so new - just more of the same. But don't have to tell you that.
* You know, don't you. Yes. Just like old times. That's what it'll be.
* You and me - together again. Ah, it's been too long.<BR><BR>
* 
* So what do you think? The fee will be the same as always. I'm a fair
* man. You know that. So what do you say?
*/
                1055081;
        public override TimeSpan RestartDelay => TimeSpan.Zero;
        public override bool IsTutorial => false;
        public override int Picture => 0x15A9;
        public override void Accept()
        {
            base.Accept();

            AddConversation(new AcceptConversation());
        }

        public override void Decline()
        {
            base.Decline();

            AddConversation(new DeclineConversation());
        }
    }
}
