using System;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;

namespace Server.Spells.Spellweaving
{
    public class NatureFurySpell : ArcanistSpell
    {
        private static readonly SpellInfo m_Info = new SpellInfo(
            "Nature's Fury", "Rauvvrae",
            -1,
            false);
        public NatureFurySpell(Mobile caster, Item scroll)
            : base(caster, scroll, m_Info)
        {
        }

        public override TimeSpan CastDelayBase
        {
            get
            {
                return TimeSpan.FromSeconds(1.5);
            }
        }
        public override double RequiredSkill
        {
            get
            {
                return 0.0;
            }
        }
        public override int RequiredMana
        {
            get
            {
                return 24;
            }
        }
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((this.Caster.Followers + 1) > this.Caster.FollowersMax)
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

        public void Target(IPoint3D point)
        {
            Point3D p = new Point3D(point);
            Map map = this.Caster.Map;

            if (map == null)
                return;

            HouseRegion r = Region.Find(p, map).GetRegion(typeof(HouseRegion)) as HouseRegion;

            if (r != null && r.House != null && !r.House.IsFriend(this.Caster))
                return;

            if (!map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                this.Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckTown(p, this.Caster) && this.CheckSequence())
            {
                TimeSpan duration = TimeSpan.FromSeconds(this.Caster.Skills.Spellweaving.Value / 24 + 25 + this.FocusLevel * 2);

                NatureFury nf = new NatureFury();
                BaseCreature.Summon(nf, false, this.Caster, p, 0x5CB, duration);

                new InternalTimer(nf).Start();
            }

            this.FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly NatureFurySpell m_Owner;
            public InternalTarget(NatureFurySpell owner)
                : base(10, true, TargetFlags.None)
            {
                this.m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    this.m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (this.m_Owner != null)
                    this.m_Owner.FinishSequence();
            }
        }

        private class InternalTimer : Timer
        {
            private readonly NatureFury m_NatureFury;
            public InternalTimer(NatureFury nf)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                this.m_NatureFury = nf;
            }

            protected override void OnTick()
            {
                if (this.m_NatureFury.Deleted || !this.m_NatureFury.Alive || this.m_NatureFury.DamageMin > 20)
                {
                    this.Stop();
                }
                else
                {
                    ++this.m_NatureFury.DamageMin;
                    ++this.m_NatureFury.DamageMax;
                }
            }
        }
    }
}