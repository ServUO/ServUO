using System;
using Server.Gumps;

namespace Server.Items
{
	public class LifeGate : Item
	{
		[Constructable]
		public LifeGate() : base( 0xF6C )
		{
			Movable = false;
			Hue = 38;
			Name = "a Life gate";
			Light = LightType.Circle300;
		}

		public LifeGate( Serial serial ) : base( serial )
		{
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( !m.Alive && m.Map != null && m.Map.CanFit( m.Location, 16, false, false ) )
			{
				m.PlaySound( 0x214 );
				m.FixedEffect( 0x376A, 10, 16 );
				m.Resurrect();

				m.Hits = m.HitsMax;
				m.Mana = m.ManaMax;
				m.Stam = m.StamMax;
			}
			else
			{
				m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
			}

			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}