using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Gumps;

namespace Server.Engines.InstancedPeerless
{
	public class InstanceEnterGate : Item
	{
		private PeerlessInstance m_Instance;
		private List<Mobile> m_AllowedPlayers;

		public override int LabelNumber => 1113494;  // (Entrance)

		public override bool ForceShowProperties => true; 

		public InstanceEnterGate( PeerlessInstance instance, List<Mobile> allowedPlayers )
			: base( 0xF6C )
		{
			m_Instance = instance;
			m_AllowedPlayers = allowedPlayers;

			Movable = false;
			Hue = 0x484;
			Light = LightType.Circle300;

			Timer.DelayCall( TimeSpan.FromMinutes( 1.0 ), new TimerCallback( Delete ) );
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( !m_AllowedPlayers.Contains( m ) )
			{
				m.SendLocalizedMessage( 1113573 ); // This instance has been reserved for another party.
			}
			else
			{
				if ( !m.HasGump( typeof( ConfirmJoinInstanceGump ) ) )
					m.SendGump( new ConfirmJoinInstanceGump( m_Instance ) );
			}

			return base.OnMoveOver( m );
		}

		public InstanceEnterGate( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();

			Delete();
		}
	}
}