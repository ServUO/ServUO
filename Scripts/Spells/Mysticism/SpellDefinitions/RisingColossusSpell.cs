using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
	public class RisingColossusSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Rising Colossus", "Kal Vas Xen Corp Ylem",
				230,
				9022,
				Reagent.DaemonBone,
				Reagent.DragonBlood,
				Reagent.FertileDirt,
				Reagent.Nightshade
			);

		public RisingColossusSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public void Target(IPoint3D p)
        {
            if ((Caster.Followers + 5) > Caster.FollowersMax)
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
                double baseskill = Caster.Skills[SkillName.Mysticism].Value;
                double boostskill = GetBoostSkill(Caster);

                int level = (int)(baseskill + boostskill);

                TimeSpan duration = TimeSpan.FromSeconds(level / 3);

                BaseCreature summon = new RisingColossus(Caster, baseskill, boostskill);
                BaseCreature.Summon(summon, false, Caster, new Point3D(p), 0x656, duration);

                Effects.SendTargetParticles(summon, 0x3728, 10, 10, 0x13AA, (EffectLayer)255);
            }

            FinishSequence();
        }

        public class InternalTarget : Target
        {
            private RisingColossusSpell m_Owner;

            public InternalTarget(RisingColossusSpell owner)
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
