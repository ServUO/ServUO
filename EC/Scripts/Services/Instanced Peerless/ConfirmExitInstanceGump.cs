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
using Server.Network;
using Server.Engines.InstancedPeerless;

namespace Server.Gumps
{
	public class ConfirmExitInstanceGump : Gump
	{
		private InstanceExitGate m_Gate;

		public ConfirmExitInstanceGump( InstanceExitGate gate )
			: base( 150, 50 )
		{
			m_Gate = gate;

			AddPage( 0 );

			Closable = false;

			AddImage( 0, 0, 0xE10 );
			AddImageTiled( 0, 14, 15, 200, 0xE13 );
			AddImageTiled( 380, 14, 14, 200, 0xE15 );
			AddImage( 0, 201, 0xE16 );
			AddImageTiled( 15, 201, 370, 16, 0xE17 );
			AddImageTiled( 15, 0, 370, 16, 0xE11 );
			AddImage( 380, 0, 0xE12 );
			AddImage( 380, 201, 0xE18 );
			AddImageTiled( 15, 15, 365, 190, 0xA40 );

			AddRadio( 30, 140, 0x25FF, 0x2602, true, 1 );
			AddHtmlLocalized( 65, 145, 300, 25, 1113745, 0xFFFFFF, false, false ); // Yes! I must go!

			AddRadio( 30, 175, 0x25FF, 0x2602, false, 0 );
			AddHtmlLocalized( 65, 178, 300, 25, 1113746, 0xFFFFFF, false, false ); // No, I am not done yet...

			AddHtmlLocalized( 30, 20, 360, 20, 1113742, 0xFEFE40, false, false ); // Instance Exit

			AddHtmlLocalized( 30, 50, 360, 60, 1113743, 0xFFFFFF, false, false ); // You may not return to this instance while it is still in progress. If you leave, you will not be able to return to claim your corpse. Resurrect first if you wish to take your belongings.
			AddHtmlLocalized( 30, 115, 345, 40, 1113747, 0x1DB2D, false, false ); // Do you really wish to leave the instance?

			AddButton( 290, 175, 0xF7, 0xF8, 2, GumpButtonType.Reply, 0 );

			AddImageTiled( 15, 14, 365, 1, 0x2393 );
			AddImageTiled( 380, 14, 1, 190, 0x2391 );
			AddImageTiled( 15, 205, 365, 1, 0x2393 );
			AddImageTiled( 15, 14, 1, 190, 0x2391 );
			AddImageTiled( 0, 0, 395, 1, 0x23C5 );
			AddImageTiled( 394, 0, 1, 217, 0x23C3 );
			AddImageTiled( 0, 216, 395, 1, 0x23C5 );
			AddImageTiled( 0, 0, 1, 217, 0x23C3 );
		}

		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile m = sender.Mobile;

			if ( info.IsSwitched( 1 ) )
			{
				if ( m.InRange( m_Gate, 2 ) )
				{
					BaseCreature.TeleportPets( m, m_Gate.LocDest, m_Gate.MapDest );
					m.MoveToWorld( m_Gate.LocDest, m_Gate.MapDest );
				}
				else
				{
					m.SendLocalizedMessage( 500446 ); // That is too far away.
				}
			}
		}
	}
}