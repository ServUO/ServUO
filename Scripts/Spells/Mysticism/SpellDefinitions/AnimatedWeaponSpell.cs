using Server.Mobiles;
using Server.Targeting;
using System;

namespace Server.Spells.Mysticism
{
    public class AnimatedWeaponSpell : MysticSpell
    {
        public override SpellCircle Circle => SpellCircle.Fourth;

        private static readonly SpellInfo m_Info = new SpellInfo(
                "Animated Weapon", "In Jux Por Ylem",
                230,
                9022,
                Reagent.BlackPearl,
                Reagent.MandrakeRoot,
                Reagent.Nightshade,
                Reagent.Bone
            );

        public AnimatedWeaponSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if ((Caster.Followers + 4) > Caster.FollowersMax)
            {
                Caster.SendLocalizedMessage(1049645); // You have too many followers to summon that creature.
                return;
            }

            Map map = Caster.Map;

            SpellHelper.GetSurfaceTop(ref p);

            if (map == null || (Caster.IsPlayer() && !map.CanSpawnMobile(p.X, p.Y, p.Z)))
            {
                Caster.SendLocalizedMessage(501942); // That location is blocked.
            }
            else if (SpellHelper.CheckTown(p, Caster) && CheckSequence())
            {
                int level = (int)((GetBaseSkill(Caster) + GetBoostSkill(Caster)) / 2.0);

                TimeSpan duration = TimeSpan.FromSeconds(10 + level);

                BaseCreature summon = new AnimatedWeapon(Caster, level);
                BaseCreature.Summon(summon, false, Caster, new Point3D(p), 0x212, duration);

                summon.PlaySound(0x64A);

                Effects.SendTargetParticles(summon, 0x3728, 10, 10, 0x13AA, (EffectLayer)255);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private readonly AnimatedWeaponSpell m_Owner;

            public InternalTarget(AnimatedWeaponSpell owner)
                : base(12, true, TargetFlags.None)
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
                m_Owner.FinishSequence();
            }
        }
    }
}