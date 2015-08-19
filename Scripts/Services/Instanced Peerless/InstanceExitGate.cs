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
using Server.Gumps;
using Server.Engines.InstancedPeerless;

namespace Server.Items
{
	public class InstanceExitGate : Item
	{
		private Map m_MapDest;
		private Point3D m_LocDest;

		public override int LabelNumber { get { return 1113495; } } // (Exit)

		public override bool ForceShowProperties { get { return true; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Map MapDest { get { return m_MapDest; } set { m_MapDest = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Point3D LocDest { get { return m_LocDest; } set { m_LocDest = value; } }

		[Constructable]
		public InstanceExitGate()
			: this( Map.Internal, Point3D.Zero )
		{
		}

		[Constructable]
		public InstanceExitGate( Map mapDest, Point3D locDest )
			: base( 0xF6C )
		{
			m_MapDest = mapDest;
			m_LocDest = locDest;

			Movable = false;
			Hue = 0x488;
			Light = LightType.Circle300;
		}

		public override bool OnMoveOver( Mobile m )
		{
			if ( !m.HasGump( typeof( ConfirmExitInstanceGump ) ) )
				m.SendGump( new ConfirmExitInstanceGump( this ) );

			return base.OnMoveOver( m );
		}

		public InstanceExitGate( Serial serial )
			: base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_MapDest );
			writer.Write( m_LocDest );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			/*int version = */
			reader.ReadInt();

			m_MapDest = reader.ReadMap();
			m_LocDest = reader.ReadPoint3D();
		}
	}
}