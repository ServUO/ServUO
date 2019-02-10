using System;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
    [FlipableAddon(Direction.South, Direction.East)]
    public class SkeletalHangmanAddon : BaseAddon
    {
        private Mobile _Clicker;

        public override BaseAddonDeed Deed { get { return new SkeletalHangmanAddonDeed(); } }

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
            public override bool ForceShowProperties { get { return true; } }

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
        public override int LabelNumber { get { return 1156982; } } // Skeletal Hangman
        public override BaseAddon Addon { get { return new SkeletalHangmanAddon(_Clicker); } }

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
        public override BaseAddonDeed Deed { get { return new KotlSacraficialAltarAddonDeed(); } }

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
        public override BaseAddon Addon { get { return new KotlSacraficialAltarAddon(); } }
        public override int LabelNumber { get { return 1124311; } } // Kotl Sacrificial Altar

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

        public override TextDefinition Title { get { return _Title; } }

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
        public override int LabelNumber { get { return 1156989; } }

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

        public override void OnDoubleClick(Mobile m)
        {
            if (IsChildOf(m.Backpack) && SkillBonuses.GetBonus(2) == 0)
            {
                m.SendGump(new InternalGump(m, this));
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (SkillBonuses.GetBonus(2) == 0)
            {
                list.Add(1155609); // Double Click to Set Skill Bonus
            }
        }

        public class InternalGump : Gump
        {
            private Mobile m_Mobile;
            private KatalkotlsRing m_Ring;

            public InternalGump(Mobile mobile, KatalkotlsRing ring)
                : base(20, 20)
            {
                mobile.CloseGump(typeof(KatalkotlsRing.InternalGump));

                m_Mobile = mobile;
                m_Ring = ring;

                int font = 0x7FFF;

                AddBackground(0, 0, 170, 210, 9270);
                AddAlphaRegion(10, 10, 150, 190);

                AddHtmlLocalized(20, 22, 150, 16, 1155610, font, false, false); //Please Choose A Skill

                AddButton(20, 50, 9702, 9703, 1, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 50, 200, 16, 1044109, font, false, false); //Necromancy

                AddButton(20, 75, 9702, 9703, 2, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 75, 200, 16, 1044085, font, false, false); //Magery

                AddButton(20, 100, 9702, 9703, 3, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 100, 200, 16, 1044112, font, false, false); //Bushido

                AddButton(20, 125, 9702, 9703, 4, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 125, 200, 16, 1044111, font, false, false); //Chivalry

                AddButton(20, 150, 9702, 9703, 5, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 150, 200, 16, 1044113, font, false, false); //Ninjitsu

                AddButton(20, 175, 9702, 9703, 6, GumpButtonType.Reply, 0);
                AddHtmlLocalized(45, 175, 200, 16, 1044115, font, false, false); //Mysticism
            }

            public override void OnResponse(NetState sender, RelayInfo info)
            {
                if (info.ButtonID == 0)
                    return;

                SkillName skill;

                switch (info.ButtonID)
                {
                    default:
                    case 1: skill = SkillName.Necromancy; break;
                    case 2: skill = SkillName.Magery; break;
                    case 3: skill = SkillName.Bushido; break;
                    case 4: skill = SkillName.Chivalry; break;
                    case 5: skill = SkillName.Ninjitsu; break;
                    case 6: skill = SkillName.Mysticism; break;
                }

                m_Mobile.SendGump(new ConfirmCallbackGump((PlayerMobile)m_Mobile, m_Mobile.Skills[skill].Info.Name, 1155611, null, confirm: (pm, state) =>
                    {
                        if (m_Ring != null && m_Ring.IsChildOf(pm.Backpack) && !m_Ring.Deleted && m_Ring.SkillBonuses.GetBonus(2) == 0)
                        {
                            m_Ring.SkillBonuses.SetValues(2, skill, 20.0);

                            pm.SendLocalizedMessage(1155612); // A skill bonus has been applied to the item!
                        }
                    }));
            }
        }

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
            int version = reader.ReadInt();
        }
    }

    public class BootsOfEscaping : ThighBoots
    {
        public override int LabelNumber { get { return 1155607; } } // Boots of Escaping

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
            int version = reader.ReadInt();
        }
    }

    public class TalonsOfEscaping : LeatherTalons
    {
        public override int LabelNumber { get { return 1155682; } } // Talons of Escaping

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
            int version = reader.ReadInt();
        }
    }

    public class TribalBanner : Item, IFlipable
    {
        public override int LabelNumber { get { return ItemID + 1084024; } }
        public override bool ForceShowProperties { get { return true; } }

        public int NorthID { get { return GetTribeID(_Tribe); } }
        public int WestID { get { return GetTribeID(_Tribe) + 1; } }

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

        public void OnFlip()
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
