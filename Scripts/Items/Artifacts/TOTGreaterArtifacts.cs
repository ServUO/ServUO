using System;

namespace Server.Items
{
    public enum PigmentType
    {
        None,
        ParagonGold,
        VioletCouragePurple,
        InvulnerabilityBlue,
        LunaWhite,
        DryadGreen,
        ShadowDancerBlack,
        BerserkerRed,
        NoxGreen,
        RumRed,
        FireOrange,
        FadedCoal,
        Coal,
        FadedGold,
        StormBronze,
        Rose,
        MidnightCoal,
        FadedBronze,
        FadedRose,
        DeepRose
    }

    public class DarkenedSky : Kama
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public DarkenedSky()
            : base()
        {
            this.WeaponAttributes.HitLightning = 60;
            this.Attributes.WeaponSpeed = 25;
            this.Attributes.WeaponDamage = 50;
        }

        public DarkenedSky(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1070966;
            }
        }// Darkened Sky
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = pois = chaos = direct = 0;
            cold = nrgy = 50;
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

    public class KasaOfTheRajin : Kasa
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public KasaOfTheRajin()
            : base()
        {
            this.Attributes.SpellDamage = 12;
        }

        public KasaOfTheRajin(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070969;
            }
        }// Kasa of the Raj-in
        public override int BasePhysicalResistance
        {
            get
            {
                return 12;
            }
        }
        public override int BaseFireResistance
        {
            get
            {
                return 17;
            }
        }
        public override int BaseColdResistance
        {
            get
            {
                return 21;
            }
        }
        public override int BasePoisonResistance
        {
            get
            {
                return 17;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 17;
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
                this.LootType = LootType.Regular;
        }
    }

    public class RuneBeetleCarapace : PlateDo
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RuneBeetleCarapace()
            : base()
        {
            this.Attributes.BonusMana = 10;
            this.Attributes.RegenMana = 3;
            this.Attributes.LowerManaCost = 15;
            this.ArmorAttributes.LowerStatReq = 100;
            this.ArmorAttributes.MageArmor = 1;
        }

        public RuneBeetleCarapace(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1070968;
            }
        }// Rune Beetle Carapace
        public override int BaseColdResistance
        {
            get
            {
                return 14;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 14;
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

    public class Stormgrip : LeatherNinjaMitts
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public Stormgrip()
            : base()
        {
            this.Attributes.BonusInt = 8;
            this.Attributes.Luck = 125;
            this.Attributes.WeaponDamage = 25;
        }

        public Stormgrip(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1070970;
            }
        }// Stormgrip
        public override int BasePhysicalResistance
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
                return 18;
            }
        }
        public override int BaseEnergyResistance
        {
            get
            {
                return 18;
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

    public class SwordOfTheStampede : NoDachi
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SwordOfTheStampede()
            : base()
        {
            this.WeaponAttributes.HitHarm = 100;
            this.Attributes.AttackChance = 10;
            this.Attributes.WeaponDamage = 60;
        }

        public SwordOfTheStampede(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1070964;
            }
        }// Sword of the Stampede
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = fire = pois = nrgy = chaos = direct = 0;
            cold = 100;
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

    public class SwordsOfProsperity : Daisho
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public SwordsOfProsperity()
            : base()
        {
            this.WeaponAttributes.MageWeapon = 30;
            this.Attributes.SpellChanneling = 1;
            this.Attributes.CastSpeed = 1;
            this.Attributes.Luck = 200;
        }

        public SwordsOfProsperity(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1070963;
            }
        }// Swords of Prosperity
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct)
        {
            phys = cold = pois = nrgy = chaos = direct = 0;
            fire = 100;
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

    public class TheHorselord : Yumi
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TheHorselord()
            : base()
        {
            this.Attributes.BonusDex = 5;
            this.Attributes.RegenMana = 1;
            this.Attributes.Luck = 125;
            this.Attributes.WeaponDamage = 50;

            this.Slayer = SlayerName.ElementalBan;
            this.Slayer2 = SlayerName.ReptilianDeath;
        }

        public TheHorselord(Serial serial)
            : base(serial)
        {
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
        public override int LabelNumber
        {
            get
            {
                return 1070967;
            }
        }// The Horselord
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

    public class TomeOfLostKnowledge : Spellbook
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public TomeOfLostKnowledge()
            : base()
        {
            this.LootType = LootType.Regular;
            this.Hue = 0x530;

            this.SkillBonuses.SetValues(0, SkillName.Magery, 15.0);
            this.Attributes.BonusInt = 8;
            this.Attributes.LowerManaCost = 15;
            this.Attributes.SpellDamage = 15;
	    
	    this.LootType = LootType.Blessed;
        }

        public TomeOfLostKnowledge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070971;
            }
        }// Tome of Lost Knowledge
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

    public class WindsEdge : Tessen
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public WindsEdge()
            : base()
        {
            this.WeaponAttributes.HitLeechMana = 40;

            this.Attributes.WeaponDamage = 50;
            this.Attributes.WeaponSpeed = 50;
            this.Attributes.DefendChance = 10;
        }

        public WindsEdge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1070965;
            }
        }// Wind's Edge
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

    public class PigmentsOfTokuno : BasePigmentsOfTokuno
    {
		public override bool IsArtifact { get { return true; } }
        private static readonly int[][] m_Table = new int[][]
        {
            // Hue, Label
            new int[] { /*PigmentType.None,*/ 0, -1 },
            new int[] { /*PigmentType.ParagonGold,*/ 0x501, 1070987 },
            new int[] { /*PigmentType.VioletCouragePurple,*/ 0x486, 1070988 },
            new int[] { /*PigmentType.InvulnerabilityBlue,*/ 0x4F2, 1070989 },
            new int[] { /*PigmentType.LunaWhite,*/ 0x47E, 1070990 },
            new int[] { /*PigmentType.DryadGreen,*/ 0x48F, 1070991 },
            new int[] { /*PigmentType.ShadowDancerBlack,*/ 0x455, 1070992 },
            new int[] { /*PigmentType.BerserkerRed,*/ 0x21, 1070993 },
            new int[] { /*PigmentType.NoxGreen,*/ 0x58C, 1070994 },
            new int[] { /*PigmentType.RumRed,*/ 0x66C, 1070995 },
            new int[] { /*PigmentType.FireOrange,*/ 0x54F, 1070996 },
            new int[] { /*PigmentType.Fadedcoal,*/ 0x96A, 1079579 },
            new int[] { /*PigmentType.Coal,*/ 0x96B, 1079580 },
            new int[] { /*PigmentType.FadedGold,*/ 0x972, 1079581 },
            new int[] { /*PigmentType.StormBronze,*/ 0x977, 1079582 },
            new int[] { /*PigmentType.Rose,*/ 0x97C, 1079583 },
            new int[] { /*PigmentType.MidnightCoal,*/ 0x96C, 1079584 },
            new int[] { /*PigmentType.FadedBronze,*/ 0x975, 1079585 },
            new int[] { /*PigmentType.FadedRose,*/ 0x97B, 1079586 },
            new int[] { /*PigmentType.DeepRose,*/ 0x97E, 1079587 }
        };
        private PigmentType m_Type;
        [Constructable]
        public PigmentsOfTokuno()
            : this(PigmentType.None, 10)
        {
        }

        [Constructable]
        public PigmentsOfTokuno(PigmentType type)
            : this(type, (type == PigmentType.None || type >= PigmentType.FadedCoal) ? 10 : 50)
        {
        }

        [Constructable]
        public PigmentsOfTokuno(PigmentType type, int uses)
            : base(uses)
        {
            this.Weight = 1.0;
            this.Type = type;
        }

        public PigmentsOfTokuno(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public PigmentType Type
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
        public override int LabelNumber
        {
            get
            {
                return 1070933;
            }
        }// Pigments of Tokuno
        public static int[] GetInfo(PigmentType type)
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
                    this.Type = (PigmentType)reader.ReadEncodedInt();
                    break;
                case 0:
                    break;
            }
        }
    }
}
