using System;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Second
{
    public class RemoveTrapSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Remove Trap", "An Jux",
            212,
            9001,
            Reagent.Bloodmoss,
            Reagent.SulfurousAsh);
        public RemoveTrapSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Second;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
            this.Caster.SendMessage("What do you wish to untrap?");
        }

        public void Target(TrapableContainer item)
        {
            if (!this.Caster.CanSee(item))
            {
                this.Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (item.TrapType != TrapType.None && item.TrapType != TrapType.MagicTrap)
            {
                base.DoFizzle();
            }
            else if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, item);

                Point3D loc = item.GetWorldLocation();

                Effects.SendLocationParticles(EffectItem.Create(loc, item.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5015);
                Effects.PlaySound(loc, item.Map, 0x1F0);

                item.TrapType = TrapType.None;
                item.TrapPower = 0;
                item.TrapLevel = 0;
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly RemoveTrapSpell m_Owner;
            public InternalTarget(RemoveTrapSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is TrapableContainer)
                {
                    this.m_Owner.Target((TrapableContainer)o);
                }
                else
                {
                    from.SendMessage("You can't disarm that");
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}