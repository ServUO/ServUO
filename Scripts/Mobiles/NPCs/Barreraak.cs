using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class SomethingFishy : BaseQuest
    {
        /* SomethingFishy */
        public override object Title => 1095059;

        public override object Description => 1095043;

        public override object Refuse => 1095044;

        public override object Uncomplete => 1095045;

        public override object Complete => 1095048;

        public SomethingFishy() : base()
        {
            AddObjective(new ObtainObjective(typeof(RedHerring), "Red Herring", 1, 0x9cc));

            AddReward(new BaseReward(typeof(BarreraaksRing), 1095049));
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

    public class Barreraak : MondainQuester
    {
        public override Type[] Quests => new Type[]
            {
                typeof( SomethingFishy )
            };

        [Constructable]
        public Barreraak()
            : base()
        {
            Name = "Barreraak";
        }

        public override void InitBody()
        {
            HairItemID = 0x2044;//
            HairHue = 1153;
            FacialHairItemID = 0x204B;
            FacialHairHue = 1153;
            Body = 334;
            Blessed = true;
        }

        public override void InitOutfit()
        {
            AddItem(new Backpack());
            AddItem(new Boots());
            AddItem(new LongPants(0x6C7));
            AddItem(new FancyShirt(0x6BB));
            AddItem(new Cloak(0x59));
        }

        public Barreraak(Serial serial)
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