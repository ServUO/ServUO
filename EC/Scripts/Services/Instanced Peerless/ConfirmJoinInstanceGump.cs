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
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Engines.InstancedPeerless;

namespace Server.Gumps
{
	public class ConfirmJoinInstanceGump : Gump
	{
		private PeerlessInstance m_Instance;

		public ConfirmJoinInstanceGump( PeerlessInstance instance )
			: base( 50, 50 )
		{
			m_Instance = instance;

			AddPage( 0 );

			AddBackground( 0, 0, 240, 135, 0x2422 );

			AddHtmlLocalized( 15, 15, 210, 75, 1072525, 0x0, false, false ); // <CENTER>Are you sure you want to teleport <BR>your party to an unknown area?</CENTER>

			AddButton( 160, 95, 0xF7, 0xF8, 1, GumpButtonType.Reply, 0 );
			AddButton( 90, 95, 0xF2, 0xF1, 0, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 1 && m_Instance.State == InstanceState.Reserved )
			{
				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				Effects.PlaySound( from.Location, from.Map, 0x1FE );

				BaseCreature.TeleportPets( from, m_Instance.EntranceLocation, m_Instance.Map );
				from.MoveToWorld( m_Instance.EntranceLocation, m_Instance.Map );

				Effects.SendLocationParticles( EffectItem.Create( from.Location, from.Map, EffectItem.DefaultDuration ), 0x3728, 10, 10, 2023 );
				Effects.PlaySound( from.Location, from.Map, 0x1FE );

				m_Instance.AddFighter( from );
			}
		}
	}
}