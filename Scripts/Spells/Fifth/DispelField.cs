using Server.Items;
using Server.Misc;
using Server.Targeting;

namespace Server.Spells.Fifth
{
    public class DispelFieldSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Dispel Field", "An Grav",
            206,
            9002,
            Reagent.BlackPearl,
            Reagent.SpidersSilk,
            Reagent.SulfurousAsh,
            Reagent.Garlic);
        public DispelFieldSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle => SpellCircle.Fifth;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IEntity e)
        {
            if (Caster.CanSee(e) && CheckSequence())
            {
                SpellHelper.Turn(Caster, e);

                Effects.SendLocationParticles(EffectItem.Create(e.Location, e.Map, EffectItem.DefaultDuration), 0x376A, 9, 20, 5042);
                Effects.PlaySound(e.Location, e.Map, 0x201);

                if (e is Item item && (e.GetType().IsDefined(typeof(DispellableFieldAttribute), false) || (item is Moongate && !((Moongate)item).Dispellable)))
                {
                    item.Delete();
                }
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly DispelFieldSpell m_Owner;
            public InternalTarget(DispelFieldSpell owner)
                : base(10, false, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IEntity)
                {
                    m_Owner.Target((IEntity)o);
                }
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
