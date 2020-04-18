using Server.Mobiles;
using Server.Regions;
using Server.Targeting;
using System;

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

        public override TimeSpan CastDelayBase => TimeSpan.FromSeconds(1.5);
        public override double RequiredSkill => 0.0;
        public override int RequiredMana => 24;
        public override bool CheckCast()
        {
            if (!base.CheckCast())
                return false;

            if ((Caster.Followers + 1) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return false;
            }

            return true;
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D point)
        {
            Point3D p = new Point3D(point);
            Map map = Caster.Map;

            if (map == null)
                return;

            HouseRegion r = Region.Find(p, map).GetRegion(typeof(HouseRegion)) as HouseRegion;

            if (r != null && r.House != null && !r.House.IsFriend(Caster))
                return;

            if (!map.CanSpawnMobile(p.X, p.Y, p.Z))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                TimeSpan duration = TimeSpan.FromSeconds(Caster.Skills.Spellweaving.Value / 24 + 25 + FocusLevel * 2);

                NatureFury nf = new NatureFury();
                BaseCreature.Summon(nf, false, Caster, p, 0x5CB, duration);

                new InternalTimer(nf).Start();
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly NatureFurySpell m_Owner;
            public InternalTarget(NatureFurySpell owner)
                : base(10, true, TargetFlags.None)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is IPoint3D)
                    m_Owner.Target((IPoint3D)o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                if (m_Owner != null)
                    m_Owner.FinishSequence();
            }
        }

        private class InternalTimer : Timer
        {
            private readonly NatureFury m_NatureFury;
            public InternalTimer(NatureFury nf)
                : base(TimeSpan.FromSeconds(5.0), TimeSpan.FromSeconds(5.0))
            {
                m_NatureFury = nf;
            }

            protected override void OnTick()
            {
                if (m_NatureFury.Deleted || !m_NatureFury.Alive || m_NatureFury.DamageMin > 20)
                {
                    Stop();
                }
                else
                {
                    ++m_NatureFury.DamageMin;
                    ++m_NatureFury.DamageMax;
                }
            }
        }
    }
}