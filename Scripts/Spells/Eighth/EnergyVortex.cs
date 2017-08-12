using System;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Eighth
{
    public class EnergyVortexSpell : MagerySpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Energy Vortex", "Vas Corp Por",
            260,
            9032,
            false,
            Reagent.Bloodmoss,
            Reagent.BlackPearl,
            Reagent.MandrakeRoot,
            Reagent.Nightshade);
        public EnergyVortexSpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override SpellCircle Circle
        {
            get
            {
                return SpellCircle.Eighth;
            }
        }
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((this.Caster.Followers + (Core.SE ? 2 : 1)) > this.Caster.FollowersMax)
            {
                this.Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            this.Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            Map map = this.Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (map == null || !map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckTown(p, this.Caster) && this.CheckSequence())
            {
                TimeSpan duration;

                if (Core.AOS)
                    duration = TimeSpan.FromSeconds(90.0);
                else
                    duration = TimeSpan.FromSeconds(Utility.Random(80, 40));

                BaseCreature.Summon(new EnergyVortex(true), false, this.Caster, new Point3D(p), 0x212, duration);
            }

            this.FinishSequence();
        }

        public class InternalTarget : Target
        {
            private EnergyVortexSpell m_Owner;
            public InternalTarget(EnergyVortexSpell owner)
                : base(Core.ML ? 10 : 12, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    this.m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetOutOfLOS(Mobile from, object o)
            {
                from.SendLocalizedMessage(501943); // Target cannot be seen. Try again.
                from.Target = new InternalTarget(this.m_Owner);
                from.Target.BeginTimeout(from, this.TimeoutTime - DateTime.UtcNow);
                this.m_Owner = null;
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (this.m_Owner != null)
                    this.m_Owner.FinishSequence();
            }
        }
    }
}