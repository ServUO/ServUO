using Server.Targeting;
using System;

namespace Server.Spells.Spellweaving
{
    public class WordOfDeathSpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo("Word of Death", "Nyraxle", -1);
        public WordOfDeathSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(3.5);
        public override double RequiredSkill => 83.0;
        public override int RequiredMana => 50;
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
                SpellHelper.CheckReflect(this, Caster, ref m);

                Point3D loc = m.Location;
                loc.Z += 50;

                m.PlaySound(0x211);
                m.FixedParticles(0x3779, 1, 30, 0x26EC, 0x3, 0x3, EffectLayer.Waist);

                Effects.SendMovingParticles(new Entity(Serial.Zero, loc, m.Map), new Entity(Serial.Zero, m.Location, m.Map), 0xF5F, 1, 0, true, false, 0x21, 0x3F, 0x251D, 0, 0, EffectLayer.Head, 0);

                double percentage = 0.05 * FocusLevel;
                bool pvmThreshold = !m.Player && ((m.Hits / (double)m.HitsMax) < percentage);

                int damage;

                if (pvmThreshold)
                {
                    damage = 300;
                }
                else
                {
                    int minDamage = (int)Caster.Skills.Spellweaving.Value / 5;
                    int maxDamage = (int)Caster.Skills.Spellweaving.Value / 3;
                    damage = Utility.RandomMinMax(minDamage, maxDamage);
                }

                int damageBonus = SpellHelper.GetSpellDamageBonus(Caster, m, CastSkill, Caster.Player && m.Player);

                damage *= damageBonus + 100;
                damage /= 100;

                SpellHelper.Damage(this, m, damage, 0, 0, 0, 0, 0, !pvmThreshold ? 100 : 0, pvmThreshold ? 100 : 0);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly WordOfDeathSpell m_Owner;
            public InternalTarget(WordOfDeathSpell owner)
                : base(10, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile m, object o)
            {
                if (o is Mobile)
                {
                    m_Owner.Target((Mobile)o);
                }
            }

            protected override void OnTargetFinish(Mobile m)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
