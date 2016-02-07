using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.First
{
    public class HealSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Heal", "In Mani",
            224,
            9061,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.SpidersSilk);
        public HealSpell(Mobile caster, Item scroll)
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
        public override bool CheckCast()
        {
            if (Engines.ConPVP.DuelContext.CheckSuddenDeath(this.Caster))
            {
                this.Caster.SendMessage(0x22, "You cannot cast this spell when in sudden death.");
                return false;
            }

            return base.CheckCast();
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
            else if (m.IsDeadBondedPet)
            {
                this.Caster.SendLocalizedMessage(1060177); // You cannot heal a creature that is already dead!
            }
            else if (m is BaseCreature && ((BaseCreature)m).IsAnimatedDead)
            {
                this.Caster.SendLocalizedMessage(1061654); // You cannot heal that which is not alive.
            }
            else if (m is Golem)
            {
                this.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500951); // You cannot heal that.
            }
            else if (m.Poisoned || Server.Items.MortalStrike.IsWounded(m))
            {
                this.Caster.LocalOverheadMessage(MessageType.Regular, 0x22, (this.Caster == m) ? 1005000 : 1010398);
            }
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                int toHeal;

                if (Core.AOS)
                {
                    toHeal = this.Caster.Skills.Magery.Fixed / 120;
                    toHeal += Utility.RandomMinMax(1, 4);

                    if (Core.SE && this.Caster != m)
                        toHeal = (int)(toHeal * 1.5);
                }
                else
                {
                    toHeal = (int)(this.Caster.Skills[SkillName.Magery].Value * 0.1);
                    toHeal += Utility.Random(1, 5);
                }

                //m.Heal( toHeal, Caster );
                SpellHelper.Heal(toHeal, m, this.Caster);

                m.FixedParticles(0x376A, 9, 32, 5005, EffectLayer.Waist);
                m.PlaySound(0x1F2);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly HealSpell m_Owner;
            public InternalTarget(HealSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.Beneficial)
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