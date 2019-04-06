using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class SplitEndsQuest : BaseQuest
    { 
        public SplitEndsQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(Arrow), "arrow", 20, 0xF3F));
						
            this.AddReward(new BaseReward(typeof(FletchersSatchel), 1074282)); // Craftsman's Satchel
        }

        /* Split Ends */
        public override object Title
        {
            get
            {
                return 1075506;
            }
        }
        /* *sighs* I think bowcrafting is a might beyond my talents. Say there, you look a bit more confident with tools. 
        Can I persuade thee to make a few arrows? You could have my satchel in return... 'tis useless to me! You'll need a 
        fletching kit to start, some feathers, and a few arrow shafts. Just use the fletching kit while you have the other 
        things, and I'm sure you'll figure out the rest. */
        public override object Description
        {
            get
            {
                return 1075507;
            }
        }
        /* Oh. Well. I'll just keep trying alone, I suppose... */
        public override object Refuse
        {
            get
            {
                return 1075508;
            }
        }
        /* You're not quite done yet.  Get back to work! */
        public override object Uncomplete
        {
            get
            {
                return 1072271;
            }
        }
        /* Thanks for helping me out.  Here's the reward I promised you. */
        public override object Complete
        {
            get
            {
                return 1072272;
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

    public class Andric : MondainQuester
    {
        [Constructable]
        public Andric()
            : base("Andric", "the archer trainer")
        {
            this.SetSkill(SkillName.Archery, 65.0, 88.0);
        }

        public Andric(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(SplitEndsQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x8407;			
            this.HairItemID = 0x2049;
            this.HairHue = 0x6CE;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new Boots(0x1BB));
						
            Item item;
			
            item = new LeatherLegs();
            item.Hue = 0x6C8;
            this.AddItem(item);
			
            item = new LeatherGloves();
            item.Hue = 0x1BB;
            this.AddItem(item);
			
            item = new LeatherChest();
            item.Hue = 0x1BB;
            this.AddItem(item);
			
            item = new LeatherArms();
            item.Hue = 0x4C7;
            this.AddItem(item);			
			
            item = new CompositeBow();
            item.Hue = 0x5DD;
            this.AddItem(item);
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

    public class FletchersSatchel : Backpack
    {
        [Constructable]
        public FletchersSatchel()
            : base()
        {
            this.Hue = BaseReward.SatchelHue();
			
            this.AddItem(new Feather(10));
            this.AddItem(new FletcherTools());
        }

        public FletchersSatchel(Serial serial)
            : base(serial)
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