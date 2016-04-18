using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{ 
    public class GuileIrkAndSpiteQuest : BaseQuest
    { 
        public GuileIrkAndSpiteQuest()
            : base()
        { 
            this.AddObjective(new SlayObjective(typeof(Guile), "guile", 1));
            this.AddObjective(new SlayObjective(typeof(Irk), "irk", 1));
            this.AddObjective(new SlayObjective(typeof(Spite), "spite", 1));
			
            this.AddReward(new BaseReward(typeof(RewardBox), 1072584));
        }

        /* Guile, Irk and Spite */
        public override object Title
        {
            get
            {
                return 1074739;
            }
        }
        /* You know them, don't you.  The three?  They look like you, you'll see. They looked like me, I remember, they 
        looked like, well, you'll see.  The three.  They'll drive you mad too, if you let them.  They are trouble, and 
        they need to be slain.  Seek them out. */
        public override object Description
        {
            get
            {
                return 1074740;
            }
        }
        /* You just don't understand the gravity of the situation.  If you did, you'd agree to my task. */
        public override object Refuse
        {
            get
            {
                return 1074745;
            }
        }
        /* Perhaps I was unclear.  You'll know them when you see them, because you'll see you, and you, and you.  Hurry now. */
        public override object Uncomplete
        {
            get
            {
                return 1074746;
            }
        }
        /* Are you one of THEM?  Ahhhh!  Oh, wait, if you were them, then you'd be me.  So you're -- you.  Good job! */
        public override object Complete
        {
            get
            {
                return 1074747;
            }
        }
        public override bool CanOffer()
        {
            return MondainsLegacy.TwistedWeald;
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

    public class Yorus : MondainQuester
    {
        [Constructable]
        public Yorus()
            : base("Yorus", "the tinker")
        { 
        }

        public Yorus(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        {
            get
            {
                return new Type[] 
                {
                    typeof(BullfightingSortOfQuest),
                    typeof(ForcedMigrationQuest),
                    typeof(FineFeastQuest),
                    typeof(OverpopulationQuest),
                    typeof(HeroInTheMakingQuest),
                    typeof(ThinningTheHerdQuest),
                    typeof(TheyllEatAnythingQuest),
                    typeof(NoGoodFishStealingQuest),
                    typeof(WildBoarCullQuest),
                    typeof(TheyreBreedingLikeRabbitsQuest),
                    typeof(GuileIrkAndSpiteQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.Race = Race.Human;
			
            this.Hue = 0x841D;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());
            this.AddItem(new Shoes(0x755));
            this.AddItem(new LongPants(0x1BB));
            this.AddItem(new FancyShirt(0x71E));
            this.AddItem(new Cloak(0x59));
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