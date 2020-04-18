using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class RumorsAboundQuest : BaseQuest
    {
        public RumorsAboundQuest()
            : base()
        {
            AddObjective(new DeliverObjective(typeof(EgwexemWrit), "Egwexem's Writ", 1, typeof(Naxatilor), "Naxatilor"));

            AddReward(new BaseReward(1112731));
        }

        public override TimeSpan RestartDelay => TimeSpan.FromHours(12);

        public override bool DoneOnce => true;

        /* Rumors Abound */
        public override object Title => 1112514;
        public override object Description => 1112515;
        public override object Refuse => 1112516;
        public override object Uncomplete => "You never spoke to Naxatillor yet! Go to him!";

        public override object Complete => 1112518;
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

    public class Egwexem : MondainQuester
    {
        [Constructable]
        public Egwexem()
            : base("Egwexem", "the Noble")
        {
        }

        public Egwexem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new Type[]
                {
                    typeof(RumorsAboundQuest)
                };
        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            CantWalk = true;
            Body = 666;
            HairItemID = 16987;
            HairHue = 1801;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new GargishClothChest());
            AddItem(new GargishClothKilt());
            AddItem(new GargishClothLegs(Utility.RandomNeutralHue()));
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

    public class EgwexemWrit : Item
    {
        [Constructable]
        public EgwexemWrit()
            : base(0x0E34)
        {
            //Hue = 3;
        }

        public EgwexemWrit(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112520;
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