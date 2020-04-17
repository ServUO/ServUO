using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class ScrapingtheBottom : BaseQuest
    {
        /* SomethingFishy */
        public override object Title => 1095059;

        public override object Description => 1095061;

        public override object Refuse => 1095062;

        public override object Uncomplete => 1095063;

        public override object Complete => 1095065;

        public ScrapingtheBottom() : base()
        {
            AddObjective(new ObtainObjective(typeof(MudPuppy), "Mud Puppy", 1, 0x9cc));

            AddReward(new BaseReward(typeof(XenrrFishingPole), 1095066));
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

    public class Xenrr : MondainQuester
    {
        public override Type[] Quests => new Type[]
            {
                typeof( ScrapingtheBottom )
            };

        [Constructable]
        public Xenrr()
            : base()
        {
            Name = "Xenrr";
        }

        public override void InitBody()
        {
            HairItemID = 0x2044;//
            HairHue = 1153;
            FacialHairItemID = 0x204B;
            FacialHairHue = 1153;
            Body = 723;
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

        public Xenrr(Serial serial)
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