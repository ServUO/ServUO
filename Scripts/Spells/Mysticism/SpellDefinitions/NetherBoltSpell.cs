using System;
using Server.Targeting;

namespace Server.Spells.Mysticism
{
	public class NetherBoltSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.First; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Nether Bolt", "In Corp Ylem",
				230,
				9022,
				Reagent.BlackPearl,
				Reagent.SulfurousAsh
			);

		public NetherBoltSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage{ get{ return true; } }
        public override bool DelayedDamageStacking { get { return false; } }
        public override Type[] DelayDamageFamily { get { return new Type[] { typeof(Server.Spells.First.MagicArrowSpell) }; } }

		public override void OnCast()
		{
			Caster.Target = new MysticSpellTarget( this, TargetFlags.Harmful );
		}

		public override void OnTarget( object o )
		{
			Mobile target = o as Mobile;

			if ( target == null )
			{
				return;
			}
			else if ( CheckHSequence( target ) )
			{
                SpellHelper.CheckReflect((int)Circle, Caster, ref target);

                double damage = GetNewAosDamage(10, 1, 4, target);

                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 0, 100, 0);

                Caster.MovingParticles(target, 0x36D4, 7, 0, false, true, 0x49A, 0, 0, 9502, 4019, 0x160);
                target.PlaySound(0x211);
			}

			FinishSequence();
		}
	}
}