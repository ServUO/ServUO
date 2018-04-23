using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections;
using System.Reflection;
using Server.Network;

namespace Server.Lokai
{
	public class LinkedGate : Moongate
	{
		private LinkedGate mateGate;
		public LinkedGate MateGate { get { return mateGate; } set { mateGate = value; } }
		
		private Point3D m_Target;
		private Map m_TargetMap;
		
		private bool showWarning;
		[CommandProperty( AccessLevel.GameMaster )]
		public bool ShowWarning { get { return showWarning; } set { showWarning = value; } }

		[Constructable]
		public LinkedGate() : base( false )
		{
			Movable = true;
			Visible = true;
			ShowWarning = false;
			Hue = 0x2D1;
			Light = LightType.Circle300;
		}

		public LinkedGate( Serial serial ) : base( serial )
		{
		}

		public override void DelayCallback( Mobile from, int range )
		{
			BeginConfirmation( from );
		}

		public override void BeginConfirmation( Mobile from )
		{
			if ( ShowWarning && ( IsInTown( from.Location, from.Map ) && !IsInTown( MateGate.Location, MateGate.Map )
					|| ( from.Map != Map.Felucca && MateGate.Map == Map.Felucca && ShowFeluccaWarning ) ) )
			{
				from.Send( new PlaySound( 0x20E, from.Location ) );
				from.CloseGump( typeof( MoongateConfirmGump ) );
				from.SendGump( new MoongateConfirmGump( from, this ) );
			}
			else
			{
				EndConfirmation( from );
			}
		}

		public override void UseGate( Mobile m )
		{
			if ( MateGate == null || MateGate.Deleted )
			{
				Console.WriteLine( "The Gate at {0} is missing it's mate", this.Location.ToString() );
				return;
			}
			m_Target = MateGate.Location;
			m_TargetMap = MateGate.Map;
			
			ClientFlags flags = m.NetState == null ? ClientFlags.None : m.NetState.Flags;

			if ( Factions.Sigil.ExistsOn( m ) )
			{
				m.SendLocalizedMessage( 1061632 ); // You can't do that while carrying the sigil.
			}
			else if ( m_TargetMap == Map.Felucca && m is PlayerMobile && ((PlayerMobile)m).Young )
			{
				m.SendLocalizedMessage( 1049543 ); // You decide against traveling to Felucca while you are still young.
			}
			else if ( (m.Kills >= 5 && m_TargetMap != Map.Felucca) || ( m_TargetMap == Map.Tokuno && (flags & ClientFlags.Tokuno) == 0 ) || ( m_TargetMap == Map.Malas && (flags & ClientFlags.Malas) == 0 ) || ( m_TargetMap == Map.Ilshenar && (flags & ClientFlags.Ilshenar) == 0 ) )
			{
				m.SendLocalizedMessage( 1019004 ); // You are not allowed to travel there.
			}
			else if ( m.Spell != null )
			{
				m.SendLocalizedMessage( 1049616 ); // You are too busy to do that at the moment.
			}
			else if ( m_TargetMap != null && m_TargetMap != Map.Internal )
			{
				BaseCreature.TeleportPets( m, m_Target, m_TargetMap );

				m.MoveToWorld( m_Target, m_TargetMap );

				m.PlaySound( 0x1FE );

				OnGateUsed( m );
			}
			else
			{
				m.SendMessage( "This moongate does not seem to go anywhere." );
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 ); // version

			writer.Write( showWarning );
			writer.Write( mateGate );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
				{
					showWarning = reader.ReadBool();
					goto case 1;
				}
				case 1:
				{
					mateGate = reader.ReadItem() as LinkedGate;
					break;
				}
			}
		}
	}
}