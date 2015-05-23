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

namespace Server.Engines.InstancedPeerless
{
	public class MedusaPlatform : PeerlessPlatform
	{
		public override Type KeyType { get { return typeof( RareSerpentEgg ); } }
		public override Type BossType { get { return typeof( Medusa ); } }

		[Constructable]
		public MedusaPlatform()
		{
			ExitLocation = new Point3D( 817, 756, 50 );

			// base
			AddComponent( new AddonComponent( 0x0709 ), -1, -1, 0 );
			AddComponent( new AddonComponent( 0x0709 ), 0, -1, 0 );
			AddComponent( new AddonComponent( 0x0709 ), 1, -1, 0 );
			AddComponent( new AddonComponent( 0x0709 ), 0, 0, 0 );
			AddComponent( new AddonComponent( 0x0709 ), 1, 0, 0 );
			AddComponent( new AddonComponent( 0x0709 ), -1, 1, 0 );
			AddComponent( new AddonComponent( 0x0709 ), 0, 1, 0 );
			AddComponent( new AddonComponent( 0x0709 ), 1, 1, 0 );

			// stairs
			AddComponent( new AddonComponent( 0x070D ), -1, 0, 0 );

			// blockers
			AddComponent( new AddonComponent( 0x21A4 ), 0, -1, 5 );
			AddComponent( new AddonComponent( 0x21A4 ), 1, 0, 5 );
			AddComponent( new AddonComponent( 0x21A4 ), 0, 1, 5 );

			// floor cracks
			AddComponent( new AddonComponent( 0x1B07 ), 0, 0, 5 );
			AddComponent( new AddonComponent( 0x1B05 ), 0, 0, 5 );

			// floor
			AddComponent( new AddonComponent( 0x4332 ), 1, 1, 5 );
			AddComponent( new AddonComponent( 0x4333 ), -1, -1, 5 );
			AddComponent( new AddonComponent( 0x4334 ), -1, 1, 5 );
			AddComponent( new AddonComponent( 0x4335 ), 1, -1, 5 );
			AddComponent( new AddonComponent( 0x433A ), 0, -1, 5 );
			AddComponent( new AddonComponent( 0x433A ), 1, 0, 5 );
			AddComponent( new AddonComponent( 0x433A ), 0, 1, 5 );
		}

		public override void AddInstances()
		{
			AddInstance( 0, -1, 7, Map.TerMur, new Point3D( 776, 928, -5 ), new Point3D( 818, 927, -15 ), new Rectangle2D( 760, 887, 95, 81 ) );
			AddInstance( 1, 0, 7, Map.TerMur, new Point3D( 776, 1094, -5 ), new Point3D( 818, 1093, -15 ), new Rectangle2D( 760, 1057, 95, 81 ) );
			AddInstance( 0, 1, 7, Map.TerMur, new Point3D( 776, 1264, -5 ), new Point3D( 818, 1263, -15 ), new Rectangle2D( 760, 1230, 95, 81 ) );
		}

		public override void AddBraziers()
		{
			AddBrazier( -1, -1, 7 );
			AddBrazier( 1, -1, 8 );
			AddBrazier( -1, 1, 8 );
			AddBrazier( 1, 1, 9 );
		}

		public MedusaPlatform( Serial serial )
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
		}
	}
}