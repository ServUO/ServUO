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
using Server.Mobiles;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Engines.InstancedPeerless;

namespace Server.Gumps
{
	public class RejoinInstanceGump : Gump
	{
		private PeerlessInstance m_Instance;

		public RejoinInstanceGump( PeerlessInstance instance, int titleCliloc, int msgCliloc )
			: base( 340, 340 )
		{
			/* Not sure if the gump structure is the same as OSI, but this is better than nothing */

			m_Instance = instance;

			AddPage( 0 );

			AddBackground( 0, 0, 291, 99, 0x13BE );
			AddImageTiled( 5, 6, 280, 20, 0xA40 );

			AddHtmlLocalized( 9, 8, 280, 20, titleCliloc, 0x7FFF, false, false );

			AddImageTiled( 5, 31, 280, 40, 0xA40 );

			AddHtmlLocalized( 9, 35, 272, 40, msgCliloc, 0x7FFF, false, false );

			AddButton( 215, 73, 0xFB7, 0xFB8, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 250, 75, 65, 20, 1006044, 0x7FFF, false, false ); // OK

			AddButton( 5, 73, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 40, 75, 100, 20, 1060051, 0x7FFF, false, false ); // CANCEL
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			if ( info.ButtonID == 1 )
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