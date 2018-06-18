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
            WeaponAttributes.HitLightning = 60;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 50;
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
            Attributes.SpellDamage = 12;
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
                MaxHitPoints = 255;
                HitPoints = 255;
            }

            if (version == 0)
                LootType = LootType.Regular;
        }
    }

    public class RuneBeetleCarapace : PlateDo
    {
		public override bool IsArtifact { get { return true; } }
        [Constructable]
        public RuneBeetleCarapace()
            : base()
        {
            Attributes.BonusMana = 10;
            Attributes.RegenMana = 3;
            Attributes.LowerManaCost = 15;
            ArmorAttributes.LowerStatReq = 100;
            ArmorAttributes.MageArmor = 1;
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
            Attributes.BonusInt = 8;
            Attributes.Luck = 125;
            Attributes.WeaponDamage = 25;
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
            WeaponAttributes.HitHarm = 100;
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 60;
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
            WeaponAttributes.MageWeapon = 30;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 1;
            Attributes.Luck = 200;
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
            Attributes.BonusDex = 5;
            Attributes.RegenMana = 1;
            Attributes.Luck = 125;
            Attributes.WeaponDamage = 50;

            Slayer = SlayerName.ElementalBan;
            Slayer2 = SlayerName.ReptilianDeath;
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
            LootType = LootType.Regular;
            Hue = 0x530;

            SkillBonuses.SetValues(0, SkillName.Magery, 15.0);
            Attributes.BonusInt = 8;
            Attributes.LowerManaCost = 15;
            Attributes.SpellDamage = 15;
	    
	    LootType = LootType.Blessed;
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
            WeaponAttributes.HitLeechMana = 40;

            Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 50;
            Attributes.DefendChance = 10;
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

        public static int[][] Table { get { return m_Table; } }
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
            Weight = 1.0;
            Type = type;
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
                return m_Type;
            }
            set
            {
                m_Type = value;
				
                int v = (int)m_Type;

                if (v >= 0 && v < m_Table.Length)
                {
                    Hue = m_Table[v][0];
                    Label = m_Table[v][1];
                }
                else
                {
                    Hue = 0;
                    Label = -1;
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

            writer.WriteEncodedInt((int)m_Type);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = (InheritsItem ? 0 : reader.ReadInt()); // Required for BasePigmentsOfTokuno insertion
			
            switch ( version )
            {
                case 1:
                    Type = (PigmentType)reader.ReadEncodedInt();
                    break;
                case 0:
                    break;
            }
        }
    }
}
