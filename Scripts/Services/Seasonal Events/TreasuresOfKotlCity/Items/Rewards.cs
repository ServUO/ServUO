using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class SkeletalHangmanAddon : BaseAddon
    {
        private Mobile _Clicker;

        public override BaseAddonDeed Deed => new SkeletalHangmanAddonDeed();

        [Constructable]
        public SkeletalHangmanAddon()
            : this(null)
        {
        }

        public SkeletalHangmanAddon(Mobile clicker)
        {
            _Clicker = clicker;

            AddComponent(new AddonComponent(0x9D39), 0, 0, 0);
            AddComponent(new InternalComponent(0x9D38), 0, 1, 0);
        }

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch (direction)
            {
                case Direction.South:
                    AddComponent(new AddonComponent(0x9D39), 0, 0, 0);
                    AddComponent(new InternalComponent(0x9D38), 0, 1, 0);
                    break;
                case Direction.East:
                    AddComponent(new AddonComponent(0x9D3B), 0, 0, 0);
                    AddComponent(new InternalComponent(0x9D3A), 1, 0, 0);
                    break;
            }
        }

        public class InternalComponent : AddonComponent
        {
            public override bool ForceShowProperties => true;

            public InternalComponent(int id) : base(id)
            {
            }

            public override void AddNameProperty(ObjectPropertyList list)
            {
                SkeletalHangmanAddon addon = Addon as SkeletalHangmanAddon;

                if (addon._Clicker != null)
                {
                    list.Add(1156983, addon._Clicker.Name);
                }
                else
                {
                    list.Add(1156982);
                }
            }

            public InternalComponent(Serial serial)
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
                int version = reader.ReadInt();
            }
        }

        public SkeletalHangmanAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write(_Clicker);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Clicker = reader.ReadMobile();
        }
    }

    public class SkeletalHangmanAddonDeed : BaseAddonDeed
    {
        public override int LabelNumber => 1156982;  // Skeletal Hangman
        public override BaseAddon Addon => new SkeletalHangmanAddon(_Clicker);

        private Mobile _Clicker;

        [Constructable]
        public SkeletalHangmanAddonDeed()
        {
        }

        public override void OnDoubleClick(Mobile m)
        {
            _Clicker = m;

            base.OnDoubleClick(m);
        }

        public SkeletalHangmanAddonDeed(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    [FlipableAddon(Direction.South, Direction.East)]
    public class KotlSacraficialAltarAddon : BaseAddon
    {
        public override BaseAddonDeed Deed => new KotlSacraficialAltarAddonDeed();

        [Constructable]
        public KotlSacraficialAltarAddon()
        {
            AddComponent(new AddonComponent(0x9D5F), 0, 0, 0);
            AddComponent(new AddonComponent(0x9D60), 0, 1, 0);
            AddComponent(new AddonComponent(0x9D61), 1, 0, 0);
            AddComponent(new AddonComponent(0x9D62), 1, 1, 0);
        }

        public virtual void Flip(Mobile from, Direction direction)
        {
            switch (direction)
            {
                case Direction.East:
                    AddComponent(new AddonComponent(0x9D5F), 0, 0, 0);
                    AddComponent(new AddonComponent(0x9D60), 0, 1, 0);
                    AddComponent(new AddonComponent(0x9D61), 1, 0, 0);
                    AddComponent(new AddonComponent(0x9D62), 1, 1, 0);
                    break;
                case Direction.South:
                    AddComponent(new AddonComponent(0x9D63), 0, 0, 0);
                    AddComponent(new AddonComponent(0x9D64), 1, 0, 0);
                    AddComponent(new AddonComponent(0x9D65), 0, 1, 0);
                    AddComponent(new AddonComponent(0x9D66), 1, 1, 0);
                    break;
            }
        }

        public KotlSacraficialAltarAddon(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    public class KotlSacraficialAltarAddonDeed : BaseAddonDeed
    {
        public override BaseAddon Addon => new KotlSacraficialAltarAddon();
        public override int LabelNumber => 1124311;  // Kotl Sacrificial Altar

        [Constructable]
        public KotlSacraficialAltarAddonDeed()
        {
        }

        public KotlSacraficialAltarAddonDeed(Serial serial)
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
            int version = reader.ReadInt();
        }
    }

    public class TreasuresOfKotlRewardDeed : BaseRewardTitleDeed
    {
        private TextDefinition _Title;

        public override TextDefinition Title => _Title;

        public TreasuresOfKotlRewardDeed(int localization)
        {
            _Title = localization;
        }

        public TreasuresOfKotlRewardDeed(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            TextDefinition.Serialize(writer, _Title);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Title = TextDefinition.Deserialize(reader);
        }
    }

    public class KatalkotlsRing : SilverRing
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1156989;

        [Constructable]
        public KatalkotlsRing()
        {
            Hue = 2591;

            SkillBonuses.SetValues(0, SkillName.MagicResist, 10);
            SkillBonuses.SetValues(1, SkillName.Meditation, 10);
            Attributes.RegenHits = 5;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 20;
        }

        public bool HasSkillBonus => SkillBonuses.Skill_3_Value != 0;

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack) && m is PlayerMobile && !HasSkillBonus)
            {
                BaseGump.SendGump(new ApplySkillBonusGump((PlayerMobile)m, SkillBonuses, Skills, 20, 2));
            }
            else
            {
                base.OnDoubleClick(m);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!HasSkillBonus)
            {
                list.Add(1155609); // Double Click to Set Skill Bonus
            }
        }

        public static SkillName[] Skills =
        {
            SkillName.Necromancy,
            SkillName.Magery,
            SkillName.Bushido,
            SkillName.Chivalry,
            SkillName.Ninjitsu,
            SkillName.Mysticism
        };

        public KatalkotlsRing(Serial serial)
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

    public class BootsOfEscaping : ThighBoots
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1155607;  // Boots of Escaping

        [Constructable]
        public BootsOfEscaping()
        {
            Attributes.BonusDex = 4;
            Attributes.RegenStam = 1;
        }

        public BootsOfEscaping(Serial serial)
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

    public class TalonsOfEscaping : LeatherTalons
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1155682;  // Talons of Escaping

        public TalonsOfEscaping()
        {
            Attributes.BonusDex = 4;
            Attributes.RegenStam = 1;
        }

        public TalonsOfEscaping(Serial serial)
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

    public class TribalBanner : Item, IFlipable
    {
        public override int LabelNumber => ItemID + 1084024;
        public override bool ForceShowProperties => true;

        public int NorthID => GetTribeID(_Tribe);
        public int WestID => GetTribeID(_Tribe) + 1;

        private EodonTribe _Tribe;

        [CommandProperty(AccessLevel.GameMaster)]
        public EodonTribe Tribe
        {
            get { return _Tribe; }
            set
            {
                _Tribe = value;

                int id = GetTribeID(_Tribe);

                if (ItemID != id)
                    ItemID = id;

                InvalidateProperties();
            }
        }

        [Constructable]
        public TribalBanner(EodonTribe tribe) : base(GetTribeID(tribe))
        {
            _Tribe = tribe;
        }

        public static int GetTribeID(EodonTribe tribe)
        {
            switch (tribe)
            {
                default:
                case EodonTribe.Jukari: return 0x9D53;
                case EodonTribe.Kurak: return 0x9D55;
                case EodonTribe.Barako: return 0x9D57;
                case EodonTribe.Sakkhra: return 0x9D59;
                case EodonTribe.Barrab: return 0x9D5B;
                case EodonTribe.Urali: return 0x9D5D;
            }
        }

        public void OnFlip(Mobile from)
        {
            // lets make sure we have the right ID before we begin
            int id = ItemID;

            if (id != WestID && id != NorthID)
            {
                id = WestID;
            }

            if (id == WestID)
                ItemID = NorthID;
            else
                ItemID = WestID;
        }

        public TribalBanner(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);

            writer.Write((int)_Tribe);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            _Tribe = (EodonTribe)reader.ReadInt();
        }
    }
}
