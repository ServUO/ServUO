using Server;
using System;
using Server.Mobiles;
using Server.Gumps;
using Server.Engines.Points;

namespace Server.Engines.Khaldun
{
	public class GoingGumpshoeQuest : BaseQuest
	{
        /* Going Gumshoe */
        public override object Title { get { return 1158588; } }

        /*You've heard rumblings of Pagan Cultists causing petty crimes in the more upscale sections of Britannia's cities,
         * but to your understanding these are sympathizers taking advantage of fear to make some easy coin. Still, if the
         * Crown is rolling out a new division of the RBG to investigate...something...you bet there's an opportunity for 
         * you! You should find Inspector Jasper and inquire further.*/
        public override object Description { get { return 1158589; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /* Find Inspector Jasper, and inquire about the Town Cryer article at the new Detective Branch in East Britain */
        public override object Uncomplete { get { return 1158590; } }

        public override object Complete { get { return null; } }

        public override int AcceptSound { get { return 0x2E8; } }
        public override int CompleteMessage { get { return 1158616; } } // // You've found Chief Inspector Jasper! Speak to him to learn more!

        public GoingGumpshoeQuest()
		{
            AddReward(new BaseReward(1158615)); // A unique opportunity with the Detective Branch
		}

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class InternalObjective : BaseObjective
        {
            public override object ObjectiveDescription { get { return Quest.Uncomplete; } }

            public InternalObjective()
                : base(1)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
            }
        }
	}

    public class GoingGumpshoeQuest2 : BaseQuest
    {
        /* Going Gumshoe */
        public override object Title { get { return 1158588; } }

        /*You can drop the coffee pods with the investigators outside...*looks up*...oh, you aren't here for the delivery? I warned 
         * them talking with reporters was a terrible idea - we've been getting walk-ins like you since the article broke, lads 
         * and ladies thinking they can become renowned detectives - Ha! Think you've got the intellect? The cunning and wisdom
         * to sniff a case where it leads you? Well I guess we'll see about that. I've got bigger cases to deal with, you can 
         * take this one. Something about vandalism and a funeral at the Britain Cemetery. Report back to me if you find anything
         * worthwhile - and I do mean worthwhile! Don't come back to me with half baked theories and bogus evidence. I need facts! 
         * Oh - and read this, you'll need it if you even hope to break a single case.*/
        public override object Description { get { return 1158592; } }

        /* You decide against accepting the quest. */
        public override object Refuse { get { return 1158130; } }

        /*What are you still doing here? Didn't I send you to the cemetery? */
        public override object Uncomplete { get { return 1158594; } }

        public override object Complete { get { return null; } }

        public override int AcceptSound { get { return 0x2E8; } }
        public override int CompleteMessage { get { return 1158595; } } // TOIDO: This

        public bool SentMessage { get; set; }

        public GoingGumpshoeQuest2()
        {
            AddReward(new BaseReward(1158615)); // A unique opportunity with the Detective Branch
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(SentMessage);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            SendMessage = reader.ReadBool();
        }

        private class InternalObjective : BaseObjective
        {
            /* You have been assigned as a probationary investigator with the Detective Branch of the RBG. Pursue leads and follow the clues where they lead you... */
            public override object ObjectiveDescription { get { return 1158593; } }

            public InternalObjective()
                : base(1)
            {
            }

            public override void Serialize(GenericWriter writer)
            {
                base.Serialize(writer);

                writer.Write((int)0); // version
            }

            public override void Deserialize(GenericReader reader)
            {
                base.Deserialize(reader);

                int version = reader.ReadInt();
            }
        }
    }
}
