using System;
using Server.Targeting;
using System.Collections.Generic;

namespace Server.Spells.First
{
    public class FeeblemindSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Feeblemind", "Rel Wis",
            212,
            9031,
            Reagent.Ginseng,
            Reagent.Nightshade);
        public FeeblemindSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

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

                BuffInfo.RemoveBuff(m, BuffIcon.FeebleMind);

                if (removeMod)
                    m.RemoveStatMod("[Magic] Int Curse");

                m_Table.Remove(m);
            }
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.First;
            }
        }
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

                SpellHelper.CheckReflect((int)Circle, Caster, ref m);

                int oldOffset = SpellHelper.GetCurseOffset(m, StatType.Int);
				SpellHelper.AddStatCurse(Caster, m, StatType.Int, false);
                int newOffset = SpellHelper.GetCurseOffset(m, StatType.Int);

				if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                m.FixedParticles(0x3779, 10, 15, 5004, EffectLayer.Head);
                m.PlaySound(0x1E4);

                HarmfulSpell(m);

                if (newOffset < oldOffset)
                {
                    int percentage = (int)(SpellHelper.GetOffsetScalar(Caster, m, true) * 100);
                    TimeSpan length = SpellHelper.GetDuration(Caster, m);

                    BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.FeebleMind, 1075833, length, m, percentage.ToString()));

                    if (m_Table.ContainsKey(m))
                        m_Table[m].Stop();

                    m_Table[m] = Timer.DelayCall(length, () =>
                    {
                        RemoveEffects(m);
                    });
                }
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly FeeblemindSpell m_Owner;
            public InternalTarget(FeeblemindSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
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