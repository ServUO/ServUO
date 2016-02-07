using System;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;

namespace Server.Spells.Ninjitsu
{
    public class Shadowjump : NinjaSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Shadowjump", null,
            -1,
            9002);
        public Shadowjump(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.0);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 50.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 15;
            }
        }
        public override bool BlockedByAnimalForm
        {
            get
            {
                return false;
            }
        }
        public override bool CheckCast()
        {
            PlayerMobile pm = this.Caster as PlayerMobile; // IsStealthing should be moved to Server.Mobiles
            if (!pm.IsStealthing)
            {
                this.Caster.SendLocalizedMessage(1063087); // You must be in stealth mode to use this ability.
                return false;
            }

            return base.CheckCast();
        }

        public override bool CheckDisturb(DisturbType type, bool firstCircle, bool resistable)
        {
            return false;
        }

        public override void OnCast()
        {
            this.Caster.SendLocalizedMessage(1063088); // You prepare to perform a Shadowjump.
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            IPoint3D orig = p;
            Map map = this.Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            Point3D from = this.Caster.Location;
            Point3D to = new Point3D(p);

            PlayerMobile pm = this.Caster as PlayerMobile; // IsStealthing should be moved to Server.Mobiles

            if (!pm.IsStealthing)
            {
                this.Caster.SendLocalizedMessage(1063087); // You must be in stealth mode to use this ability.
            }
            else if (Factions.Sigil.ExistsOn(this.Caster))
            {
                this.Caster.SendLocalizedMessage(1061632); // You can't do that while carrying the sigil.
            }
            else if (Server.Misc.WeightOverloading.IsOverloaded(this.Caster))
            {
                this.Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!SpellHelper.CheckTravel(this.Caster, TravelCheckType.TeleportFrom) || !SpellHelper.CheckTravel(this.Caster, map, to, TravelCheckType.TeleportTo))
            {
            }
            else if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                this.Caster.SendLocalizedMessage(502831); // Cannot teleport to that spot.
            }
            else if (SpellHelper.CheckMulti(to, map, true, 5))
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

                Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

                m.PlaySound(0x512);
				
                Server.SkillHandlers.Stealth.OnUse(m); // stealth check after the a jump
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly Shadowjump m_Owner;
            public InternalTarget(Shadowjump owner)
                : base(11, true, TargetFlags.None)
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