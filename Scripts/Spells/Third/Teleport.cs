using System;
using Server.Items;
using Server.Regions;
using Server.Targeting;

namespace Server.Spells.Third
{
    public class TeleportSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Teleport", "Rel Por",
            215,
            9031,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot);
        public TeleportSpell(Mobile caster, Item scroll)
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
        public override bool CheckCast()
        {
            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
                return false;
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
                return false;
            }

            return SpellHelper.CheckTravel(this.Caster, TravelCheckType.TeleportFrom);
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            IPoint3D orig = p;
            Map map = this.Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            Point3D from = this.Caster.Location;
            Point3D to = new Point3D(p);

            if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.TeleportFrom))
            {
            }
            else if (!SpellHelper.CheckTravel(this.Caster, map, to, TravelCheckType.TeleportTo))
            {
            }
            else if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckMulti(to, map))
            {
                this.Caster.SendLocalizedMessage(502831); // Cannot teleport to that spot.
            }
            else if (Region.Find(to, map).GetRegion(typeof(HouseRegion)) != null)
            {
                this.Caster.SendLocalizedMessage(502829); // Cannot teleport to that spot.
            }
            else if (this.CheckSequence())
            {
                SpellHelper.Turn(this.Caster, orig);

                Mobile m = this.Caster;

                m.Location = to;
                m.ProcessDelta();

                if (m.Player)
                {
                    Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
                    Effects.SendLocationParticles(EffectItem.Create(to, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 5023);
                }
                else
                {
                    m.FixedParticles(0x376A, 9, 32, 0x13AF, EffectLayer.Waist);
                }

                m.PlaySound(0x1FE);

                IPooledEnumerable eable = m.GetItemsInRange(0);

                foreach (Item item in eable)
                {
                    if (item is Server.Spells.Fifth.PoisonFieldSpell.InternalItem || item is Server.Spells.Fourth.FireFieldSpell.FireFieldItem)
                        item.OnMoveOver(m);
                }

                eable.Free();
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly TeleportSpell m_Owner;
            public InternalTarget(TeleportSpell owner)
                : base(Core.ML ? 11 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    this.m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                this.m_Owner.FinishSequence();
            }
        }
    }
}