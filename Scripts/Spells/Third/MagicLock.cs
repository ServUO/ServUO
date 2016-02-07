using System;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class MagicLockSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Magic Lock", "An Por",
            215,
            9001,
            Reagent.Garlic,
            Reagent.Bloodmoss,
            Reagent.SulfurousAsh);
        public MagicLockSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Third;
            }
        }
        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(LockableContainer targ)
        {
            if (Multis.BaseHouse.CheckLockedDownOrSecured(targ))
            {
                // You cannot cast this on a locked down item.
                this.Caster.LocalOverheadMessage(MessageType.Regular, 0x22, 501761);
            }
            else if (targ.Locked || targ.LockLevel == 0 || targ is ParagonChest)
            {
                // Target must be an unlocked chest.
                this.Caster.SendLocalizedMessage(501762);
            }
            else if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, targ);

                Point3D loc = targ.GetWorldLocation();

                Effects.SendLocationParticles(
                    EffectItem.Create(loc, targ.Map, EffectItem.DefaultDuration),
                    0x376A, 9, 32, 5020);

                Effects.PlaySound(loc, targ.Map, 0x1FA);

                // The chest is now locked!
                this.Caster.LocalOverheadMessage(MessageType.Regular, 0x3B2, 501763);

                targ.LockLevel = -255; // signal magic lock
                targ.Locked = true;
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MagicLockSpell m_Owner;
            public InternalTarget(MagicLockSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is LockableContainer)
                    this.m_Owner.Target((LockableContainer)o);
                else
                    from.SendLocalizedMessage(501762); // Target must be an unlocked chest.
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}