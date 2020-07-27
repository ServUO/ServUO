using Server.Targeting;
using System;
using System.Collections.Generic;

namespace Server.Spells.First
{
    public class WeakenSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Weaken", "Des Mani",
            212,
            9031,
            Reagent.Garlic,
            Reagent.Nightshade);

        public static Dictionary<Mobile, Timer> m_Table = new Dictionary<Mobile, Timer>();

        public static bool IsUnderEffects(Mobile m)
        {
            return m_Table.ContainsKey(m);
        }

        public static void RemoveEffects(Mobile m, bool removeMod = true)
        {
            if (m_Table.ContainsKey(m))
            {
                Timer t = m_Table[m];

                if (t != null && t.Running)
                {
                    t.Stop();
                }

                BuffInfo.RemoveBuff(m, BuffIcon.Weaken);

                if (removeMod)
                    m.RemoveStatMod("[Magic] Str Curse");

                m_Table.Remove(m);
            }
        }

        public WeakenSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.First;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (CheckHSequence(m))
            {
                SpellHelper.Turn(Caster, m);
                SpellHelper.CheckReflect(this, Caster, ref m);

                if (Mysticism.StoneFormSpell.CheckImmunity(m))
                {
                    Caster.SendLocalizedMessage(1080192); // Your target resists your ability reduction magic.
                    return;
                }

                int oldOffset = SpellHelper.GetCurseOffset(m, StatType.Str);
                int newOffset = SpellHelper.GetOffset(Caster, m, StatType.Str, true, false);

                if (-newOffset > oldOffset || newOffset == 0)
                {
                    DoHurtFizzle();
                }
                else
                {
                    if (m.Spell != null)
                        m.Spell.OnCasterHurt();

                    m.Paralyzed = false;

                    m.FixedParticles(0x3779, 10, 15, 5002, EffectLayer.Head);
                    m.PlaySound(0x1DF);

                    HarmfulSpell(m);

                    if (-newOffset < oldOffset)
                    {
                        SpellHelper.AddStatCurse(Caster, m, StatType.Str, false, newOffset);

                        int percentage = (int)(SpellHelper.GetOffsetScalar(Caster, m, true) * 100);
                        TimeSpan length = SpellHelper.GetDuration(Caster, m);
                        BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Weaken, 1075837, length, m, percentage.ToString()));

                        if (m_Table.ContainsKey(m))
                            m_Table[m].Stop();

                        m_Table[m] = Timer.DelayCall(length, () =>
                        {
                            RemoveEffects(m);
                        });
                    }
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly WeakenSpell m_Owner;
            public InternalTarget(WeakenSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
