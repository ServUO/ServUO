using Server.Items;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Ninjitsu;
using Server.Spells.Second;
using Server.Spells.Third;
using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Spells.Mysticism
{
    public enum BuffType
    {
        None,
        MagicReflect,
        ReactiveArmor,
        Protection,
        Transformation,
        StrBonus,
        DexBonus,
        IntBonus,
        BarrabHemolymph,
        UraliTrance,
        Bless
    }

    public class PurgeMagicSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.Second;

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Purge", "An Ort Sanct ",
                230,
                9022,
                Reagent.Garlic,
                Reagent.MandrakeRoot,
                Reagent.SulfurousAsh,
                Reagent.FertileDirt
            );

        public PurgeMagicSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void OnTarget(object o)
        {
            Mobile target = o as Mobile;

            if (target == null)
                return;

            if (m_CurseTable.ContainsKey(Caster))
            {
                Caster.SendLocalizedMessage(1154212); //You may not use the Purge Magic spell while you are under its curse.
            }
            else if (m_ImmuneTable.ContainsKey(target) || m_CurseTable.ContainsKey(target))
            {
                Caster.SendLocalizedMessage(1080119); // Your Purge Magic has been resisted!
            }
            else if (CheckHSequence(target))
            {
                if (CheckResisted(target))
                {
                    target.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                    Caster.SendLocalizedMessage(1080119); //Your Purge Magic has been resisted!
                }
                else
                {
                    SpellHelper.CheckReflect(this, Caster, ref target);

                    Caster.PlaySound(0x655);
                    Effects.SendLocationParticles(EffectItem.Create(target.Location, target.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 0x834, 0, 0x13B2, 0);

                    BuffType type = GetRandomBuff(target);

                    if (type != BuffType.None)
                    {
                        string arg = "";

                        switch (type)
                        {
                            case BuffType.MagicReflect:
                                MagicReflectSpell.EndReflect(target);
                                arg = "magic reflect";
                                break;
                            case BuffType.ReactiveArmor:
                                ReactiveArmorSpell.EndArmor(target);
                                arg = "reactive armor";
                                break;
                            case BuffType.Protection:
                                ProtectionSpell.EndProtection(target);
                                arg = "protection";
                                break;
                            case BuffType.Transformation:
                                TransformationSpellHelper.RemoveContext(target, true);
                                arg = "transformation spell";
                                break;
                            case BuffType.StrBonus:
                                arg = "strength bonus";
                                target.RemoveStatMod("[Magic] Str Buff");
                                BuffInfo.RemoveBuff(target, BuffIcon.Strength);
                                break;
                            case BuffType.DexBonus:
                                arg = "dexterity bonus";
                                target.RemoveStatMod("[Magic] Dex Buff");
                                BuffInfo.RemoveBuff(target, BuffIcon.Agility);
                                break;
                            case BuffType.IntBonus:
                                arg = "intelligence bonus";
                                target.RemoveStatMod("[Magic] Int Buff");
                                BuffInfo.RemoveBuff(target, BuffIcon.Cunning);
                                break;
                            case BuffType.BarrabHemolymph:
                                arg = "Barrab hemolymph";
                                EodonianPotion.RemoveEffects(target, PotionEffect.Barrab);
                                break;
                            case BuffType.UraliTrance:
                                arg = "Urali Trance";
                                EodonianPotion.RemoveEffects(target, PotionEffect.Urali);
                                break;
                            case BuffType.Bless:
                                arg = "bless";
                                target.RemoveStatMod("[Magic] Str Buff");
                                target.RemoveStatMod("[Magic] Dex Buff");
                                target.RemoveStatMod("[Magic] Int Buff");
                                BuffInfo.RemoveBuff(target, BuffIcon.Bless);
                                BlessSpell.RemoveBless(target);
                                break;
                        }

                        target.SendLocalizedMessage(1080117, arg); //Your ~1_ABILITY_NAME~ has been purged.
                        Caster.SendLocalizedMessage(1080118, arg); //Your target's ~1_ABILITY_NAME~ has been purged.

                        int duration = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 15);

                        if (duration <= 0)
                            duration = 1;

                        m_ImmuneTable.Add(target, new ImmuneTimer(target, TimeSpan.FromSeconds(duration)));
                    }
                    else
                    {
                        Caster.SendLocalizedMessage(1080120); //Your target has no magic that can be purged.

                        int duration = (int)((Caster.Skills[CastSkill].Value + Caster.Skills[DamageSkill].Value) / 28);

                        if (duration <= 0)
                            duration = 1;

                        m_CurseTable.Add(target, new CurseTimer(target, Caster, TimeSpan.FromSeconds(duration)));
                    }
                }
            }

            FinishSequence();
        }

        public BuffType GetRandomBuff(Mobile target)
        {
            List<BuffType> buffs = new List<BuffType>();

            if (MagicReflectSpell.HasReflect(target))
                buffs.Add(BuffType.MagicReflect);

            if (ReactiveArmorSpell.HasArmor(target))
                buffs.Add(BuffType.ReactiveArmor);

            if (ProtectionSpell.HasProtection(target))
                buffs.Add(BuffType.Protection);

            TransformContext context = TransformationSpellHelper.GetContext(target);

            if (context != null && context.Type != typeof(AnimalForm))
                buffs.Add(BuffType.Transformation);

            if (BlessSpell.IsBlessed(target))
            {
                buffs.Add(BuffType.Bless);
            }
            else
            {
                StatMod mod = target.GetStatMod("[Magic] Str Buff");
                if (mod != null)
                    buffs.Add(BuffType.StrBonus);

                mod = target.GetStatMod("[Magic] Dex Buff");
                if (mod != null)
                    buffs.Add(BuffType.DexBonus);

                mod = target.GetStatMod("[Magic] Int Buff");
                if (mod != null)
                    buffs.Add(BuffType.IntBonus);
            }

            if (EodonianPotion.IsUnderEffects(target, PotionEffect.Barrab))
                buffs.Add(BuffType.BarrabHemolymph);

            if (EodonianPotion.IsUnderEffects(target, PotionEffect.Urali))
                buffs.Add(BuffType.UraliTrance);

            if (buffs.Count == 0)
                return BuffType.None;

            BuffType type = buffs[Utility.Random(buffs.Count)];
            buffs.Clear();

            return type;
        }

        private static readonly Dictionary<Mobile, ImmuneTimer> m_ImmuneTable = new Dictionary<Mobile, ImmuneTimer>();
        private static readonly Dictionary<Mobile, CurseTimer> m_CurseTable = new Dictionary<Mobile, CurseTimer>();

        public static void RemoveImmunity(Mobile from)
        {
            if (m_ImmuneTable.ContainsKey(from))
            {
                m_ImmuneTable[from].Stop();
                m_ImmuneTable.Remove(from);
            }
        }

        public static void RemoveCurse(Mobile from, Mobile caster)
        {
            if (m_CurseTable.ContainsKey(from))
            {
                m_CurseTable[from].Stop();

                if (DateTime.UtcNow > m_CurseTable[from].StartTime)
                {
                    TimeSpan inEffect = DateTime.UtcNow - m_CurseTable[from].StartTime;

                    int damage = 5 * (int)inEffect.TotalSeconds;

                    from.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                    from.PlaySound(0x307);

                    m_CurseTable.Remove(from);

                    AOS.Damage(from, caster, damage, 0, 0, 0, 0, 0, 100, 0);
                }
            }

            m_ImmuneTable[from] = new ImmuneTimer(from, TimeSpan.FromSeconds(16));
        }

        public static void OnMobileDoDamage(Mobile from)
        {
            if (from != null && m_CurseTable.ContainsKey(from))
                RemoveCurse(from, m_CurseTable[from].Caster);
        }

        public static bool IsUnderCurseEffects(Mobile from)
        {
            return m_CurseTable.ContainsKey(from);
        }

        private class ImmuneTimer : Timer
        {
            private readonly Mobile m_Mobile;

            public ImmuneTimer(Mobile mob, TimeSpan duration) : base(duration)
            {
                m_Mobile = mob;
                Start();
            }

            protected override void OnTick()
            {
                RemoveImmunity(m_Mobile);
            }
        }

        private class CurseTimer : Timer
        {
            private readonly Mobile m_Mobile;
            private readonly Mobile m_Caster;
            private readonly DateTime m_StartTime;

            public DateTime StartTime => m_StartTime;
            public Mobile Caster => m_Caster;

            public CurseTimer(Mobile mob, Mobile caster, TimeSpan duration)
                : base(duration)
            {
                m_Mobile = mob;
                m_Caster = caster;
                m_StartTime = DateTime.UtcNow;
                Start();
            }

            protected override void OnTick()
            {
                RemoveCurse(m_Mobile, m_Caster);
            }
        }

        public class InternalTarget : Target
        {
            public PurgeMagicSpell Owner { get; set; }

            public InternalTarget(PurgeMagicSpell owner)
                : this(owner, false)
            {
            }

            public InternalTarget(PurgeMagicSpell owner, bool allowland)
                : base(12, allowland, TargetFlags.Harmful)
            {
                Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o == null)
                    return;

                if (!from.CanSee(o))
                    from.SendLocalizedMessage(500237); // Target can not be seen.
                else
                {
                    SpellHelper.Turn(from, o);
                    Owner.OnTarget(o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                Owner.FinishSequence();
            }
        }
    }
}
