using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System;
using System.Linq;

namespace Server.Spells.Seventh
{
    public class MeteorSwarmSpell : MagerySpell
    {
        public override DamageType SpellDamageType => DamageType.SpellAOE;
        public Item Item { get; set; }

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Meteor Swarm", "Flam Kal Des Ylem",
            233,
            9042,
            false,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot,
            Reagent.SulfurousAsh,
            Reagent.SpidersSilk);

        public MeteorSwarmSpell(Mobile caster, Item scroll, Item item)
            : base(caster, scroll, m_Info)
        {
            Item = item;
        }

        public MeteorSwarmSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override int GetMana()
        {
            if (Item != null)
                return 0;

            return base.GetMana();
        }

        public override SpellCircle Circle => SpellCircle.Seventh;
        public override bool DelayedDamage => true;
        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this, Item);
        }

        public void Target(IPoint3D p, Item item)
        {
            if (!Caster.CanSee(p))
            {
                Caster.SendLocalizedMessage(500237); // Target can not be seen.
            }
            else if (SpellHelper.CheckTown(p, Caster) && (item != null || CheckSequence()))
            {
                if (item != null)
                {
                    if (item is MaskOfKhalAnkur)
                    {
                        ((MaskOfKhalAnkur)item).Charges--;
                    }

                    if (item is PendantOfKhalAnkur)
                    {
                        ((PendantOfKhalAnkur)item).Charges--;
                    }
                }

                SpellHelper.Turn(Caster, p);

                if (p is Item)
                    p = ((Item)p).GetWorldLocation();

                System.Collections.Generic.List<IDamageable> targets = AcquireIndirectTargets(p, 2).ToList();
                int count = Math.Max(1, targets.Count);

                if (count > 0)
                {
                    Effects.PlaySound(p, Caster.Map, 0x160);
                }

                foreach (IDamageable id in targets)
                {
                    Mobile m = id as Mobile;
                    double damage = GetNewAosDamage(51, 1, 5, id is PlayerMobile, id);

                    if (count > 2)
                        damage = (damage * 2) / count;

                    IDamageable source = Caster;
                    IDamageable target = id;

                    if (SpellHelper.CheckReflect(this, ref source, ref target))
                    {
                        Timer.DelayCall(TimeSpan.FromSeconds(.5), () =>
                        {
                            source.MovingParticles(target, item != null ? 0xA1ED : 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
                        });
                    }

                    if (m != null)
                    {
                        damage *= GetDamageScalar(m);
                    }

                    Caster.DoHarmful(id);
                    SpellHelper.Damage(this, target, damage, 0, 100, 0, 0, 0);

                    Caster.MovingParticles(id, item != null ? 0xA1ED : 0x36D4, 7, 0, false, true, 9501, 1, 0, 0x100);
                }

                ColUtility.Free(targets);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly MeteorSwarmSpell m_Owner;
            private readonly Item m_Item;

            public InternalTarget(MeteorSwarmSpell owner, Item item)
                : base(10, true, TargetFlags.None)
            {
                m_Owner = owner;
                m_Item = item;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p, m_Item);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
