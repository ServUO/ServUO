using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
    public class CloseWoundsSpell : PaladinSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Close Wounds", "Obsu Vulni",
            -1,
            9002);
        public CloseWoundsSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 0.0;
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
                return 1060719;
            }
        }// Obsu Vulni
        
        public override bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
        {
            return true;
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(Mobile m)
        {
            if (!this.Caster.InRange(m, 2))
            {
                this.Caster.SendLocalizedMessage(1060178); // You are too far away to perform that action!
            }
            else if (m is BaseCreature && ((BaseCreature)m).IsAnimatedDead)
            {
                this.Caster.SendLocalizedMessage(1061654); // You cannot heal that which is not alive.
            }
            else if (m.IsDeadBondedPet)
            {
                this.Caster.SendLocalizedMessage(1060177); // You cannot heal a creature that is already dead!
            }
            else if (m.Hits >= m.HitsMax)
            {
                this.Caster.SendLocalizedMessage(500955); // That being is not damaged!
            }
            else if (m.Poisoned || Server.Items.MortalStrike.IsWounded(m))
            {
                this.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, (this.Caster == m) ? 1005000 : 1010398);
            }
            else if (this.CheckBSequence(m))
            {
                SpellHelper.Turn(this.Caster, m);

                /* Heals the target for 7 to 39 points of damage.
                * The caster's Karma affects the amount of damage healed.
                */

                int toHeal = this.ComputePowerValue(6) + Utility.RandomMinMax(0, 2);

                // TODO: Should caps be applied?
                if (toHeal < 7)
                    toHeal = 7;
                else if (toHeal > 39)
                    toHeal = 39;

                if ((m.Hits + toHeal) > m.HitsMax)
                    toHeal = m.HitsMax - m.Hits;

                //m.Hits += toHeal;	//Was previosuly due to the message
                //m.Heal( toHeal, Caster, false );
                SpellHelper.Heal(toHeal, m, this.Caster, false);

                m.SendLocalizedMessage(1060203, toHeal.ToString()); // You have had ~1_HEALED_AMOUNT~ hit points of damage healed.

                m.PlaySound(0x202);
                m.FixedParticles(0x376A, 1, 62, 9923, 3, 3, EffectLayer.Waist);
                m.FixedParticles(0x3779, 1, 46, 9502, 5, 3, EffectLayer.Waist);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly CloseWoundsSpell m_Owner;
            public InternalTarget(CloseWoundsSpell owner)
                : base(12, false, TargetFlags.Beneficial)
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
