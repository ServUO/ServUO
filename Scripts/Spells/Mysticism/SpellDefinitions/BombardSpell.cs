using System;
using Server.Targeting;

namespace Server.Spells.Mystic
{
	public class BombardSpell : MysticSpell
	{
        public override SpellCircle Circle { get { return SpellCircle.Sixth; } }

		private static SpellInfo m_Info = new SpellInfo(
				"Bombard", "Corp Por Ylem",
				230,
				9022,
				Reagent.Bloodmoss,
				Reagent.Garlic,
				Reagent.SulfurousAsh,
				Reagent.DragonBlood
			);

		public BombardSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

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
				Caster.MovingEffect( target, 0x11B6, 12, 1, false, false, 0, 0 );
				Caster.PlaySound( 0x64B );

				if (!CheckResisted(target))
				{
					Direction d = Caster.GetDirectionTo( target );
					Point3D point = new Point3D( target.Location );

					switch( d )
					{
						case (Direction)0x0: case (Direction)0x80: point.Y--; break; //North
						case (Direction)0x1: case (Direction)0x81: { point.X++; point.Y--; break; } //Right
						case (Direction)0x2: case (Direction)0x82: point.X++; break; //East
						case (Direction)0x3: case (Direction)0x83: { point.X++; point.Y++; break; } //Down
						case (Direction)0x4: case (Direction)0x84: point.Y++; break; //South
						case (Direction)0x5: case (Direction)0x85: { point.X--; point.Y++; break; } //Left
						case (Direction)0x6: case (Direction)0x86: point.X--; break; //West
						case (Direction)0x7: case (Direction)0x87: { point.X--; point.Y--; break; } //Up
						default: { break; }
					}

					if ( target.Map.CanFit( point, 16, false, false ) )
						target.MoveToWorld( point, target.Map );

					target.Paralyze( TimeSpan.FromSeconds( 2 ) );
				}

				SpellHelper.Damage( this, target, (int)GetNewAosDamage( 40, 1, 5, target ), 100, 0, 0, 0, 0 );
			}

			FinishSequence();
		}
	}
}