using System;
using Server;
using Server.Network;
using System.Collections;
using Server.ContextMenus;
using Server.Gumps;
using Server.Misc;
using Server.Accounting;
using System.Collections.Generic;

namespace Server.Items
{
	public class Santa2013Statuette : BaseStatuette
	{
        [Constructable]
        public Santa2013Statuette( int itemID ) : base( itemID )
        {
            ItemID = 0x4A98;
		{
			LootType = LootType.Blessed;
			Weight = 10.0;			
		}
}

		public Santa2013Statuette( Serial serial ) : base( serial )
		{
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			if ( TurnedOn && IsLockedDown && (!m.Hidden || m.AccessLevel == AccessLevel.Player) && Utility.InRange( m.Location, this.Location, 2 ) && !Utility.InRange( oldLocation, this.Location, 2 ) )
			{
				int cliloc = Utility.RandomMinMax( 1007149, 1007165 );

				PublicOverheadMessage( MessageType.Regular, 0x3B2, cliloc );
				Effects.PlaySound( Location, Map, 0x669);//HO HO HO!
			}
				
			base.OnMovement( m, oldLocation );
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
	
	[FlipableAttribute( 0x4A98, 0x4A99 )]
	public class  SantaStatuette2013 : Santa2013Statuette
	{
		public override int LabelNumber{ get{ return 1097968; } } // santa statue
	
		[Constructable]
		public  SantaStatuette2013() : base( 0x4A98 )
		{
		}

        public SantaStatuette2013(Serial serial)
            : base(serial)
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
			
			int version = reader.ReadInt();
		}
	}
}
	
	

