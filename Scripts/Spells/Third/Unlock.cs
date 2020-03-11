using System;
using Server.Items;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Third
{
    public interface IMageUnlockable
    {
        void OnMageUnlock(Mobile from);
    }

    public class UnlockSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Unlock Spell", "Ex Por",
            215,
            9001,
            Reagent.Bloodmoss,
            Reagent.SulfurousAsh);
        public UnlockSpell(Mobile caster, Item scroll)
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

        private class InternalTarget : Target
        {
            private readonly UnlockSpell m_Owner;
            public InternalTarget(UnlockSpell owner)
                : base(Core.ML ? 10 : 12, false, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D loc = o as IPoint3D;

                if (loc == null)
                    return;

                if (this.m_Owner.CheckSequence())
                {
                    SpellHelper.Turn(from, o);

                    Effects.SendLocationParticles(EffectItem.Create(new Point3D(loc), from.Map, EffectItem.DefaultDuration), 0x376A, 9, 32, 5024);

                    Effects.PlaySound(loc, from.Map, 0x1FF);

                    if (o is Mobile)
                        from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101); // That did not need to be unlocked.
                    else if (o is IMageUnlockable)
                        ((IMageUnlockable)o).OnMageUnlock(from);
                    else if (!(o is LockableContainer))
                        from.SendLocalizedMessage(501666); // You can't unlock that!
                    else
                    {
                        LockableContainer cont = (LockableContainer)o;

                        if (Multis.BaseHouse.CheckSecured(cont))
                            from.SendLocalizedMessage(503098); // You cannot cast this on a secure item.
                        else if (!cont.Locked)
                            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503101); // That did not need to be unlocked.
                        else if (cont.LockLevel == 0)
                            from.SendLocalizedMessage(501666); // You can't unlock that!
                        else if (cont is TreasureMapChest && ((TreasureMapChest)cont).Level > 2)
                            from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503099); // My spell does not seem to have an effect on that lock.
                        else
                        {
                            int level;
                            int reqSkill;

                            if (cont is TreasureMapChest && TreasureMapInfo.NewSystem)
                            {
                                level = (int)from.Skills[SkillName.Magery].Value;

                                switch (((TreasureMapChest)cont).Level)
                                {
                                    default:
                                    case 0: reqSkill = 50; break;
                                    case 1: reqSkill = 80; break;
                                    case 2: reqSkill = 100; break;
                                }
                            }
                            else
                            {
                                level = (int)(from.Skills[SkillName.Magery].Value * 0.8) - 4;
                                reqSkill = cont.RequiredSkill;
                            }
   
                            if (level >= reqSkill)
                            {
                                cont.Locked = false;

                                if (cont.LockLevel == -255)
                                    cont.LockLevel = cont.RequiredSkill - 10;
                            }
                            else
                                from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 503099); // My spell does not seem to have an effect on that lock.
                        }
                    }
                }

                this.m_Owner.FinishSequence();
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}
