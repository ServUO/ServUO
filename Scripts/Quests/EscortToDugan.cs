using Server.Items;

namespace Server.Engines.Quests
{
    public class EscortToDugan : BaseQuest
    {
        public EscortToDugan()
            : base()
        {
            AddObjective(new EscortObjective(1113623, "NPC Encampment")); // Human settler camp entrance

            AddReward(new BaseReward(typeof(TalismanofGoblinSlaying), "Talisman of Goblin Slaying"));
        }

        public override bool DoneOnce => true;

        /* The Lost Brightwhistle */
        public override object Title => 1095003;

        /* Escort Neville Brightwhistle to Elder Dugan. After Neville is safe, speak
		 * to Elder Dugan for your reward.
		 * 
		 * I was separated from my brothers when the goblins attacked. I am a member
		 * of the Society of Ariel Haven, come to colonize these halls that we had
		 * though abandoned. I must get out of here and warn Elder Dugan that these
		 * creatures live here and are very dangerous! Will you show me the way out? */
        public override object Description => 1095005;

        /* Oh, have mercy on me!  I will have to make it on my own. */
        public override object Refuse => 1095006;

        /* Is it much farther to the camp? */
        public override object Uncomplete => 1095007;

        /* You have done me and my people a great service, traveler. I had assumed
		 * the worst had befallen Neville and I do not doubt it would have soon if
		 * you had not intervened.
		 * 
		 * The goblins are a creature of legend; it was thought that they had all
		 * been killed. They are not as strong as they are evil, but their blight
		 * can become a plague. Take this talisman, it was worn by those sent to
		 * end the previous goblin menace. It has magical powers and will aid you
		 * in defending yourself against the goblins. I know where I can get more
		 * of them and I will send for some for the rest of my people. */
        public override object Complete => 1095008;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
