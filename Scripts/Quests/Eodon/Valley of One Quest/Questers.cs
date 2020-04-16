using Server.Engines.Quests;
using Server.Items;
using System;

namespace Server.Mobiles
{
    public class Hawkwind2 : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(TimeIsOfTheEssenceQuest) };

        [Constructable]
        public Hawkwind2()
            : base("Hawkwind", "the Time Lord")
        {
        }

        public Hawkwind2(Serial serial)
            : base(serial)
        {
        }

        public override void OnTalk(PlayerMobile player)
        {
            if (QuestHelper.HasQuest(player, typeof(UnitingTheTribesQuest)))
            {
                OnOfferFailed();
                return;
            }

            base.OnTalk(player);
        }

        public override void Advertise()
        {
            Say(1156465); // The situation is dire, King Blackthorn. I fear only a courageous adventurer can aid us...
        }

        public override void InitBody()
        {
            Body = 689;
            InitStats(100, 100, 100);
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

    public class SirGeoffery : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(UnitingTheTribesQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public SirGeoffery()
            : base("Sir Geoffrey", "the Guardsman")
        {
        }

        public override void InitBody()
        {
            InitStats(125, 150, 25);

            Female = false;
            Body = 0x190;

            FacialHairItemID = 8267;
            HairItemID = Race.RandomHair(false);
            HairHue = Race.RandomHairHue();

            Hue = Utility.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new PlateArms());
            SetWearable(new PlateChest());
            SetWearable(new Doublet(), 1702);
            SetWearable(new BodySash(), 437);
            SetWearable(new ChainLegs());
            SetWearable(new PlateGorget());
            SetWearable(new Helmet());
            SetWearable(new Halberd());
            SetWearable(new ShortPants(), 2305);
            SetWearable(new Shoes(), 2305);
        }

        public SirGeoffery(Serial serial)
            : base(serial)
        {
        }

        public override void OnTalk(PlayerMobile player)
        {
            TimeIsOfTheEssenceQuest q = QuestHelper.GetQuest(player, typeof(TimeIsOfTheEssenceQuest)) as TimeIsOfTheEssenceQuest;

            if (QuestHelper.HasQuest(player, typeof(TimeIsOfTheEssenceQuest)) || QuestHelper.HasQuest(player, typeof(UnitingTheTribesQuest)))
            {
                base.OnTalk(player);
            }
            else
            {
                Advertise();
            }
        }

        public override void Advertise()
        {
            Say(1156466); // Get these supplies unloaded! Set up a perimeter! I don't want to see a Myrmidex within 10 feet of this camp! Where's that bloody assistance the King promised!?
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

    public class SakkhraHighChieftess : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(TheGreatHuntQuest) };

        [Constructable]
        public SakkhraHighChieftess()
            : base(BaseEodonTribesman.GetRandomName(), "the Sakkhra High Chieftess")
        {
        }

        public SakkhraHighChieftess(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Body = 0x191;
            HairItemID = 0x203C;
            Hue = 34894;
        }

        public override void InitOutfit()
        {
            SetWearable(new StuddedChest(), 2118);
            SetWearable(new LeatherArms(), 2106);
            SetWearable(new LeatherGloves(), 2106);
            SetWearable(new SkullCap(), 2118);
            SetWearable(new RingmailLegs(), 2106);
            SetWearable(new ThighBoots(), 2106);
            SetWearable(new Yumi(), 2118);
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

    public class UraliHighChieftess : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(EmptyNestQuest) };

        [Constructable]
        public UraliHighChieftess()
            : base(BaseEodonTribesman.GetRandomName(), "the Urali High Chieftess")
        {
        }

        public UraliHighChieftess(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Body = 0x25E;
            Race = Race.Elf;
            HairItemID = 0x2FD0;
            Hue = 35356;
        }

        public override void InitOutfit()
        {
            SetWearable(new ChainLegs(), 2576);
            SetWearable(new DragonChest(), 2576);
            SetWearable(new DragonArms(), 2576);
            SetWearable(new MetalShield(), 2576);
            SetWearable(new Circlet(), 2576);
            SetWearable(new JinBaori(), 2592);
            SetWearable(new Waraji(), 2576);
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

    public class JukariHighChief : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(TheGreatVolcanoQuest) };

        [Constructable]
        public JukariHighChief()
            : base(BaseEodonTribesman.GetRandomName(), "the Jukari High Chief")
        {
        }

        public JukariHighChief(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = 0;
            Hue = 34723;
        }

        public override void InitOutfit()
        {
            SetWearable(new LeatherLegs(), 1175);
            SetWearable(new Shirt(), 1175);
            SetWearable(new Torch());
            SetWearable(new Bokuto(), 1175);
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

    public class KurakHighChief : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(PrideOfTheAmbushQuest) };

        [Constructable]
        public KurakHighChief()
            : base(BaseEodonTribesman.GetRandomName(), "the Kurak High Chief")
        {
        }

        public KurakHighChief(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = 0x203B;
            Hue = 33960;
        }

        public override void InitOutfit()
        {
            SetWearable(new LeatherDo(), 1175);
            SetWearable(new FancyShirt(), 1175);
            SetWearable(new TattsukeHakama());
            SetWearable(new Sandals(), 1175);
            SetWearable(new Tekagi(), 1175);
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

    public class BarakoHighChief : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(TheGreatApeQuest) };

        [Constructable]
        public BarakoHighChief()
            : base(BaseEodonTribesman.GetRandomName(), "the Barako High Chief")
        {
        }

        public BarakoHighChief(Serial serial)
            : base(serial)
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = 0x203C;
            Hue = 35187;
        }

        public override void InitOutfit()
        {
            SetWearable(new BoneChest(), 2407);
            SetWearable(new LeatherNinjaPants(), 2407);
            SetWearable(new StuddedHiroSode(), 2407);
            SetWearable(new BoneGloves(), 2407);
            SetWearable(new StuddedGorget(), 2407);
            SetWearable(new Boots(), 2407);
            SetWearable(new Scepter(), 2407);
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