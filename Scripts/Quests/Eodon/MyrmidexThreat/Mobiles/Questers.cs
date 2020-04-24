using Server.Engines.Quests;
using Server.Items;
using System;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class Yar : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(TheZealotryOfZipactriotlQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Yar() : base("Yar", "the Barrab Tinker")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            Hue = 34214;
        }

        public override void InitOutfit()
        {
            SetWearable(new BoneChest(), 1828);
            SetWearable(new DecorativePlateKabuto(), 1828);
            SetWearable(new LeatherHaidate(), 1828);
            SetWearable(new Sandals(), 1828);
            SetWearable(new SledgeHammer(), 1828);
        }

        public override void OnOfferFailed()
        {
            Say(1080107); // I'm sorry, I have nothing for you at this time.
        }

        public Yar(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Carroll : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(HiddenTreasuresQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Carroll() : base("Carroll", "the Gemologist")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt());
            SetWearable(new JinBaori(), 1366);
            SetWearable(new LongPants(), 1336);
            SetWearable(new GoldNecklace());
            SetWearable(new GoldBracelet());
            SetWearable(new GoldRing());
            SetWearable(new Shoes());
        }

        public Carroll(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Bront : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(TheSaltySeaQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Bront() : base("Bront", "the Captain")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(false);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new TricorneHat());
            SetWearable(new Epaulette());
            SetWearable(new BodySash());
            SetWearable(new ElvenShirt());
            SetWearable(new WoodlandBelt());
            SetWearable(new TattsukeHakama());
            SetWearable(new Sandals());
        }

        public Bront(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Eriathwen : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(ATinkersTaskQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Eriathwen() : base("Eriathwen", "the Golem Maker")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Race = Race.Elf;
            Female = true;
            Body = 605;
            HairItemID = Race.RandomHair(true);
            Hue = 0x847E;
        }

        public override void InitOutfit()
        {
            SetWearable(new ElvenShirt(), 164);
            SetWearable(new ElvenPants(), 1114);
            SetWearable(new ElvenBoots(), 1828);
        }

        public Eriathwen(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class CollectorOfOddities : BaseVendor
    {
        private readonly List<SBInfo> m_SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => m_SBInfos;

        public override void InitSBInfo()
        {
            m_SBInfos.Add(new InternalSB());
        }

        [Constructable]
        public CollectorOfOddities() : base("the collector of oddities")
        {
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt(), 1156);
            SetWearable(new Doublet(), 1316);
            SetWearable(new ElvenPants(), 1151);
            SetWearable(new Cloak(), 1151);
            SetWearable(new ElvenBoots(), 2007);
        }

        private class InternalSB : SBInfo
        {
            private readonly List<GenericBuyInfo> m_BuyInfo;
            private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

            public InternalSB() : this(null)
            {
            }

            public InternalSB(BaseVendor owner)
            {
                m_BuyInfo = new InternalBuyInfo(owner);
            }

            public override IShopSellInfo SellInfo => m_SellInfo;
            public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

            public class InternalBuyInfo : List<GenericBuyInfo>
            {
                public InternalBuyInfo(BaseVendor owner)
                {
                    Add(new GenericBuyInfo("Stasis Chamber Power Core", typeof(StasisChamberPowerCore), 101250, 500, 40155, 0));

                    Add(new GenericBuyInfo("1159014", typeof(CircuitTrapTrainingKit), 99375, 500, 41875, 0));
                    Add(new GenericBuyInfo("1159015", typeof(CylinderTrapTrainingKit), 99375, 500, 41875, 0));
                    Add(new GenericBuyInfo("1159016", typeof(SliderTrapTrainingKit), 99375, 500, 41875, 0));
                }
            }

            public class InternalSellInfo : GenericSellInfo
            {
            }
        }

        public CollectorOfOddities(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class EllieRafkin : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(ExterminatingTheInfestationQuest) };

        public override bool ChangeRace => false;
        public override bool IsActiveVendor => true;

        private readonly List<SBInfo> _SBInfos = new List<SBInfo>();
        protected override List<SBInfo> SBInfos => _SBInfos;

        public override void InitSBInfo()
        {
            _SBInfos.Add(new InternalSB());
        }

        [Constructable]
        public EllieRafkin() : base("Ellie Rafkin", "the Professor")
        {
        }

        public override void OnOfferFailed()
        {
            Say(1080107); // I'm sorry, I have nothing for you at this time.
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Body = 0x191;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new FancyShirt());
            SetWearable(new Kilt(), 933);
            SetWearable(new ThighBoots(), 1);
        }

        private class InternalSB : SBInfo
        {
            private readonly List<GenericBuyInfo> m_BuyInfo;
            private readonly IShopSellInfo m_SellInfo = new InternalSellInfo();

            public InternalSB()
                : this(null)
            {
            }

            public InternalSB(BaseVendor owner)
            {
                m_BuyInfo = new InternalBuyInfo(owner);
            }

            public override IShopSellInfo SellInfo => m_SellInfo;
            public override List<GenericBuyInfo> BuyInfo => m_BuyInfo;

            public class InternalBuyInfo : List<GenericBuyInfo>
            {
                public InternalBuyInfo(BaseVendor owner)
                {
                    Add(new GenericBuyInfo("Unabridged Map of Eodon", typeof(UnabridgedAtlasOfEodon), 62500, 500, 7185, 0));
                }
            }

            public class InternalSellInfo : GenericSellInfo
            {
            }
        }

        public EllieRafkin(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Foxx : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(PestControlQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Foxx() : base("Foxx", "the Lieutenant")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new PlateChest());
            SetWearable(new PlateLegs());
            SetWearable(new BodySash(), 1828);
            SetWearable(new OrderShield());
            SetWearable(new Longsword());
        }

        public override void Advertise()
        {
            Say(1156619); // Fall in now! These Myrmidex aren't going to slay themselves! We've got to squash these bugs!
        }

        public Foxx(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Yero : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(GettingEvenQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Yero() : base("Yero", "the Gambler")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = false;
            Body = 0x190;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new ShortPants());
            SetWearable(new Sandals());
        }

        public Yero(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }

    public class Alida : MondainQuester
    {
        public override Type[] Quests => new Type[] { typeof(OfVorpalsAndLettacesTheGardnerQuest) };

        public override bool ChangeRace => false;

        [Constructable]
        public Alida() : base("Alida", "the Gardener")
        {
        }

        public override void InitBody()
        {
            InitStats(100, 100, 25);

            Female = true;
            Body = 0x191;
            HairItemID = Race.RandomHair(true);
            Hue = Race.RandomSkinHue();
        }

        public override void InitOutfit()
        {
            SetWearable(new Shirt());
            SetWearable(new LongPants(), 1);
            SetWearable(new HalfApron(), 263);
            SetWearable(new LeatherGloves());
            SetWearable(new FloppyHat());
        }

        public Alida(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
        }
    }
}
