using System;
using Server.Targeting;
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

        public static void RemoveEffects(Mobile m)
        {
            if (m_Table.ContainsKey(m))
            {
                Timer t = m_Table[m];

                if (t != null && t.Running)
                {
                    t.Stop();
                }

                m.RemoveStatMod("[Magic] Dex Curse");

                m_Table.Remove(m);
            }
        }

        public WeakenSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
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
				int percentage = (int)(SpellHelper.GetOffsetScalar(this.Caster, m, true) * 100);
				TimeSpan length = SpellHelper.GetDuration(this.Caster, m);
				BuffInfo.AddBuff(m, new BuffInfo(BuffIcon.Weaken, 1075837, length, m, percentage.ToString()));

				if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                m.FixedParticles(0x3779, 10, 15, 5009, EffectLayer.Waist);
                m.PlaySound(0x1E6);

                this.HarmfulSpell(m);

                m_Table[m] = Timer.DelayCall<Mobile>(length, RemoveEffects, m);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly WeakenSpell m_Owner;
            public InternalTarget(WeakenSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Harmful)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                {
                    this.m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}