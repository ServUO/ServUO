using System;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
    public class CleanseByFireSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Cleanse By Fire", "Expor Flamus",
            -1,
            9002);
        public CleanseByFireSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 5.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 10;
            }
        }
        public override int RequiredTithing
        {
            get
            {
                return 10;
            }
        }
        public override int MantraNumber
        {
            get
            {
                return 1060718;
            }
        }// Expor Flamus

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public override bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
        {
            return true;
        }

        public void Target(Mobile m)
        {
            if (!m.Poisoned)
            {
                this.Caster.SendLocalizedMessage(1060176); // That creature is not poisoned!
            }
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                /* Cures the target of poisons, but causes the caster to be burned by fire damage for 13-55 hit points.
                * The amount of fire damage is lessened if the caster has high Karma.
                */

                Poison p = m.Poison;

                if (p != null)
                {
                    // Cleanse by fire is now difficulty based 
                    int chanceToCure = 10000 + (int)(this.Caster.Skills[SkillName.Chivalry].Value * 75) - ((p.RealLevel + 1) * 2000);
                    chanceToCure /= 100;

                    if (chanceToCure > Utility.Random(100))
                    {
                        if (m.CurePoison(this.Caster))
                        {
                            if (this.Caster != m)
                                this.Caster.SendLocalizedMessage(1010058); // You have cured the target of all poisons!

                            m.SendLocalizedMessage(1010059); // You have been cured of all poisons.
                        }
                    }
                    else
                    {
                        m.SendLocalizedMessage(1010060); // You have failed to cure your target!
                    }
                }

                m.PlaySound(0x1E0);
                m.FixedParticles(0x373A, 1, 15, 5012, 3, 2, EffectLayer.Waist);

                IEntity from = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z - 5), m.Map);
                IEntity to = new Entity(Serial.Zero, new Point3D(m.X, m.Y, m.Z + 45), m.Map);
                Effects.SendMovingParticles(from, to, 0x374B, 1, 0, false, false, 63, 2, 9501, 1, 0, EffectLayer.Head, 0x100);

                this.Caster.PlaySound(0x208);
                this.Caster.FixedParticles(0x3709, 1, 30, 9934, 0, 7, EffectLayer.Waist);

                int damage = 50 - this.ComputePowerValue(4);

                // TODO: Should caps be applied?
                if (damage < 13)
                    damage = 13;
                else if (damage > 55)
                    damage = 55;

                AOS.Damage(this.Caster, this.Caster, damage, 0, 100, 0, 0, 0, true);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly CleanseByFireSpell m_Owner;
            public InternalTarget(CleanseByFireSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
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
