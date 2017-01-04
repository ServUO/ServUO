using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
    public enum FishPieEffect
    {
        None,
        MedBoost,
        FocusBoost,
        ColdSoak,
        EnergySoak,
        FireSoak,
        PoisonSoak,
        PhysicalSoak,
        WeaponDam,
        HitChance,
        DefChance,
        SpellDamage,
        ManaRegen,
        HitsRegen,
        StamRegen,
        SoulCharge,
        CastFocus,
    }

    public class BaseFishPie : Item
    {
        public virtual TimeSpan Duration { get { return TimeSpan.FromMinutes(5); } }
        public virtual int BuffName { get { return 0; } }
        public virtual int BuffAmount { get { return 0; } }
        public virtual FishPieEffect Effect { get { return FishPieEffect.None; } }

        public static Dictionary<Mobile, List<FishPieEffect>> m_EffectsList = new Dictionary<Mobile, List<FishPieEffect>>();

        public BaseFishPie() : base(4161)
        {
            Stackable = true;
        }

        public static bool IsUnderEffects(Mobile from, FishPieEffect type)
        {
            if (!m_EffectsList.ContainsKey(from) || m_EffectsList[from] == null)
                return false;

            return m_EffectsList[from].Contains(type);
        }

        public static bool TryAddBuff(Mobile from, FishPieEffect type)
        {
            if (IsUnderEffects(from, type))
                return false;

            if (!m_EffectsList.ContainsKey(from))
                m_EffectsList.Add(from, new List<FishPieEffect>());

            m_EffectsList[from].Add(type);
            return true;
        }

        public static void RemoveBuff(Mobile from, FishPieEffect type)
        {
            if(!m_EffectsList.ContainsKey(from))
                return;

            if (m_EffectsList[from] != null && m_EffectsList[from].Contains(type))
                m_EffectsList[from].Remove(type);

            if (m_EffectsList[from] == null || m_EffectsList[from].Count == 0)
                m_EffectsList.Remove(from);

            BuffInfo.RemoveBuff(from, BuffIcon.FishPie);
            from.Delta(MobileDelta.WeaponDamage);
        }

        public static void ScaleDamage(Mobile from, ref int totalDamage, int phys, int fire, int cold, int pois, int nrgy, int direct)
        {
            if (IsUnderEffects(from, FishPieEffect.PhysicalSoak) && phys > 0)
                totalDamage -= (int)Math.Min(5.0, totalDamage * ((double)phys / 100.0));

            if (IsUnderEffects(from, FishPieEffect.FireSoak) && fire > 0)
                totalDamage -= (int)Math.Min(5.0, totalDamage * ((double)fire / 100.0));

            if (IsUnderEffects(from, FishPieEffect.ColdSoak) && cold > 0)
                totalDamage -= (int)Math.Min(5.0, totalDamage * ((double)cold / 100.0));

            if (IsUnderEffects(from, FishPieEffect.PoisonSoak) && pois > 0)
                totalDamage -= (int)Math.Min(5.0, totalDamage * ((double)pois / 100.0));

            if (IsUnderEffects(from, FishPieEffect.EnergySoak) && nrgy > 0)
                totalDamage -= (int)Math.Min(5.0, totalDamage * ((double)nrgy / 100.0));
        }

        public virtual bool Apply(Mobile from)
        {
            if (TryAddBuff(from, Effect))
            {
                switch (Effect)
                {
                    default:
                    case FishPieEffect.None: break;
                    case FishPieEffect.MedBoost:
                        TimedSkillMod mod1 = new TimedSkillMod(SkillName.Meditation, true, 10.0, Duration);
                        mod1.ObeyCap = true;
                        from.AddSkillMod(mod1);
                        break;
                    case FishPieEffect.FocusBoost:
                        TimedSkillMod mod2 = new TimedSkillMod(SkillName.Focus, true, 10.0, Duration);
                        mod2.ObeyCap = true;
                        from.AddSkillMod(mod2);
                        break;
                    case FishPieEffect.ColdSoak: break;
                    case FishPieEffect.EnergySoak: break;
                    case FishPieEffect.PoisonSoak: break;
                    case FishPieEffect.FireSoak: break;
                    case FishPieEffect.PhysicalSoak: break;
                    case FishPieEffect.WeaponDam: break;
                    case FishPieEffect.HitChance: break;
                    case FishPieEffect.DefChance: break;
                    case FishPieEffect.SpellDamage: break;
                    case FishPieEffect.ManaRegen: break;
                    case FishPieEffect.StamRegen: break;
                    case FishPieEffect.HitsRegen: break;
                    case FishPieEffect.SoulCharge: break;
                    case FishPieEffect.CastFocus: break;
                }

                if (Effect != FishPieEffect.None)
                {
                    new InternalTimer(Duration, from, Effect);

                    BuffInfo.AddBuff(from, new BuffInfo(BuffIcon.FishPie, 1116340, LabelNumber));
                }

                return true;
            }
            else
                from.SendLocalizedMessage(502173); // You are already under a similar effect.
            return false;
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;
            private FishPieEffect m_EffectType;

            public InternalTimer(TimeSpan duration, Mobile from, FishPieEffect type) : base(duration)
            {
                m_From = from;
                m_EffectType = type;
                this.Start();
            }

            protected override void OnTick()
            {
                BaseFishPie.RemoveBuff(m_From, m_EffectType);
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(BuffName, BuffAmount.ToString());
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
            }
            else if (Apply(from))
            {
                from.FixedEffect(0x375A, 10, 15);
                from.PlaySound(0x1E7);
                from.SendLocalizedMessage(1116285, String.Format("#{0}", LabelNumber)); //You eat the ~1_val~.  Mmm, tasty!
                Delete();
            }
        }

        public BaseFishPie(Serial serial) : base(serial) { }

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

    public class AutumnDragonfishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116224; } }
        public override int BuffName { get { return 1116280; } }
        public override int BuffAmount { get { return 10; } }
        public override FishPieEffect Effect { get { return FishPieEffect.MedBoost; } }

        [Constructable]
        public AutumnDragonfishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(AutumnDragonfish));
        }

        public AutumnDragonfishPie(Serial serial) : base(serial) { }

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


    public class BullFishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116220; } }
        public override int BuffName { get { return 1116276; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.WeaponDam; } }

        [Constructable]
        public BullFishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(BullFish));
        }

        public BullFishPie(Serial serial) : base(serial) { }

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


    public class CrystalFishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116219; } }
        public override int BuffName { get { return 1116275; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.EnergySoak; } }

        [Constructable]
        public CrystalFishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(CrystalFish));
        }

        public CrystalFishPie(Serial serial) : base(serial) { }

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

    public class FairySalmonPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116222; } }
        public override int BuffName { get { return 1116278; } }
        public override int BuffAmount { get { return 2; } }
        public override FishPieEffect Effect { get { return FishPieEffect.CastFocus; } }

        [Constructable]
        public FairySalmonPie()
        {
            Hue = FishInfo.GetFishHue(typeof(FairySalmon));
        }

        public FairySalmonPie(Serial serial) : base(serial) { }

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


    public class FireFishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116217; } }
        public override int BuffName { get { return 1116271; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.FireSoak; } }

        [Constructable]
        public FireFishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(FireFish));
        }

        public FireFishPie(Serial serial) : base(serial) { }

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


    public class GiantKoiPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116216; } }
        public override int BuffName { get { return 1116270; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.DefChance; } }

        [Constructable]
        public GiantKoiPie()
        {
            Hue = FishInfo.GetFishHue(typeof(GiantKoi));
        }

        public GiantKoiPie(Serial serial) : base(serial) { }

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

    public class GreatBarracudaPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116214; } }
        public override int BuffName { get { return 1116269; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.HitChance; } }

        [Constructable]
        public GreatBarracudaPie()
        {
            Hue = 1287;// FishInfo.GetFishHue(typeof(GreatBarracuda));
        }

        public GreatBarracudaPie(Serial serial) : base(serial) { }

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


    public class HolyMackerelPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116225; } }
        public override int BuffName { get { return 1116283; } }
        public override int BuffAmount { get { return 3; } }
        public override FishPieEffect Effect { get { return FishPieEffect.ManaRegen; } }

        [Constructable]
        public HolyMackerelPie()
        {
            Hue = FishInfo.GetFishHue(typeof(HolyMackerel));
        }

        public HolyMackerelPie(Serial serial) : base(serial) { }

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


    public class LavaFishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116223; } }
        public override int BuffName { get { return 1116279; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.SoulCharge; } }

        [Constructable]
        public LavaFishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(LavaFish));
        }

        public LavaFishPie(Serial serial) : base(serial) { }

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


    public class ReaperFishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116218; } }
        public override int BuffName { get { return 1116274; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.PoisonSoak; } }

        [Constructable]
        public ReaperFishPie()
        {
            Hue = 1152;// FishInfo.GetFishHue(typeof(ReaperFish));
        }

        public ReaperFishPie(Serial serial) : base(serial) { }

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


    public class SummerDragonfishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116221; } }
        public override int BuffName { get { return 1116277; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.SpellDamage; } }

        [Constructable]
        public SummerDragonfishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(SummerDragonfish));
        }

        public SummerDragonfishPie(Serial serial) : base(serial) { }

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


    public class UnicornFishPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116226; } }
        public override int BuffName { get { return 1116284; } }
        public override int BuffAmount { get { return 3; } }
        public override FishPieEffect Effect { get { return FishPieEffect.StamRegen; } }

        [Constructable]
        public UnicornFishPie()
        {
            Hue = FishInfo.GetFishHue(typeof(UnicornFish));
        }

        public UnicornFishPie(Serial serial) : base(serial) { }

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

    public class YellowtailBarracudaPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116215; } }
        public override int BuffName { get { return 1116282; } }
        public override int BuffAmount { get { return 3; } }
        public override FishPieEffect Effect { get { return FishPieEffect.HitsRegen; } }

        [Constructable]
        public YellowtailBarracudaPie()
        {
            Hue = FishInfo.GetFishHue(typeof(YellowtailBarracuda));
        }

        public YellowtailBarracudaPie(Serial serial) : base(serial) { }

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

    public class StoneCrabPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116227; } }
        public override int BuffName { get { return 1116272; } }
        public override int BuffAmount { get { return 3; } }
        public override FishPieEffect Effect { get { return FishPieEffect.PhysicalSoak; } }

        [Constructable]
        public StoneCrabPie()
        {
            Hue = FishInfo.GetFishHue(typeof(StoneCrab));
        }

        public StoneCrabPie(Serial serial) : base(serial) { }

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

    public class SpiderCrabPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116229; } }
        public override int BuffName { get { return 1116281; } }
        public override int BuffAmount { get { return 10; } }
        public override FishPieEffect Effect { get { return FishPieEffect.FocusBoost; } }

        [Constructable]
        public SpiderCrabPie()
        {
            Hue = FishInfo.GetFishHue(typeof(SpiderCrab));
        }

        public SpiderCrabPie(Serial serial) : base(serial) { }

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

    public class BlueLobsterPie : BaseFishPie
    {
        public override int LabelNumber { get { return 1116228; } }
        public override int BuffName { get { return 1116273; } }
        public override int BuffAmount { get { return 5; } }
        public override FishPieEffect Effect { get { return FishPieEffect.ColdSoak; } }

        [Constructable]
        public BlueLobsterPie()
        {
            Hue = FishInfo.GetFishHue(typeof(BlueLobster));
        }

        public BlueLobsterPie(Serial serial) : base(serial) { }

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
}