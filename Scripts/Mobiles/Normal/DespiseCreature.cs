using Server;
using System;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.SkillHandlers;

namespace Server.Mobiles
{
    public enum Alignment
    {
        Neutral,
        Good,
        Evil
    }
}

namespace Server.Engines.Despise
{
    public class DespiseCreature : BaseCreature
    {
        private WispOrb m_Orb;
        private int m_Power;
        private int m_MaxPower;
        private int m_Progress;

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual Alignment Alignment
        {
            get
            {
                if (Karma > 0)
                    return Alignment.Good;
                if (Karma < 0)
                    return Alignment.Evil;
                return Alignment.Neutral;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public WispOrb Orb { get { return m_Orb; } set { m_Orb = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxPower { get { return m_MaxPower; } set { m_MaxPower = value; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Power
        {
            get { return m_Power; }
            set
            {
                int oldPower = m_Power;

                if (value > m_MaxPower)
                    m_Power = m_MaxPower;

                if (oldPower < value)
                {
                    m_Power = value;
                    IncreasePower();
                    InvalidateProperties();
                }

                if (m_Orb != null)
                    m_Orb.InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Progress
        {
            get { return m_Progress; }
            set
            {
                m_Progress = value;

                if (m_Progress >= m_Power)
                {
                    Power++;
                    m_Progress = 0;
                }

                if (m_Orb != null)
                    m_Orb.InvalidateProperties();
            }
        }

        public virtual int TightLeashLength { get { return 2; } }
        public virtual int ShortLeashLength { get { return 5; } }
        public virtual int LongLeashLength { get { return 12; } }

        public virtual int StatRatio { get { return Utility.RandomMinMax(35, 60); } }

        public virtual double SkillStart { get { return Utility.RandomMinMax(35, 50); } }
        public virtual double SkillMax { get { return m_MaxPower == 15 ? 130.0 : 110.0; } }

        public virtual int StrStart { get { return Utility.RandomMinMax(91, 100); } }
        public virtual int DexStart { get { return Utility.RandomMinMax(91, 100); } }
        public virtual int IntStart { get { return Utility.RandomMinMax(91, 100); } }

        public virtual int StrMax { get { return 600; } }
        public virtual int DexMax { get { return 150; } }
        public virtual int IntMax { get { return 450; } }

        public virtual int HitsStart { get { return StrStart + (int)((double)StrStart * ((double)StatRatio / 100.0)); } }
        public virtual int StamStart { get { return DexStart + (int)((double)DexStart * ((double)StatRatio / 100.0)); } }
        public virtual int ManaStart { get { return DexStart + (int)((double)DexStart * ((double)StatRatio / 100.0)); } }

        public virtual int MaxHits { get { return 1000; } }
        public virtual int MaxStam { get { return 1000; } }
        public virtual int MaxMana { get { return 1500; } }

        public virtual int MinDamStart { get { return 8; } }
        public virtual int MaxDamStart { get { return 13; } }

        public virtual int MinDamMax { get { return 12; } }
        public virtual int MaxDamMax { get { return 17; } }

        public virtual bool RaiseDamage { get { return false; } }
        public virtual double RaiseDamageFactor { get { return 0.33; } }

        public virtual int GetFame { get { return m_Power * 500; } }
        public virtual int GetKarmaGood { get { return m_Power * 500; } }
        public virtual int GetKarmaEvil { get { return m_Power * -500; } }

        public override bool Commandable { get { return false; } }

        public override bool InitialInnocent { get { return Alignment < Alignment.Evil; } }
        public override bool AlwaysMurderer { get { return Alignment == Alignment.Evil; } }
        public override bool ForceNotoriety { get { return true; } }
        public override bool IsBondable { get { return false; } }
        public override bool GivesFameAndKarmaAward { get { return false; } }

        public DespiseCreature(AIType ai, FightMode fightmode)
            : base(ai, fightmode, 10, 1, .2, .4)
        {
            m_MaxPower = 10;
            m_Power = 1;

            SetStr(StrStart);
            SetDex(DexStart);
            SetInt(IntStart);

            SetHits(HitsStart);
            SetStam(StamStart);
            SetMana(ManaStart);

            SetDamage(MinDamStart, MaxDamStart);
        }

        public override void GenerateLoot(bool spawning)
        {
            if (spawning)
                Timer.DelayCall(TimeSpan.FromSeconds(.5), new TimerCallback(GenerateLoot_Callback));
            else
                base.GenerateLoot(spawning);
        }

        public void GenerateLoot_Callback()
        {
            base.GenerateLoot(true);
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosRich, Math.Max(1, m_Power / 2));
        }

        public override bool CanBeRenamedBy(Mobile from)
        {
            if (from.AccessLevel > AccessLevel.Player)
                return base.CanBeRenamedBy(from);

            return false;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1153297, String.Format("#{0}\t{1}", GetPowerLabel(m_Power), m_Power.ToString())); // Power Level: ~1_LEVEL~: ~2_VAL~

            if (ControlMaster != null)
                list.Add(1153303, ControlMaster.Name); // Controller: ~1_NAME~
        }

        public override void OnCombatantChange()
        {
            base.OnCombatantChange();

            if (m_Orb != null)
                m_Orb.InvalidateHue();
        }

        public override void OnKarmaChange(int oldValue)
        {
            if ((oldValue < 0 && Karma > 0) || (oldValue > 0 && Karma < 0))
            {
                switch (Alignment)
                {
                    case Alignment.Good: FightMode = FightMode.Evil; break;
                    case Alignment.Evil: FightMode = FightMode.Good; break;
                    default: FightMode = FightMode.Aggressor; break;
                }
            }
        }

        public override void OnDeath(Container c)
        {
            base.OnDeath(c);

            if (m_Orb != null)
            {
                if (m_Orb.Owner != null)
                    m_Orb.Owner.SendLocalizedMessage(1153312); // The Wisp Orb dissolves into aether.

                m_Orb.Delete();
            }
        }

        public override void Delete()
        {
            base.Delete();

            if (m_Orb != null && !m_Orb.Deleted)
                m_Orb.Pet = null;
        }

        public int GetLeashLength()
        {
            if (m_Orb == null)
                return RangePerception;

            switch (m_Orb.LeashLength)
            {
                case LeashLength.Tight: return TightLeashLength;
                case LeashLength.Short: return ShortLeashLength;
                default:
                case LeashLength.Long: return LongLeashLength;
            }
        }

        public void Link(WispOrb orb)
        {
            m_Orb = orb;
            RangeHome = 2;
            m_Orb.InvalidateHue();
        }

        public void Unlink()
        {
            RangeHome = 10;
            SetControlMaster(null);
            PublicOverheadMessage(MessageType.Regular, 0x59, 1153296); // * This creature is no longer influenced by a Wisp Orb *

            if (m_Orb != null)
            {
                m_Orb.Conscripted = false;
                m_Orb.OnUnlinkPet();
                m_Orb.InvalidateHue();

                m_Orb = null;
            }
        }

        public virtual void IncreasePower()
        {
            foreach (Skill skill in Skills)
            {
                if (skill != null && skill.Base > 0 && skill.Base < SkillMax)
                {
                    double toRaise = ((SkillMax / m_MaxPower) * m_Power) + Utility.RandomMinMax(-5, 5);

                    if (toRaise > skill.Base)
                        skill.Base = Math.Min(SkillMax, toRaise);
                }
            }

            int strRaise = ((StrMax / 15) * m_Power) + Utility.RandomMinMax(-5, 5);
            int dexRaise = ((DexMax / 15) * m_Power) + Utility.RandomMinMax(-5, 5);
            int intRaise = ((IntMax / 15) * m_Power) + Utility.RandomMinMax(-5, 5);

            if (strRaise > RawStr)
                SetStr(Math.Min(StrMax, strRaise));

            if (dexRaise > RawDex)
                SetDex(Math.Min(DexMax, dexRaise));

            if (intRaise > RawInt)
                SetInt(Math.Min(IntMax, intRaise));

            int hitsRaise = ((MaxHits / 15) * m_Power) + Utility.RandomMinMax(-5, 5);
            int stamRaise = ((MaxStam / 15) * m_Power) + Utility.RandomMinMax(-5, 5);
            int manaRaise = ((MaxMana / 15) * m_Power) + Utility.RandomMinMax(-5, 5);

            if (hitsRaise > HitsMax)
                SetHits(Math.Min(MaxHits, hitsRaise));

            if (stamRaise > StamMax)
                SetStam(Math.Min(MaxStam, stamRaise));

            if (manaRaise > ManaMax)
                SetMana(Math.Min(MaxMana, manaRaise));

            if (RaiseDamage && Utility.RandomDouble() < RaiseDamageFactor)
            {
                DamageMin = Math.Min(MinDamMax, DamageMin + 1);
                DamageMax = Math.Min(MaxDamMax, DamageMax + 1);
            }

            FixedEffect(0x373A, 10, 30);
            PlaySound(0x209);
        }

        public static int GetPowerLabel(int power)
        {
            switch (power)
            {
                default:
                case 0:
                case 1:
                case 3: return 1153298; // Normal
                case 4:
                case 5:
                case 6: return 1153299; // Improved
                case 7:
                case 8: return 1153300; // Heightened
                case 9:
                case 10: return 1153301; // Magnified
                case 11:
                case 12: return 1153302; // Amplified
                case 13:
                case 14: return 1153307; // Inspired
                case 15: return 1153308; // Galvanized
            }
        }

        public DespiseCreature(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
            writer.Write(m_Orb);
            writer.Write(m_Power);
            writer.Write(m_MaxPower);
            writer.Write(m_Progress);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();
            m_Orb = reader.ReadItem() as WispOrb;
            m_Power = reader.ReadInt();
            m_MaxPower = reader.ReadInt();
            m_Progress = reader.ReadInt();
        }
    }
}