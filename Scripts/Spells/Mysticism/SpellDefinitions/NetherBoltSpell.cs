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
				double damage = GetNewAosDamage( 10, 1, 4, target );

                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 0, 100, 0);

                target.FixedParticles(0x36CB, 1, 9, 9911, 1455, 5, EffectLayer.Head);
                target.PlaySound(0x211);
			}

			FinishSequence();
		}
	}
}