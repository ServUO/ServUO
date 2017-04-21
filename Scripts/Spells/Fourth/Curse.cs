using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<Mobile, Timer> m_UnderEffect = new Dictionary<Mobile, Timer>();

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

        public static void AddEffect(Mobile m, TimeSpan duration)
        {
            if (m == null)
                return;

            m_UnderEffect[m] = Timer.DelayCall<Mobile>(duration, RemoveEffect, m);
            m.UpdateResistances();
        }

        public static void RemoveEffect(object state)
        {
            Mobile m = (Mobile)state;

            m.RemoveStatMod("[Magic] Str Curse");
            m.RemoveStatMod("[Magic] Dex Curse");
            m.RemoveStatMod("[Magic] Int Curse");

            BuffInfo.RemoveBuff(m, BuffIcon.Curse);

            if(m_UnderEffect.ContainsKey(m))
            {
                Timer t = m_UnderEffect[m];
                
                if(t != null)
                    t.Stop();
                
                m_UnderEffect.Remove(m);
            }
            
            m.UpdateResistances();
        }

        public static bool UnderEffect(Mobile m)
        {
            return m_UnderEffect.ContainsKey(m);
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public static void DoCurse(Mobile caster, Mobile m, bool masscurse)
        {
            SpellHelper.AddStatCurse(caster, m, StatType.Str);
            SpellHelper.DisableSkillCheck = true;
            SpellHelper.AddStatCurse(caster, m, StatType.Dex);
            SpellHelper.AddStatCurse(caster, m, StatType.Int);
            SpellHelper.DisableSkillCheck = false;

            int percentage = (int)(SpellHelper.GetOffsetScalar(caster, m, true) * 100);
            TimeSpan length = SpellHelper.GetDuration(caster, m);
            string args;

            if (masscurse)
            {
                args = String.Format("{0}\t{0}\t{0}", percentage);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.MassCurse, 1075839, length, m, args));
            }
            else
            {
                args = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}", percentage, percentage, percentage, 10, 10, 10, 10);
                BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Curse, 1075835, 1075836, length, m, args.ToString()));
            }

            if (!m_UnderEffect.ContainsKey(m))
            {
                AddEffect(m, SpellHelper.GetDuration(caster, m));
            }

            if (m.Spell != null)
                m.Spell.OnCasterHurt();

            m.Paralyzed = false;

            m.FixedParticles(0x374A, 10, 15, 5028, EffectLayer.Waist);
            m.PlaySound(0x1E1);
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

				DoCurse(this.Caster, m, false);

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
