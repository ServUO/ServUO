using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class StitchInTimeQuest : BaseQuest
    { 
        public StitchInTimeQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(FancyDress), "fancy dress", 1, 0x1EFF));
						
            this.AddReward(new BaseReward(typeof(OldRing), 1075524)); // an old ring
            this.AddReward(new BaseReward(typeof(OldNecklace), 1075525)); // an old necklace
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(2);
            }
        }
        /* A Stitch in Time */
        public override object Title
        {
            get
            {
                return 1075523;
            }
        }
        /* Oh how I wish I had a fancy dress like the noble ladies of Castle British! I don't have much... but I 
        have a few trinkets I might trade for it. It would mean the world to me to go to a fancy ball and dance 
        the night away. Oh, and I could tell you how to make one! You just need to use your sewing kit on enough 
        cut cloth, that's all. */
        public override object Description
        {
            get
            {
                return 1075522;
            }
        }
        /* Won't you reconsider? It'd mean the world to me, it would! */
        public override object Refuse
        {
            get
            {
                return 1075526;
            }
        }
        /* Hello again! Do you need anything? You may want to visit the tailor's shop for cloth and a sewing kit, 
        if you don't already have them. */
        public override object Uncomplete
        {
            get
            {
                return 1075527;
            }
        }
        /* It's gorgeous! I only have a few things to give you in return, but I can't thank you enough! Maybe I'll 
        even catch Uzeraan's eye at the, er, *blushes* I mean, I can't wait to wear it to the next town dance! */
        public override object Complete
        {
            get
            {
                return 1075528;
            }
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

    public class Clairesse : MondainQuester
    {
        [Constructable]
        public Clairesse()
            : base("Clairesse", "the servant")
        { 
        }

        public Clairesse(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(StitchInTimeQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = true;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x840B;			
            this.HairItemID = 0x203D;
            this.HairHue = 0x458;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new PlainDress(0x3C9));
            this.AddItem(new Shoes(0x740));
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

    public class OldRing : GoldRing
    {
        [Constructable]
        public OldRing()
            : base()
        {
            this.Hue = 0x222;
        }

        public OldRing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075524;
            }
        }// an old ring
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

    public class OldNecklace : Necklace
    {
        [Constructable]
        public OldNecklace()
            : base()
        {
            this.Hue = 0x222;
        }

        public OldNecklace(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075525;
            }
        }// an old necklace
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