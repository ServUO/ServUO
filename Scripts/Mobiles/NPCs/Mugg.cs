using System;
using Server.Items;

namespace Server.Engines.Quests
{
    public class MoreOrePleaseQuest : BaseQuest
    { 
        public MoreOrePleaseQuest()
            : base()
        { 
            this.AddObjective(new ObtainObjective(typeof(IronOre), "iron ore", 5, 0x19B9));
						
            this.AddReward(new BaseReward(typeof(MinersSatchel), 1074282)); // Craftsman's Satchel
        }

        /* More Ore Please */
        public override object Title
        {
            get
            {
                return 1075530;
            }
        }
        /* Have a pickaxe? My supplier is late and I need some iron ore so I can complete a bulk order for another 
        merchant. If you can get me some soon I'll pay you double what it's worth on the market. Just find a cave 
        or mountainside and try to use your pickaxe there, maybe you'll strike a good vein! 5 large pieces should 
        do it. */
        public override object Description
        {
            get
            {
                return 1075529;
            }
        }
        /* Not feeling strong enough today? Its alright, I didn't need a bucket of rocks anyway. */
        public override object Refuse
        {
            get
            {
                return 1075531;
            }
        }
        /* Hmmm… we need some more Ore. Try finding a mountain or cave, and give it a whack. */
        public override object Uncomplete
        {
            get
            {
                return 1075532;
            }
        }
        /* I see you found a good vien! Great!  This will help get this order out on time. Good work! */
        public override object Complete
        {
            get
            {
                return 1075533;
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

    public class Mugg : MondainQuester
    {
        [Constructable]
        public Mugg()
            : base("Mugg", "the miner")
        { 
        }

        public Mugg(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests
        { 
            get
            {
                return new Type[] 
                {
                    typeof(MoreOrePleaseQuest)
                };
            }
        }
        public override void InitBody()
        {
            this.InitStats(100, 100, 25);
			
            this.Female = false;
            this.CantWalk = true;
            this.Race = Race.Human;
			
            this.Hue = 0x840A;
            this.HairItemID = 0x2047;
            this.HairHue = 0x0;
        }

        public override void InitOutfit()
        {
            this.AddItem(new Backpack());			
            this.AddItem(new Pickaxe());
            this.AddItem(new Boots(0x901));
            this.AddItem(new ShortPants(0x3B2));
            this.AddItem(new Shirt(0x22B));
            this.AddItem(new SkullCap(0x177));
            this.AddItem(new HalfApron(0x5F1));
			
            Item item;
			
            item = new PlateGloves();
            item.Hue = 0x21E;
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

    public class MinersSatchel : Backpack
    {
        [Constructable]
        public MinersSatchel()
            : base()
        {
            this.Hue = BaseReward.SatchelHue();
			
            this.AddItem(new Pickaxe());
            this.AddItem(new Pickaxe());
        }

        public MinersSatchel(Serial serial)
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