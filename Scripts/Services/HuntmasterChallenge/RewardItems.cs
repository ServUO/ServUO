using Server.Mobiles;
using System;

namespace Server.Items
{
    public class HarvestersBlade : ElvenSpellblade
    {
        public override int LabelNumber => 1114096;  // Harvester's Blade

        [Constructable]
        public HarvestersBlade()
        {
            Hue = 1191;
            Attributes.SpellChanneling = 1;
        }

        public HarvestersBlade(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class RangersGuildSash : BodySash
    {
        public override int LabelNumber => 1155744;  // Member of the Skara Brae Ranger's Guild

        [Constructable]
        public RangersGuildSash()
        {
            LootType = LootType.Blessed;
        }

        public RangersGuildSash(Serial serial) : base(serial)
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
            reader.ReadInt();
        }
    }

    public class GargishRangersGuildSash : GargishSash
    {
        public override int LabelNumber => 1155744;  // Member of the Skara Brae Ranger's Guild

        [Constructable]
        public GargishRangersGuildSash()
        {
            LootType = LootType.Blessed;
        }

        public GargishRangersGuildSash(Serial serial)
            : base(serial)
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
            reader.ReadInt();
        }
    }

    public class HuntmastersRewardTitleDeed : BaseRewardTitleDeed
    {
        public override TextDefinition Title => new TextDefinition(1155727);  // Huntmaster's Champion

        [Constructable]
        public HuntmastersRewardTitleDeed()
        {
        }

        public HuntmastersRewardTitleDeed(Serial serial)
            : base(serial)
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
            reader.ReadInt();
        }
    }

    [Flipable(0x4BD9, 0x4BDA)]
    public class HornOfPlenty : Item, IUsesRemaining
    {
        public override int LabelNumber => 1153503;  // Horn of Plenty

        private int m_UsesRemaining;

        [CommandProperty(AccessLevel.GameMaster)]
        public int UsesRemaining
        {
            get { return m_UsesRemaining; }
            set { m_UsesRemaining = value; InvalidateProperties(); }
        }

        public bool ShowUsesRemaining { get { return true; } set { } }

        private DateTime _NextRecharge;

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime NextRecharge
        {
            get { return _NextRecharge; }
            set { _NextRecharge = value; }
        }

        [Constructable]
        public HornOfPlenty()
            : base(0x4BD9)
        {
            UsesRemaining = 10;
        }

        //TODO: add pub 84, 88 and 95 shit
        public override void OnDoubleClick(Mobile from)
        {
            if (m_UsesRemaining > 0)
            {
                Item item = null;

                switch (Utility.Random(10))
                {
                    case 0: item = new SweetPotatoPie(); break;
                    case 1: item = new MashedSweetPotatoes(); break;
                    case 2: item = new BasketOfRolls(); break;
                    case 3: item = new TurkeyPlatter(); break;
                    case 4:
                        BaseCreature bc = new Turkey(true);
                        if (0.10 > Utility.RandomDouble())
                            bc.Name = "Mister Gobbles";
                        bc.MoveToWorld(from.Location, from.Map);
                        from.SendLocalizedMessage(1153512); //That one's not cooked!
                        break;
                    case 5:
                        new InternalTimer(from);
                        from.Frozen = true;
                        break;
                    case 6: item = new PottedCoffeePlant(); break;
                    case 7: item = new RoastingPigOnASpitDeed(); break;
                    case 8: item = new FormalDiningTableDeed(); break;
                    case 9: item = new BuffetTableDeed(); break;
                }

                if (item != null)
                {
                    if (from.Backpack == null || !from.Backpack.TryDropItem(from, item, false))
                        item.MoveToWorld(from.Location, from.Map);

                    UsesRemaining--;
                }
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Mobile m_From;
            private int m_Ticks;

            public InternalTimer(Mobile from)
                : base(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
            {
                m_From = from;
                Start();
            }

            protected override void OnTick()
            {
                m_Ticks++;

                if (m_Ticks % 3 == 0)
                    m_From.Say(1153513); // * ZzzzZzzzZzzzZ *

                if (m_Ticks == 10)
                {
                    Stop();
                    m_From.Frozen = false;
                }
            }
        }

        public override void AddUsesRemainingProperties(ObjectPropertyList list)
        {
            if (ShowUsesRemaining)
                list.Add(1049116, m_UsesRemaining.ToString()); // [ Charges: ~1_CHARGES~ ]
        }

        private void CheckRecharge()
        {
            if (DateTime.UtcNow.Month == 11 && UsesRemaining < 10 && _NextRecharge < DateTime.UtcNow)
            {
                UsesRemaining++;
                _NextRecharge = DateTime.UtcNow + TimeSpan.FromDays(1);
            }
        }

        public HornOfPlenty(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);

            writer.Write(_NextRecharge);
            writer.Write(m_UsesRemaining);

            CheckRecharge();
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            switch (v)
            {
                case 1:
                    _NextRecharge = reader.ReadDateTime();
                    goto case 0;
                case 0:
                    m_UsesRemaining = reader.ReadInt();
                    break;
            }
        }
    }

    public class HarvestersAxe : TwoHandedAxe
    {
        public override int LabelNumber => 1158774;  // Harvester's Axe

        private int _Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return _Charges; } set { _Charges = value; InvalidateProperties(); } }

        [Constructable]
        public HarvestersAxe()
        {
            Charges = 1000;
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);
            list.Add(1158775);  // * Magically Chops Logs into Boards *
            list.Add(1060741, _Charges.ToString()); // charges: 
        }

        public HarvestersAxe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Charges = reader.ReadInt();
        }
    }

    public class GargishHarvestersAxe : GargishAxe
    {
        public override int LabelNumber => 1158774;  // Harvester's Axe

        private int _Charges;

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges { get { return _Charges; } set { _Charges = value; InvalidateProperties(); } }

        [Constructable]
        public GargishHarvestersAxe()
        {
            Charges = 1000;
        }

        public override void AddWeightProperty(ObjectPropertyList list)
        {
            base.AddWeightProperty(list);
            list.Add(1158775);  // * Magically Chops Logs into Boards *
            list.Add(1060741, _Charges.ToString()); // charges: 
        }

        public GargishHarvestersAxe(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Charges);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();

            _Charges = reader.ReadInt();
        }
    }

    public class BakeKitsuneHat : BaseHat
    {
        public override int LabelNumber => 1126051;  // bake-kitsune hat

        public override int BasePhysicalResistance => 1;
        public override int BaseFireResistance => 3;
        public override int BaseColdResistance => 8;
        public override int BasePoisonResistance => 3;
        public override int BaseEnergyResistance => 9;

        public override int InitMinHits => 40;
        public override int InitMaxHits => 60;

        [Constructable]
        public BakeKitsuneHat()
            : this(0)
        {
        }

        [Constructable]
        public BakeKitsuneHat(int hue)
            : base(0xA42B, hue)
        {
            Quality = ItemQuality.Exceptional;
        }

        public BakeKitsuneHat(Serial serial)
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
            reader.ReadInt();
        }
    }
}
