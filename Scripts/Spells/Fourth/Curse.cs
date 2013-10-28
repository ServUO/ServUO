using System;
using System.Collections;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class CurseSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Curse", "Des Sanct",
            227,
            9031,
            Reagent.Nightshade,
            Reagent.Garlic,
            Reagent.SulfurousAsh);
        private static readonly Hashtable m_UnderEffect = new Hashtable();
        public CurseSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Fourth;
            }
        }
        public static void RemoveEffect(object state)
        {
            Mobile m = (Mobile)state;

            m_UnderEffect.Remove(m);

            m.UpdateResistances();
        }

        public static bool UnderEffect(Mobile m)
        {
            return m_UnderEffect.Contains(m);
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.CanSee(m))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (this.CheckHSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                SpellHelper.CheckReflect((int)this.Circle, this.Caster, ref m);

                SpellHelper.AddStatCurse(this.Caster, m, StatType.Str);
                SpellHelper.DisableSkillCheck = true;
                SpellHelper.AddStatCurse(this.Caster, m, StatType.Dex);
                SpellHelper.AddStatCurse(this.Caster, m, StatType.Int);
                SpellHelper.DisableSkillCheck = false;

                Timer t = (Timer)m_UnderEffect[m];

                if (this.Caster.Player && m.Player /*&& Caster != m */ && t == null)	//On OSI you CAN curse yourself and get this effect.
                {
                    TimeSpan duration = SpellHelper.GetDuration(this.Caster, m);
                    m_UnderEffect[m] = t = Timer.DelayCall(duration, new TimerStateCallback(RemoveEffect), m);
                    m.UpdateResistances();
                }

                if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
                m.PlaySound(0x1E1);

                int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, true) * 100);
                TimeSpan length = SpellHelper.GetDuration(this.Caster, m);

                string args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", percentage, percentage, percentage, 10, 10, 10, 10);

                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Curse, 1075835, 1075836, length, m, args.ToString()));

                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly CurseSpell m_Owner;
            public InternalTarget(CurseSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    this.m_Owner.Target((Mobile)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}