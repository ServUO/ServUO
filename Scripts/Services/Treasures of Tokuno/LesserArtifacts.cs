using System;

namespace Server.Items
{
    public enum LesserPigmentType
    {
        None,
        PaleOrange,
        FreshRose,
        ChaosBlue,
        Silver,
        NobleGold,
        LightGreen,
        PaleBlue,
        FreshPlum,
        DeepBrown,
        BurntBrown
    }

    public class AncientFarmersKasa : Kasa
    {
        [Constructable]
        public AncientFarmersKasa()
            : base()
        {
            this.Attributes.BonusStr = 5;
            this.Attributes.BonusStam = 5;
            this.Attributes.RegenStam = 5;

            this.SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);
        }

        public AncientFarmersKasa(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070922;
            }
        }// Ancient Farmer's Kasa
        public override int BaseColdResistance
        {
            get
            {
                return 19;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version <= 1)
            {
                this.MaxHitPoints = 255;
                this.HitPoints = 255;
            }

            if (version == 0)
                this.SkillBonuses.SetValues(0, SkillName.AnimalLore, 5.0);
        }
    }

    public class AncientSamuraiDo : PlateDo 
    {
        [Constructable]
        public AncientSamuraiDo()
            : base()
        {
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
            this.SkillBonuses.SetValues(0, SkillName.Parry, 10.0);
        }

        public AncientSamuraiDo(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070926;
            }
        }// Ancient Samurai Do
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 8;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class ArmsOfTacticalExcellence : LeatherHiroSode
    {
        [Constructable]
        public ArmsOfTacticalExcellence()
            : base()
        {
            this.Attributes.BonusDex = 5;
            this.SkillBonuses.SetValues(0, SkillName.Tactics, 12.0);
        }

        public ArmsOfTacticalExcellence(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070921;
            }
        }// Arms of Tactical Excellence
        public override int BaseFireResistance
        {
            get
            {
                return 9;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 13;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 8;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class BlackLotusHood : ClothNinjaHood
    {
        [Constructable]
        public BlackLotusHood()
            : base()
        {
            this.Attributes.LowerManaCost = 6;
            this.Attributes.AttackChance = 6;
            this.ClothingAttributes.SelfRepair = 5;
        }

        public BlackLotusHood(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070919;
            }
        }// Black Lotus Hood
        public override int BasePhysicalResistance
        {
            get
            {
                return 0;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 11;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 11;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0)
            {
                this.MaxHitPoints = 255;
                this.HitPoints = 255;
            }
        }
    }

    public class DaimyosHelm : PlateBattleKabuto
    {
        [Constructable]
        public DaimyosHelm()
            : base()
        {
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
            this.ArmorAttributes.SelfRepair = 3;
            this.Attributes.WeaponSpeed = 10;
        }

        public DaimyosHelm(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070920;
            }
        }// Daimyo's Helm
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DemonForks : Sai
    {
        [Constructable]
        public DemonForks()
            : base()
        {
            this.WeaponAttributes.ResistFireBonus = 10;
            this.WeaponAttributes.ResistPoisonBonus = 10;

            this.Attributes.ReflectPhysical = 10;
            this.Attributes.WeaponDamage = 35;
            this.Attributes.DefendChance = 10;
        }

        public DemonForks(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070917;
            }
        }// Demon Forks
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class DragonNunchaku : Nunchaku
    {
        [Constructable]
        public DragonNunchaku()
            : base()
        {
            this.WeaponAttributes.ResistFireBonus = 5;
            this.WeaponAttributes.SelfRepair = 3;
            this.WeaponAttributes.HitFireball = 50;

            this.Attributes.WeaponDamage = 40;
            this.Attributes.WeaponSpeed = 20;
        }

        public DragonNunchaku(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070914;
            }
        }// Dragon Nunchaku
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class Exiler : Tetsubo
    {
        [Constructable]
        public Exiler()
            : base()
        {
            this.WeaponAttributes.HitDispel = 33;
            this.Slayer = SlayerName.Exorcism;

            this.Attributes.WeaponDamage = 40;
            this.Attributes.WeaponSpeed = 20;
        }

        public Exiler(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070913;
            }
        }// Exiler
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = cold = pois = chaos = direct = 0;

            nrgy = 100;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class GlovesOfTheSun : LeatherNinjaMitts
    {
        [Constructable]
        public GlovesOfTheSun()
            : base()
        {
            this.Attributes.RegenHits = 2;
            this.Attributes.NightSight = 1;
            this.Attributes.LowerManaCost = 5;
            this.Attributes.LowerRegCost = 18;
        }

        public GlovesOfTheSun(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070924;
            }
        }// Gloves of the Sun
        public override int BaseFireResistance
        {
            get
            {
                return 24;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class HanzosBow : Yumi
    {
        [Constructable]
        public HanzosBow()
            : base()
        {
            this.WeaponAttributes.HitLeechHits = 40;
            this.WeaponAttributes.SelfRepair = 3;

            this.Attributes.WeaponDamage = 50;

            this.SkillBonuses.SetValues(0, SkillName.Ninjitsu, 10);
        }

        public HanzosBow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070918;
            }
        }// Hanzo's Bow
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LegsOfStability : PlateSuneate
    {
        [Constructable]
        public LegsOfStability()
            : base()
        {
            this.Attributes.BonusStam = 5;

            this.ArmorAttributes.SelfRepair = 3;
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
        }

        public LegsOfStability(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070925;
            }
        }// Legs of Stability
        public override int BasePhysicalResistance
        {
            get
            {
                return 20;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 18;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PeasantsBokuto : Bokuto
    {
        [Constructable]
        public PeasantsBokuto()
            : base()
        {
            this.WeaponAttributes.SelfRepair = 3;
            this.WeaponAttributes.HitLowerDefend = 30;

            this.Attributes.WeaponDamage = 35;
            this.Attributes.WeaponSpeed = 10;
            this.Slayer = SlayerName.SnakesBane;
        }

        public PeasantsBokuto(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070912;
            }
        }// Peasant's Bokuto
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class PilferedDancerFans : Tessen
    {
        [Constructable]
        public PilferedDancerFans()
            : base()
        {
            this.Attributes.WeaponDamage = 20;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.CastRecovery = 2;
            this.Attributes.DefendChance = 5;
            this.Attributes.SpellChanneling = 1;
        }

        public PilferedDancerFans(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070916;
            }
        }// Pilfered Dancer Fans
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TheDestroyer : NoDachi
    {
        [Constructable]
        public TheDestroyer()
            : base()
        {
            this.WeaponAttributes.HitLeechStam = 40;

            this.Attributes.BonusStr = 6;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponDamage = 50;
        }

        public TheDestroyer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070915;
            }
        }// The Destroyer
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class TomeOfEnlightenment : Spellbook
    {
        [Constructable]
        public TomeOfEnlightenment()
            : base()
        {
            this.LootType = LootType.Regular;
            this.Hue = 0x455;

            this.Attributes.BonusInt = 5;
            this.Attributes.SpellDamage = 10;
            this.Attributes.CastSpeed = 1;
        }

        public TomeOfEnlightenment(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070934;
            }
        }// Tome of Enlightenment
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class LeurociansMempoOfFortune : LeatherMempo
    {
        [Constructable]
        public LeurociansMempoOfFortune()
            : base()
        {
            this.LootType = LootType.Regular;
            this.Hue = 0x501;

            this.Attributes.Luck = 300;
            this.Attributes.RegenMana = 1;
        }

        public LeurociansMempoOfFortune(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1071460;
            }
        }// Leurocian's mempo of fortune
        public override int BasePhysicalResistance
        {
            get
            {
                return 15;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 10;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 15;
            }
        }
        public override int InitMinHits
        {
            get
            {
                return 255;
            }
        }
        public override int InitMaxHits
        {
            get
            {
                return 255;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    //Non weapon/armor ones:
    public class AncientUrn : Item
    {
        private static readonly string[] m_Names = new string[]
        {
            "Akira",
            "Avaniaga",
            "Aya",
            "Chie",
            "Emiko",
            "Fumiyo",
            "Gennai",
            "Gennosuke",
            "Genjo",
            "Hamato",
            "Harumi",
            "Ikuyo",
            "Juri",
            "Kaori",
            "Kaoru",
            "Kiyomori",
            "Mayako",
            "Motoki",
            "Musashi",
            "Nami",
            "Nobukazu",
            "Roku",
            "Romi",
            "Ryo",
            "Sanzo",
            "Sakamae",
            "Satoshi",
            "Takamori",
            "Takuro",
            "Teruyo",
            "Toshiro",
            "Yago",
            "Yeijiro",
            "Yoshi",
            "Zeshin"
        };
        private string m_UrnName;
        [Constructable]
        public AncientUrn(string urnName)
            : base(0x241D)
        {
            this.m_UrnName = urnName;
            this.Weight = 1.0;
        }

        [Constructable]
        public AncientUrn()
            : this(m_Names[Utility.Random(m_Names.Length)])
        {
        }

        public AncientUrn(Serial serial)
            : base(serial)
        {
        }

        public static string[] Names
        {
            get
            {
                return m_Names;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string UrnName
        {
            get
            {
                return this.m_UrnName;
            }
            set
            {
                this.m_UrnName = value;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1071014;
            }
        }// Ancient Urn
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(this.m_UrnName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.m_UrnName = reader.ReadString();

            Utility.Intern(ref this.m_UrnName);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1070935, this.m_UrnName); // Ancient Urn of ~1_name~
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, 1070935, this.m_UrnName); // Ancient Urn of ~1_name~
        }
    }

    public class HonorableSwords : Item
    {
        private string m_SwordsName;
        [Constructable]
        public HonorableSwords(string swordsName)
            : base(0x2853)
        {
            this.m_SwordsName = swordsName;

            this.Weight = 5.0;
        }

        [Constructable]
        public HonorableSwords()
            : this(AncientUrn.Names[Utility.Random(AncientUrn.Names.Length)])
        {
        }

        public HonorableSwords(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public string SwordsName
        {
            get
            {
                return this.m_SwordsName;
            }
            set
            {
                this.m_SwordsName = value;
            }
        }
        public override int LabelNumber
        {
            get
            {
                return 1071015;
            }
        }// Honorable Swords
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(this.m_SwordsName);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            this.m_SwordsName = reader.ReadString();

            Utility.Intern(ref this.m_SwordsName);
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            list.Add(1070936, this.m_SwordsName); // Honorable Swords of ~1_name~
        }

        public override void OnSingleClick(Mobile from)
        {
            this.LabelTo(from, 1070936, this.m_SwordsName); // Honorable Swords of ~1_name~
        }
    }

    [Furniture]
    [Flipable(0x2811, 0x2812)]
    public class ChestOfHeirlooms : LockableContainer
    {
        [Constructable]
        public ChestOfHeirlooms()
            : base(0x2811)
        {
            this.Locked = true;
            this.LockLevel = 95;
            this.MaxLockLevel = 140;
            this.RequiredSkill = 95;
			
            this.TrapType = TrapType.ExplosionTrap;
            this.TrapLevel = 10;
            this.TrapPower = 100;
			
            this.GumpID = 0x10B;
			
            for (int i = 0; i < 10; ++i)
            {
                Item item = Loot.ChestOfHeirloomsContains();
				
                int attributeCount = Utility.RandomMinMax(1, 5);
                int min = 20;
                int max = 80;
				
                if (item is BaseWeapon)
                {
                    BaseWeapon weapon = (BaseWeapon)item;

                    if (Core.AOS)
                        BaseRunicTool.ApplyAttributesTo(weapon, attributeCount, min, max);
                    else
                    {
                        weapon.DamageLevel = (WeaponDamageLevel)Utility.Random(6);
                        weapon.AccuracyLevel = (WeaponAccuracyLevel)Utility.Random(6);
                        weapon.DurabilityLevel = (WeaponDurabilityLevel)Utility.Random(6);
                    }
                }
                else if (item is BaseArmor)
                {
                    BaseArmor armor = (BaseArmor)item;

                    if (Core.AOS)
                        BaseRunicTool.ApplyAttributesTo(armor, attributeCount, min, max);
                    else
                    {
                        armor.ProtectionLevel = (ArmorProtectionLevel)Utility.Random(6);
                        armor.Durability = (ArmorDurabilityLevel)Utility.Random(6);
                    }
                }
                else if (item is BaseHat && Core.AOS)
                    BaseRunicTool.ApplyAttributesTo((BaseHat)item, attributeCount, min, max);
                else if (item is BaseJewel && Core.AOS)
                    BaseRunicTool.ApplyAttributesTo((BaseJewel)item, attributeCount, min, max);
				
                this.DropItem(item);
            }
        }

        public ChestOfHeirlooms(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070937;
            }
        }// Chest of heirlooms
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

    public class FluteOfRenewal : BambooFlute
    {
        [Constructable]
        public FluteOfRenewal()
            : base()
        {
            this.Slayer = SlayerGroup.Groups[Utility.Random(SlayerGroup.Groups.Length - 1)].Super.Name; //-1 to exclude Fey slayer.  Try to confrim no fey slayer on this on OSI

            this.ReplenishesCharges = true;
        }

        public FluteOfRenewal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070927;
            }
        }// Flute of Renewal
        public override int InitMinUses
        {
            get
            {
                return 300;
            }
        }
        public override int InitMaxUses
        {
            get
            {
                return 300;
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version == 0 && this.Slayer == SlayerName.Fey)
                this.Slayer = SlayerGroup.Groups[Utility.Random(SlayerGroup.Groups.Length - 1)].Super.Name;
        }
    }

    public class LesserPigmentsOfTokuno : BasePigmentsOfTokuno
    {
        private static readonly int[][] m_Table = new int[][]
        {
            // Hue, Label
            new int[] { /*PigmentType.None,*/ 0, -1 },
            new int[] { /*PigmentType.PaleOrange,*/ 0x02E, 1071458 },
            new int[] { /*PigmentType.FreshRose,*/ 0x4B9, 1071455 },
            new int[] { /*PigmentType.ChaosBlue,*/ 0x005, 1071459 },
            new int[] { /*PigmentType.Silver,*/ 0x3E9, 1071451 },
            new int[] { /*PigmentType.NobleGold,*/ 0x227, 1071457 },
            new int[] { /*PigmentType.LightGreen,*/ 0x1C8, 1071454 },
            new int[] { /*PigmentType.PaleBlue,*/ 0x24F, 1071456 },
            new int[] { /*PigmentType.FreshPlum,*/ 0x145, 1071450 },
            new int[] { /*PigmentType.DeepBrown,*/ 0x3F0, 1071452 },
            new int[] { /*PigmentType.BurntBrown,*/ 0x41A, 1071453 }
        };
        private LesserPigmentType m_Type;
        [Constructable]
        public LesserPigmentsOfTokuno()
            : this((LesserPigmentType)Utility.Random(0, 11))
        {
        }

        [Constructable]
        public LesserPigmentsOfTokuno(LesserPigmentType type)
            : base(1)
        {
            this.Weight = 1.0;
            this.Type = type;
        }

        public LesserPigmentsOfTokuno(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public LesserPigmentType Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
				
                int v = (int)this.m_Type;

                if (v >= 0 && v < m_Table.Length)
                {
                    this.Hue = m_Table[v][0];
                    this.Label = m_Table[v][1];
                }
                else
                {
                    this.Hue = 0;
                    this.Label = -1;
                }
            }
        }
        public static int[] GetInfo(LesserPigmentType type)
        {
            int v = (int)type;

            if (v < 0 || v >= m_Table.Length)
                v = 0;
			
            return m_Table[v];
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);

            writer.WriteEncodedInt((int)this.m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for BasePigmentsOfTokuno insertion
			
            switch ( version )
            {
                case 1:
                    this.Type = (LesserPigmentType)reader.ReadEncodedInt();
                    break;
                case 0:
                    break;
            }
        }
    }

    public class MetalPigmentsOfTokuno : BasePigmentsOfTokuno
    {
        [Constructable]
        public MetalPigmentsOfTokuno()
            : base(1)
        {
            this.RandomHue();
            this.Label = -1;
        }

        public MetalPigmentsOfTokuno(Serial serial)
            : base(serial)
        {
        }

        public void RandomHue()
        {
            int a = Utility.Random(0, 30);
            if (a != 0)
                this.Hue = a + 0x960;
            else
                this.Hue = 0;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (this.InheritsItem ? 0 : reader.ReadInt()); // Required for BasePigmentsOfTokuno insertion
        }
    }
}