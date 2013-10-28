using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlParagon : XmlAttachment
    {
        #region private properties
        private double m_ChestChance;         // Chance that a paragon will carry a paragon chest

        // default artifact types
        private static Type[] m_Artifacts = new Type[]
        {
            typeof(GoldBricks), typeof(PhillipsWoodenSteed),
            typeof(AlchemistsBauble), typeof(ArcticDeathDealer),
            typeof(BlazeOfDeath), typeof(BowOfTheJukaKing),
            typeof(BurglarsBandana), typeof(CavortingClub),
            typeof(EnchantedTitanLegBone), typeof(GwennosHarp),
            typeof(IolosLute), typeof(LunaLance),
            typeof(NightsKiss), typeof(NoxRangersHeavyCrossbow),
            typeof(OrcishVisage), typeof(PolarBearMask),
            typeof(ShieldOfInvulnerability), typeof(StaffOfPower),
            typeof(VioletCourage), typeof(HeartOfTheLion),
            typeof(WrathOfTheDryad), typeof(PixieSwatter),
            typeof(GlovesOfThePugilist)
        };

        private int m_Hue;

        private double m_HitsBuff;
        private double m_StrBuff;
        private double m_IntBuff;
        private double m_DexBuff;
        private double m_SkillsBuff;
        private double m_SpeedBuff;
        private double m_FameBuff;
        private double m_KarmaBuff;
        private int m_DamageBuff;

        private bool m_EnableParagon = true;
        private string m_ParagonLabel;
        private double m_ConvertFactor = 1;
        private double m_ArtifactFactor = 1;
        #endregion

        #region public properties

        public virtual Type[] Artifacts
        {
            get
            {
                return m_Artifacts;
            }
            set
            {
                m_Artifacts = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double ConvertFactor
        {
            get
            {
                return this.m_ConvertFactor;
            }
            set
            {
                this.m_ConvertFactor = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double ArtifactFactor
        {
            get
            {
                return this.m_ArtifactFactor;
            }
            set
            {
                this.m_ArtifactFactor = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Hue
        {
            get
            {
                return this.m_Hue;
            }
            set
            {
                this.m_Hue = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HitsBuff
        {
            get
            {
                return this.m_HitsBuff;
            }
            set
            {
                this.m_HitsBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double StrBuff
        {
            get
            {
                return this.m_StrBuff;
            }
            set
            {
                this.m_StrBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double IntBuff
        {
            get
            {
                return this.m_IntBuff;
            }
            set
            {
                this.m_IntBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double DexBuff
        {
            get
            {
                return this.m_DexBuff;
            }
            set
            {
                this.m_DexBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SkillsBuff
        {
            get
            {
                return this.m_SkillsBuff;
            }
            set
            {
                this.m_SkillsBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SpeedBuff
        {
            get
            {
                return this.m_SpeedBuff;
            }
            set
            {
                this.m_SpeedBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double FameBuff
        {
            get
            {
                return this.m_FameBuff;
            }
            set
            {
                this.m_FameBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double KarmaBuff
        {
            get
            {
                return this.m_KarmaBuff;
            }
            set
            {
                this.m_KarmaBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageBuff
        {
            get
            {
                return this.m_DamageBuff;
            }
            set
            {
                this.m_DamageBuff = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double ChestChance
        {
            get
            {
                return this.m_ChestChance;
            }
            set
            {
                this.m_ChestChance = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool EnableParagon
        {
            get
            {
                return this.m_EnableParagon;
            }
            set
            {
                this.m_EnableParagon = value;
                this.InvalidateParentProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string ParagonLabel
        {
            get
            {
                return this.m_ParagonLabel;
            }
            set
            {
                this.m_ParagonLabel = value;
            }
        }
        #endregion

        #region static interface methods

        public static XmlParagon GetXmlParagon(BaseCreature bc)
        {
            // check for an XmlParagon attachment on the spawner of the creature
            if (bc != null && bc.Spawner != null)
            {
                return (XmlParagon)XmlAttach.FindAttachment(bc.Spawner, typeof(XmlParagon));
            }

            return null;
        }

        public static bool IsCustomParagon(BaseCreature bc)
        {
            return (GetXmlParagon(bc) != null);
        }

        public static string GetParagonLabel(BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                return xa.XmlGetParagonLabel(bc);
            }
            else
            {
                return "(Paragon)";
            }
        }

        // static method hooks that interface with the distro Paragon system

        public static bool CheckConvert(BaseCreature bc, Point3D location, Map m)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                return xa.XmlCheckConvert(bc, location, m);
            }
            else
            {
                return Paragon.CheckConvert(bc, location, m);
            }
        }

        public static void AddChest(BaseCreature bc, int treasureLevel)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                xa.XmlAddChest(bc, treasureLevel);
            }
            else
            {
                bc.PackItem(new ParagonChest(bc.Name, treasureLevel));
            }
        }

        public static double GetChestChance(BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                return xa.XmlChestChance(bc);
            }
            else
            {
                return Paragon.ChestChance;
            }
        }

        public static double GetHitsBuff(BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                return xa.HitsBuff;
            }
            else
            {
                return Paragon.HitsBuff;
            }
        }

        public static void Convert(BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                xa.XmlConvert(bc);
            }
            else
            {
                Paragon.Convert(bc);
            }
        }

        public static void UnConvert(BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                xa.XmlUnConvert(bc);
            }
            else
            {
                Paragon.UnConvert(bc);
            }
        }

        public static bool CheckArtifactChance(Mobile m, BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                return xa.XmlCheckArtifactChance(m, bc);
            }
            else
            {
                return Paragon.CheckArtifactChance(m, bc);
            }
        }

        public static void GiveArtifactTo(Mobile m, BaseCreature bc)
        {
            XmlParagon xa = GetXmlParagon(bc);

            if (xa != null)
            {
                xa.XmlGiveArtifactTo(m, bc);
            }
            else
            {
                Paragon.GiveArtifactTo(m);
            }
        }

        #endregion

        // modify these custom conversion methods to create unique paragons for spawners with this attachment
        #region custom conversion methods

        public virtual void XmlAddChest(BaseCreature bc, int treasureLevel)
        {
            if (bc == null)
                return;

            bc.PackItem(new ParagonChest(bc.Name, treasureLevel));
        }

        public virtual double XmlChestChance(BaseCreature bc)
        {
            return this.ChestChance;
        }

        public virtual string XmlGetParagonLabel(BaseCreature bc)
        {
            return this.ParagonLabel;
        }

        public virtual void XmlConvert(BaseCreature bc)
        {
            if (bc == null || bc.IsParagon)
                return;

            bc.Hue = this.Hue;

            if (bc.HitsMaxSeed >= 0)
                bc.HitsMaxSeed = (int)(bc.HitsMaxSeed * this.HitsBuff);

            bc.RawStr = (int)(bc.RawStr * this.StrBuff);
            bc.RawInt = (int)(bc.RawInt * this.IntBuff);
            bc.RawDex = (int)(bc.RawDex * this.DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            for (int i = 0; i < bc.Skills.Length; i++)
            {
                Skill skill = (Skill)bc.Skills[i];

                if (skill.Base > 0.0)
                    skill.Base *= this.SkillsBuff;
            }

            bc.PassiveSpeed /= this.SpeedBuff;
            bc.ActiveSpeed /= this.SpeedBuff;

            bc.DamageMin += this.DamageBuff;
            bc.DamageMax += this.DamageBuff;

            if (bc.Fame > 0)
                bc.Fame = (int)(bc.Fame * this.FameBuff);

            if (bc.Fame > 32000)
                bc.Fame = 32000;

            // TODO: Mana regeneration rate = Sqrt( buffedFame ) / 4

            if (bc.Karma != 0)
            {
                bc.Karma = (int)(bc.Karma * this.KarmaBuff);

                if (Math.Abs(bc.Karma) > 32000)
                    bc.Karma = 32000 * Math.Sign(bc.Karma);
            }
        }

        public virtual void XmlUnConvert(BaseCreature bc)
        {
            if (bc == null || !bc.IsParagon)
                return;

            bc.Hue = 0;

            if (bc.HitsMaxSeed >= 0)
                bc.HitsMaxSeed = (int)(bc.HitsMaxSeed / this.HitsBuff);

            bc.RawStr = (int)(bc.RawStr / this.StrBuff);
            bc.RawInt = (int)(bc.RawInt / this.IntBuff);
            bc.RawDex = (int)(bc.RawDex / this.DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            for (int i = 0; i < bc.Skills.Length; i++)
            {
                Skill skill = (Skill)bc.Skills[i];

                if (skill.Base > 0.0)
                    skill.Base /= this.SkillsBuff;
            }

            bc.PassiveSpeed *= this.SpeedBuff;
            bc.ActiveSpeed *= this.SpeedBuff;

            bc.DamageMin -= this.DamageBuff;
            bc.DamageMax -= this.DamageBuff;

            if (bc.Fame > 0)
                bc.Fame = (int)(bc.Fame / this.FameBuff);
            if (bc.Karma != 0)
                bc.Karma = (int)(bc.Karma / this.KarmaBuff);
        }

        public virtual bool XmlCheckConvert(BaseCreature bc, Point3D location, Map m)
        {
            if (!this.EnableParagon || bc == null)
                return false;

            if (bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc is Clone)
                return false;

            int fame = bc.Fame;

            if (fame > 32000)
                fame = 32000;

            double chance = this.ConvertFactor / Math.Round(20.0 - (fame / 3200));

            return (chance > Utility.RandomDouble());
        }

        public virtual bool XmlCheckArtifactChance(Mobile m, BaseCreature bc)
        {
            if (m == null || bc == null)
                return false;

            double fame = (double)bc.Fame;

            if (fame > 32000)
                fame = 32000;

            double chance = this.ArtifactFactor / (Math.Max(10, 100 * (0.83 - Math.Round(Math.Log(Math.Round(fame / 6000, 3) + 0.001, 10), 3))) * (100 - Math.Sqrt(m.Luck)) / 100.0);

            return chance > Utility.RandomDouble();
        }

        public virtual void XmlGiveArtifactTo(Mobile m, BaseCreature bc)
        {
            if (m == null)
                return;

            Item item = (Item)Activator.CreateInstance(this.Artifacts[Utility.Random(this.Artifacts.Length)]);

            if (m.AddToBackpack(item))
                m.SendMessage("As a reward for slaying the mighty paragon, an artifact has been placed in your backpack.");
            else
                m.SendMessage("As your backpack is full, your reward for destroying the legendary paragon has been placed at your feet.");
        }

        #endregion

        #region attachment method overrides
        public override string OnIdentify(Mobile from)
        {
            return String.Format("Paragon: {0}", this.EnableParagon ? "Enabled" : "Disabled");
        }

        public override void OnAttach()
        {
            base.OnAttach();

            this.InvalidateParentProperties();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            this.InvalidateParentProperties();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_Hue);
            writer.Write(this.m_HitsBuff);
            writer.Write(this.m_StrBuff);
            writer.Write(this.m_IntBuff);
            writer.Write(this.m_DexBuff);
            writer.Write(this.m_SkillsBuff);
            writer.Write(this.m_SpeedBuff);
            writer.Write(this.m_FameBuff);
            writer.Write(this.m_KarmaBuff);
            writer.Write(this.m_DamageBuff);
            writer.Write(this.m_EnableParagon);
            writer.Write(this.m_ChestChance);
            writer.Write(this.m_ParagonLabel);
            writer.Write(this.m_ConvertFactor);
            writer.Write(this.m_ArtifactFactor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            this.m_Hue = reader.ReadInt();
            this.m_HitsBuff = reader.ReadDouble();
            this.m_StrBuff = reader.ReadDouble();
            this.m_IntBuff = reader.ReadDouble();
            this.m_DexBuff = reader.ReadDouble();
            this.m_SkillsBuff = reader.ReadDouble();
            this.m_SpeedBuff = reader.ReadDouble();
            this.m_FameBuff = reader.ReadDouble();
            this.m_KarmaBuff = reader.ReadDouble();
            this.m_DamageBuff = reader.ReadInt();
            this.m_EnableParagon = reader.ReadBool();
            this.m_ChestChance = reader.ReadDouble();
            this.m_ParagonLabel = reader.ReadString();
            this.m_ConvertFactor = reader.ReadDouble();
            this.m_ArtifactFactor = reader.ReadDouble();
        }

        #endregion

        #region constructors
        public XmlParagon(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlParagon()
        {
            this.Hue = 0x501;

            // standard buff modifiers
            this.HitsBuff = 5.0;
            this.StrBuff = 1.05;
            this.IntBuff = 1.20;
            this.DexBuff = 1.20;
            this.SkillsBuff = 1.20;
            this.SpeedBuff = 1.20;
            this.FameBuff = 1.40;
            this.KarmaBuff = 1.40;
            this.DamageBuff = 5;

            this.ConvertFactor = 1;
            this.ArtifactFactor = 1;
            this.ChestChance = .10;
            this.ParagonLabel = "(Paragon)";
        }
        #endregion
    }
}