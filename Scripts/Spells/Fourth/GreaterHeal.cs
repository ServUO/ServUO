using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Fourth
{
    public class GreaterHealSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Greater Heal", "In Vas Mani",
            204,
            9061,
            Reagent.Garlic,
            Reagent.Ginseng,
            Reagent.MandrakeRoot,
            Reagent.SpidersSilk);
        public GreaterHealSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Fourth;

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
            else if (m is BaseCreature && ((BaseCreature)m).IsAnimatedDead)
            {
                Caster.SendLocalizedMessage(1061654); // You cannot heal that which is not alive.
            }
            else if (m.IsDeadBondedPet)
            {
                Caster.SendLocalizedMessage(1060177); // You cannot heal a creature that is already dead!
            }
            else if (m is IRepairableMobile && ((IRepairableMobile)m).RepairResource != typeof(Items.Bandage))
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 500951); // You cannot heal that.
            }
            else if (m.Poisoned || Items.MortalStrike.IsWounded(m))
            {
                Caster.LocalOverheadMessage(MessageType.Regular, 0x22, (Caster == m) ? 1005000 : 1010398);
            }
            else if (CheckBSequence(m))
            {
                SpellHelper.Turn(Caster, m);

                // Algorithm: (40% of magery) + (1-10)

                int toHeal = (int)(Caster.Skills[SkillName.Magery].Value * 0.4);
                toHeal += Utility.Random(1, 10);

                //m.Heal( toHeal, Caster );
                SpellHelper.Heal(toHeal, m, Caster);

                m.FixedParticles(0x376A, 9, 32, 5030, EffectLayer.Waist);
                m.PlaySound(0x202);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly GreaterHealSpell m_Owner;
            public InternalTarget(GreaterHealSpell owner)
                : base(10, false, TargetFlags.Beneficial)
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
