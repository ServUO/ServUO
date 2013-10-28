using System;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Second
{
    public class MagicTrapSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Magic Trap", "In Jux",
            212,
            9001,
            Reagent.Garlic,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh);
        public MagicTrapSpell(Mobile caster, Item scroll)
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

                item.TrapType = TrapType.MagicTrap;
                item.TrapPower = Core.AOS ? Utility.RandomMinMax(10, 50) : 1;
                item.TrapLevel = 0;

                Point3D loc = item.GetWorldLocation();

                Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc.X + 1, loc.Y, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
                Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc.X, loc.Y - 1, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
                Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc.X - 1, loc.Y, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
                Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc.X, loc.Y + 1, loc.Z), item.Map, EffectItem.DefaultDuration), 0x376A, 9, 10, 9502);
                Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc.X, loc.Y, loc.Z), item.Map, EffectItem.DefaultDuration), 0, 0, 0, 5014);

                Effects.PlaySound(loc, item.Map, 0x1EF);
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MagicTrapSpell m_Owner;
            public InternalTarget(MagicTrapSpell owner)
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
                    from.SendMessage("You can't trap that");
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}