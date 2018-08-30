using System;
using System.Collections.Generic;
using Server.Misc;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;

namespace Server.ACC.PG
{
	public class PublicGate : Item
	{
		[Constructable]
		public PublicGate() : base( 3948 )
		{
			Movable = false;
			Name = "Public Gate";
			Light = LightType.Circle300;
		}

		public PublicGate( Serial serial ) : base( serial )
		{
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( !from.Player )
				return;

			if ( from.InRange( GetWorldLocation(), 1 ) )
				UseGate( from );
			else
				from.SendLocalizedMessage( 500446 ); // That is too far away.
		}

		public override bool OnMoveOver( Mobile m )
		{
			return !m.Player || UseGate( m );
		}

		public bool UseGate( Mobile m )
		{
			if( !PGSystem.Running )
			{
				m.SendMessage( "The Public Gate System is not active.  Please page a GM for assistance." );
				return false;
			}

			if ( m.Criminal )
			{
				m.SendLocalizedMessage( 1005561, "", 0x22 ); // Thou'rt a criminal and cannot escape so easily.
				return false;
			}
			else if ( Server.Spells.SpellHelper.CheckCombat( m ) )
			{
				m.SendLocalizedMessage( 1005564, "", 0x22 ); // Wouldst thou flee during the heat of battle??
				return false;
			}
			else if ( m.Spell != null )
			{
				m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
				return false;
			}
			else
			{
				int page = 0;
				bool found = false;
				for( int i = 0; i < PGSystem.CategoryList.Count && !found; i++ )
				{
					PGCategory PGC = PGSystem.CategoryList[i];
					if( PGC != null && PGC.Locations != null && PGC.Locations.Count > 0 &&
					    (PGC.GetFlag( EntryFlag.StaffOnly ) && m.AccessLevel > AccessLevel.Player) ||
					    (!PGC.GetFlag( EntryFlag.StaffOnly ) && ((!PGC.GetFlag( EntryFlag.Reds ) && m.Kills < 5) || PGC.GetFlag( EntryFlag.Reds ))) ||
					    (m.AccessLevel > AccessLevel.Player) )
					{
						for( int j = 0; j < PGC.Locations.Count && !found; j++ )
						{
							PGLocation PGL = PGC.Locations[j];
							if( PGL != null && (PGL.GetFlag( EntryFlag.StaffOnly ) && m.AccessLevel > AccessLevel.Player) ||
							    (!PGL.GetFlag( EntryFlag.StaffOnly) && ((!PGL.GetFlag( EntryFlag.Reds ) && m.Kills < 5) || PGL.GetFlag( EntryFlag.Reds ))) ||
							    (m.AccessLevel > AccessLevel.Player) )
							{
								if( PGL.Location == this.Location && PGL.Map == this.Map )
								{
									page = i;
									found = true;
								}
							}
						}
					}
				}

				m.CloseGump( typeof( PGGump ) );
				m.SendGump( new PGGump( m, page, this ) );

				if ( !m.Hidden || m.AccessLevel == AccessLevel.Player )
					Effects.PlaySound( m.Location, m.Map, 0x20E );

				return true;
			}
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