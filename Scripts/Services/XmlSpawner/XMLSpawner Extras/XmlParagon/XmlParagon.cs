using System;
using Server;
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
			typeof( GoldBricks ), typeof( PhillipsWoodenSteed ), 
			typeof( AlchemistsBauble ), typeof( ArcticDeathDealer ),
			typeof( BlazeOfDeath ), typeof( BowOfTheJukaKing ),
			typeof( BurglarsBandana ), typeof( CavortingClub ),
			typeof( EnchantedTitanLegBone ), typeof( GwennosHarp ),
			typeof( IolosLute ), typeof( LunaLance ),
			typeof( NightsKiss ), typeof( NoxRangersHeavyCrossbow ),
			typeof( OrcishVisage ), typeof( PolarBearMask ),
			typeof( ShieldOfInvulnerability ), typeof( StaffOfPower ),
			typeof( VioletCourage ), typeof( HeartOfTheLion ), 
			typeof( WrathOfTheDryad ), typeof( PixieSwatter ), 
			typeof( GlovesOfThePugilist )
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

        public virtual Type[] Artifacts { get { return m_Artifacts; } set { m_Artifacts = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double ConvertFactor { get { return m_ConvertFactor; } set { m_ConvertFactor = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double ArtifactFactor { get { return m_ArtifactFactor; } set { m_ArtifactFactor = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int Hue { get { return m_Hue; } set { m_Hue = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HitsBuff { get { return m_HitsBuff; } set { m_HitsBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double StrBuff { get { return m_StrBuff; } set { m_StrBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double IntBuff { get { return m_IntBuff; } set { m_IntBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double DexBuff { get { return m_DexBuff; } set { m_DexBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SkillsBuff { get { return m_SkillsBuff; } set { m_SkillsBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SpeedBuff { get { return m_SpeedBuff; } set { m_SpeedBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double FameBuff { get { return m_FameBuff; } set { m_FameBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double KarmaBuff { get { return m_KarmaBuff; } set { m_KarmaBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual int DamageBuff { get { return m_DamageBuff; } set { m_DamageBuff = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double ChestChance { get { return m_ChestChance; } set { m_ChestChance = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool EnableParagon { get { return m_EnableParagon; } set { m_EnableParagon = value; InvalidateParentProperties();  } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual string ParagonLabel { get { return m_ParagonLabel; } set { m_ParagonLabel = value; } }
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
            if (bc == null) return;

            bc.PackItem(new ParagonChest(bc.Name, treasureLevel));
        }

        public virtual double XmlChestChance(BaseCreature bc)
        {
            return ChestChance;
        }


        public virtual string XmlGetParagonLabel(BaseCreature bc)
        {
            return ParagonLabel;
        }

        public virtual void XmlConvert(BaseCreature bc)
        {
            if (bc == null || bc.IsParagon)
                return;

            bc.Hue = Hue;

            if (bc.HitsMaxSeed >= 0)
                bc.HitsMaxSeed = (int)(bc.HitsMaxSeed * HitsBuff);

            bc.RawStr = (int)(bc.RawStr * StrBuff);
            bc.RawInt = (int)(bc.RawInt * IntBuff);
            bc.RawDex = (int)(bc.RawDex * DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            for (int i = 0; i < bc.Skills.Length; i++)
            {
                Skill skill = (Skill)bc.Skills[i];

                if (skill.Base > 0.0)
                    skill.Base *= SkillsBuff;
            }

            bc.PassiveSpeed /= SpeedBuff;
            bc.ActiveSpeed /= SpeedBuff;

            bc.DamageMin += DamageBuff;
            bc.DamageMax += DamageBuff;

            if (bc.Fame > 0)
                bc.Fame = (int)(bc.Fame * FameBuff);

            if (bc.Fame > 32000)
                bc.Fame = 32000;

            // TODO: Mana regeneration rate = Sqrt( buffedFame ) / 4

            if (bc.Karma != 0)
            {
                bc.Karma = (int)(bc.Karma * KarmaBuff);

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
                bc.HitsMaxSeed = (int)(bc.HitsMaxSeed / HitsBuff);

            bc.RawStr = (int)(bc.RawStr / StrBuff);
            bc.RawInt = (int)(bc.RawInt / IntBuff);
            bc.RawDex = (int)(bc.RawDex / DexBuff);

            bc.Hits = bc.HitsMax;
            bc.Mana = bc.ManaMax;
            bc.Stam = bc.StamMax;

            for (int i = 0; i < bc.Skills.Length; i++)
            {
                Skill skill = (Skill)bc.Skills[i];

                if (skill.Base > 0.0)
                    skill.Base /= SkillsBuff;
            }

            bc.PassiveSpeed *= SpeedBuff;
            bc.ActiveSpeed *= SpeedBuff;

            bc.DamageMin -= DamageBuff;
            bc.DamageMax -= DamageBuff;

            if (bc.Fame > 0)
                bc.Fame = (int)(bc.Fame / FameBuff);
            if (bc.Karma != 0)
                bc.Karma = (int)(bc.Karma / KarmaBuff);
        }

        public virtual bool XmlCheckConvert(BaseCreature bc, Point3D location, Map m)
        {
            if (!EnableParagon || bc == null)
                return false;

            if (bc is BaseChampion || bc is Harrower || bc is BaseVendor || bc is BaseEscortable || bc is Clone)
                return false;

            int fame = bc.Fame;

            if (fame > 32000)
                fame = 32000;

            double chance = ConvertFactor / Math.Round(20.0 - (fame / 3200));

            return (chance > Utility.RandomDouble());
        }

        public virtual bool XmlCheckArtifactChance(Mobile m, BaseCreature bc)
        {
            if (m == null || bc == null) return false;

            double fame = (double)bc.Fame;

            if (fame > 32000)
                fame = 32000;

            double chance = ArtifactFactor / (Math.Max(10, 100 * (0.83 - Math.Round(Math.Log(Math.Round(fame / 6000, 3) + 0.001, 10), 3))) * (100 - Math.Sqrt(m.Luck)) / 100.0);

            return chance > Utility.RandomDouble();
        }

        public virtual void XmlGiveArtifactTo(Mobile m, BaseCreature bc)
        {
            if (m == null) return;

            Item item = (Item)Activator.CreateInstance(Artifacts[Utility.Random(Artifacts.Length)]);

            if (m.AddToBackpack(item))
                m.SendMessage("As a reward for slaying the mighty paragon, an artifact has been placed in your backpack.");
            else
                m.SendMessage("As your backpack is full, your reward for destroying the legendary paragon has been placed at your feet.");
        }

        #endregion

        #region attachment method overrides
        public override string OnIdentify(Mobile from)
        {
            return String.Format("Paragon: {0}", EnableParagon ? "Enabled" : "Disabled");
        }

        public override void OnAttach()
        {
            base.OnAttach();

            InvalidateParentProperties();
        }

        public override void OnDelete()
        {
            base.OnDelete();

            InvalidateParentProperties();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(m_Hue);
            writer.Write(m_HitsBuff);
            writer.Write(m_StrBuff);
            writer.Write(m_IntBuff);
            writer.Write(m_DexBuff);
            writer.Write(m_SkillsBuff);
            writer.Write(m_SpeedBuff);
            writer.Write(m_FameBuff);
            writer.Write(m_KarmaBuff);
            writer.Write(m_DamageBuff);
            writer.Write(m_EnableParagon);
            writer.Write(m_ChestChance);
            writer.Write(m_ParagonLabel);
            writer.Write(m_ConvertFactor);
            writer.Write(m_ArtifactFactor);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            // version 0
            m_Hue = reader.ReadInt();
            m_HitsBuff = reader.ReadDouble();
            m_StrBuff = reader.ReadDouble();
            m_IntBuff = reader.ReadDouble();
            m_DexBuff = reader.ReadDouble();
            m_SkillsBuff = reader.ReadDouble();
            m_SpeedBuff = reader.ReadDouble();
            m_FameBuff = reader.ReadDouble();
            m_KarmaBuff = reader.ReadDouble();
            m_DamageBuff = reader.ReadInt();
            m_EnableParagon = reader.ReadBool();
            m_ChestChance = reader.ReadDouble();
            m_ParagonLabel = reader.ReadString();
            m_ConvertFactor = reader.ReadDouble();
            m_ArtifactFactor = reader.ReadDouble();
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
            Hue = 0x501;

            // standard buff modifiers
            HitsBuff = 5.0;
            StrBuff = 1.05;
            IntBuff = 1.20;
            DexBuff = 1.20;
            SkillsBuff = 1.20;
            SpeedBuff = 1.20;
            FameBuff = 1.40;
            KarmaBuff = 1.40;
            DamageBuff = 5;

            ConvertFactor = 1;
            ArtifactFactor = 1;
            ChestChance = .10;
            ParagonLabel = "(Paragon)";
        }
        #endregion
    }
}