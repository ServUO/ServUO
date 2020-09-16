using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using System;

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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.0);
        public override double RequiredSkill => 50.0;
        public override int RequiredMana => 15;
        public override bool BlockedByAnimalForm => false;
        public override bool CheckCast()
        {
            PlayerMobile pm = Caster as PlayerMobile; // IsStealthing should be moved to Server.Mobiles
            if (!pm.IsStealthing)
            {
                Caster.SendLocalizedMessage(1063087); // You must be in stealth mode to use this ability.
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
            Caster.SendLocalizedMessage(1063088); // You prepare to perform a Shadowjump.
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            IPoint3D orig = p;
            Map map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            Point3D from = Caster.Location;
            Point3D to = new Point3D(p);

            PlayerMobile pm = Caster as PlayerMobile; // IsStealthing should be moved to Server.Mobiles

            if (!pm.IsStealthing)
            {
                Caster.SendLocalizedMessage(1063087); // You must be in stealth mode to use this ability.
            }
            else if (Misc.WeightOverloading.IsOverloaded(Caster))
            {
                Caster.SendLocalizedMessage(502359, "", 0x22); // Thou art too encumbered to move.
            }
            else if (!SpellHelper.CheckTravel(Caster, TravelCheckType.TeleportFrom) || !SpellHelper.CheckTravel(Caster, map, to, TravelCheckType.TeleportTo))
            {
            }
            else if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                Caster.SendLocalizedMessage(502831); // Cannot teleport to that spot.
            }
            else if (SpellHelper.CheckMulti(to, map, true, 5))
            {
                Caster.SendLocalizedMessage(502831); // Cannot teleport to that spot.
            }
            else if (Region.Find(to, map).GetRegion(typeof(HouseRegion)) != null)
            {
                Caster.SendLocalizedMessage(502829); // Cannot teleport to that spot.
            }
            else if (CheckSequence())
            {
                SpellHelper.Turn(Caster, orig);

                Mobile m = Caster;

                m.Location = to;
                m.ProcessDelta();

                Effects.SendLocationParticles(EffectItem.Create(from, m.Map, EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);

                m.PlaySound(0x512);

                SkillHandlers.Stealth.OnUse(m); // stealth check after the a jump
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly Shadowjump m_Owner;
            public InternalTarget(Shadowjump owner)
                : base(11, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                IPoint3D p = o as IPoint3D;

                if (p != null)
                    m_Owner.Target(p);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}
