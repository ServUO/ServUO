using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class DeliciousFishesQuest : BaseQuest
    { 
        public DeliciousFishesQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Fish), "fish", 5, 0x9CC));
						
            this.AddReward(new BaseReward(typeof(PeppercornFishsteak), 3, 1075557)); // peppercorn fishsteak
        }

        public override TimeSpan RestartDelay
        {
            get
            {
                return TimeSpan.FromMinutes(3);
            }
        }
        /* Delicious Fishes */
        public override object Title
        {
            get
            {
                return 1075555;
            }
        }
        /* Ello there, looking for a good place on the dock to fish? I like the southeast corner meself. 
        What's that? Oh, no, *sighs* me pole is broken and in for fixin'. My grandpappy gave me that pole, 
        means a lot you see. Miss the taste of fish though... Oh say, since you're here, could you catch 
        me a few fish? I can cook a mean fish steak, and I'll split 'em with you! But make sure it's one 
        of the green kind, they're the best for seasoning! */
        public override object Description
        {
            get
            {
                return 1075556;
            }
        }
        /* Ah, you're missin' out my friend, you're missing out. My peppercorn fishsteaks are famous on 
        this little isle of ours! */
        public override object Refuse
        {
            get
            {
                return 1075558;
            }
        }
        /* Eh? Find yerself a pole and get close to some water. Just toss the line on in and hopefully you 
        won't snag someone's old boots! Remember, that's twenty of them green fish we'll be needin', so come 
        back when you've got em, 'aight? */
        public override object Uncomplete
        {
            get
            {
                return 1075559;
            }
        }
        /* Just a moment my friend, just a moment! *rummages in his pack* Here we are! My secret blend of 
        peppers always does the trick, never fails, no not once. These'll fill you up much faster than that 
        tripe they sell in the market! */
        public override object Complete
        {
            get
            {
                return 1075560;
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

    public class Norton : MondainQuester
    {
        [Constructable]
        public Norton()
            : base("Norton", "the fisher")
        { 
        }

        public Norton(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(DeliciousFishesQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x83F8;
            this.HairItemID = 0x203B;
            this.HairHue = 0x472;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());		
            this.AddItem(new ThighBoots());
            this.AddItem(new Shirt(0x11D));
            this.AddItem(new LongPants(0x6C2));
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

    public class PeppercornFishsteak : FishSteak
    {
        [Constructable]
        public PeppercornFishsteak()
            : base()
        {
        }

        public PeppercornFishsteak(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1075557;
            }
        }// peppercorn fishsteak
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