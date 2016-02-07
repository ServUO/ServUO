using System;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class PoisonSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Poison", "In Nox",
            203,
            9051,
            Reagent.Nightshade);

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Third;
            }
        }

        public PoisonSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
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

                if (m.Spell != null)
                    m.Spell.OnCasterHurt();

                m.Paralyzed = false;

                if (this.CheckResisted(m))
                {
                    m.SendLocalizedMessage(501783); // You feel yourself resisting magical energy.
                }
                else
                {
                    int level;

                    if (Core.AOS)
                    {
                        if (this.Caster.InRange(m, 2))
                        {
                            int total = (this.Caster.Skills.Magery.Fixed + this.Caster.Skills.Poisoning.Fixed) / 2;

                            if (total >= 1000)
                                level = 3;
                            else if (total > 850)
                                level = 2;
                            else if (total > 650)
                                level = 1;
                            else
                                level = 0;
                        }
                        else
                        {
                            level = 0;
                        }
                    }
                    else
                    {
                        //double total = Caster.Skills[SkillName.Magery].Value + Caster.Skills[SkillName.Poisoning].Value;
                        #region Dueling
                        double total = this.Caster.Skills[SkillName.Magery].Value;

                        if (this.Caster is Mobiles.PlayerMobile)
                        {
                            Mobiles.PlayerMobile pm = (Mobiles.PlayerMobile)this.Caster;

                            if (pm.DuelContext != null && pm.DuelContext.Started && !pm.DuelContext.Finished && !pm.DuelContext.Ruleset.GetOption("Skills", "Poisoning"))
                            {
                            }
                            else
                            {
                                total += this.Caster.Skills[SkillName.Poisoning].Value;
                            }
                        }
                        else
                        {
                            total += this.Caster.Skills[SkillName.Poisoning].Value;
                        }
                        #endregion

                        double dist = this.Caster.GetDistanceToSqrt(m);

                        if (dist >= 3.0)
                            total -= (dist - 3.0) * 10.0;

                        if (total >= 200.0 && 1 > Utility.Random(10))
                            level = 3;
                        else if (total > (Core.AOS ? 170.1 : 170.0))
                            level = 2;
                        else if (total > (Core.AOS ? 130.1 : 130.0))
                            level = 1;
                        else
                            level = 0;
                    }

                    m.ApplyPoison(this.Caster, Poison.GetPoison(level));
                }

                m.FixedParticles(0x374A, 10, 15, 5021, EffectLayer.Waist);
                m.PlaySound(0x205);

                this.HarmfulSpell(m);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly PoisonSpell m_Owner;

            public InternalTarget(PoisonSpell owner)
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