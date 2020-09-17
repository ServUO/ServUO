using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class MoreOrePleaseQuest : BaseQuest
    {
        public MoreOrePleaseQuest()
            : base()
        {
            AddObjective(new ObtainObjective(typeof(IronOre), "iron ore", 5, 0x19B9));

            AddReward(new BaseReward(typeof(MinersQuestSatchel), 1074282)); // Craftsman's Satchel
        }

        /* More Ore Please */
        public override object Title => 1075530;
        /* Have a pickaxe? My supplier is late and I need some iron ore so I can complete a bulk order for another 
        merchant. If you can get me some soon I'll pay you double what it's worth on the market. Just find a cave 
        or mountainside and try to use your pickaxe there, maybe you'll strike a good vein! 5 large pieces should 
        do it. */
        public override object Description => 1075529;
        /* Not feeling strong enough today? Its alright, I didn't need a bucket of rocks anyway. */
        public override object Refuse => 1075531;
        /* Hmmm… we need some more Ore. Try finding a mountain or cave, and give it a whack. */
        public override object Uncomplete => 1075532;
        /* I see you found a good vien! Great!  This will help get this order out on time. Good work! */
        public override object Complete => 1075533;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
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

        public override Type[] Quests => new Type[]
                {
                    typeof(MoreOrePleaseQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Race = Race.Human;

            Hue = 0x840A;
            HairItemID = 0x2047;
            HairHue = 0x0;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Pickaxe());
            AddItem(new Boots(0x901));
            AddItem(new ShortPants(0x3B2));
            AddItem(new Shirt(0x22B));
            AddItem(new SkullCap(0x177));
            AddItem(new HalfApron(0x5F1));

            Item item;

            item = new PlateGloves
            {
                Hue = 0x21E
            };
            AddItem(item);
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    [TypeAlias("Server.Engines.Quests.MinersSatchel")]
    public class MinersQuestSatchel : Backpack
    {
        [Constructable]
        public MinersQuestSatchel()
            : base()
        {
            Hue = BaseReward.SatchelHue();

            AddItem(new Pickaxe());
            AddItem(new Pickaxe());
        }

        public MinersQuestSatchel(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}