//
//  X-RunUO - Ultima Online Server Emulator
//  Copyright (C) 2015 Pedro Pardal
//
//  This library is free software; you can redistribute it and/or
//  modify it under the terms of the GNU Lesser General Public
//  License as published by the Free Software Foundation; either
//  version 3.0 of the License, or (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public
//  License along with this program.
//

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

		public override int LabelNumber { get { return 1113494; } } // (Entrance)

		public override bool ForceShowProperties { get { return true; } }

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