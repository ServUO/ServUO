using Server.Items;
using System;

namespace Server.Engines.Quests
{
    public class ABrokenVaseQuest : BaseQuest
    {
        public ABrokenVaseQuest()

        {
            AddObjective(new ObtainObjective(typeof(AncientPotteryFragments), "Ancient Pottery Fragments", 10, 0x223B, 0, 2108));

            AddReward(new BaseReward(typeof(MeagerMuseumBag), 1112993));
            AddReward(new BaseReward("Loyalty Rating"));
        }

        /*A Broken Vase */

        public override object Title => 1112795;

        public override object Description => 1112917;

        public override object Refuse => 1112918;

        public override object Uncomplete => 1112919;

        public override object Complete => 1112920;

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

    public class PuttingThePiecesTogetherQuest : BaseQuest
    {
        public PuttingThePiecesTogetherQuest()
        {
            AddObjective(new ObtainObjective(typeof(TatteredAncientScroll), "Tattered Ancient Scrolls", 5, 0x1437));

            AddReward(new BaseReward(typeof(DustyMuseumBag), 1112994));
            AddReward(new BaseReward("Loyalty Rating"));
        }

        /* Putting The Pieces Together */

        public override object Title => 1112796;

        public override object Description => 1112921;

        public override object Refuse => 1112922;

        public override object Uncomplete => 1112923;

        public override object Complete => 1112924;

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

    public class YeOldeGargishQuest : BaseQuest
    {
        public YeOldeGargishQuest()
        {
            AddObjective(new ObtainObjective(typeof(UntranslatedAncientTome), "Untranslated Ancient Tome", 1, 0xFF2, 0, 2405));

            AddReward(new BaseReward(typeof(BulgingMuseumBag), 1112995));
            AddReward(new BaseReward("Loyalty Rating"));
        }

        /* Ye Olde Gargish */

        public override object Title => 1112797;

        public override object Description => 1112925;

        public override object Refuse => 1112926;

        public override object Uncomplete => 1112927;

        public override object Complete => 1112928;

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

    public class Axem : MondainQuester
    {
        [Constructable]
        public Axem()
            : base("Axem", "the Curator")
        {
            SetSkill(SkillName.Meditation, 60.0, 83.0);
            SetSkill(SkillName.Focus, 60.0, 83.0);
        }

        public Axem(Serial serial)
            : base(serial)
        {
        }

        public override Type[] Quests => new[]
                {
                    typeof (ABrokenVaseQuest),
                    typeof (PuttingThePiecesTogetherQuest),
                    typeof (YeOldeGargishQuest)
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

            AddItem(new GargishClothChest(Utility.RandomNeutralHue()));
            AddItem(new GargishClothKilt(Utility.RandomNeutralHue()));
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
}