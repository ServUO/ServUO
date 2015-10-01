using System;
using Server.Targeting;

namespace Server.Spells.Mystic
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

		public override void OnCast()
		{
			Caster.Target = new MysticSpellTarget( this, TargetFlags.Harmful );
		}

		public override void OnTarget( Object o )
		{
			Mobile target = o as Mobile;

			if ( target == null )
			{
				return;
			}
			else if ( CheckHSequence( target ) )
			{
				double damage = GetNewAosDamage( 10, 1, 4, target );
				int hue = 0;

                SpellHelper.Damage(this, target, damage, 0, 0, 0, 0, 0, 100, 0);

				Effects.SendBoltEffect( target, false, hue );
				Caster.PlaySound( 0x653 );
			}

			FinishSequence();
		}
	}
}