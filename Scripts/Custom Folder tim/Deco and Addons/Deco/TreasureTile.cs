using System;
using Server;
using Server.Network;
using Server.Items;
using Server.Mobiles;

namespace Server.Items
{
	// Insignificant Change
    public class TreasureTile : BaseDecorationArtifact
    {
	public override int ArtifactRarity{ get{ return 21; } }

        [Constructable]
        public TreasureTile()
            : base(1310)
        {
            Movable = true;
	    Name = "treasure";
	    ItemID = ItemID = Utility.RandomList( 				           6975, 6976, 6977, 6978, 6979,
							     6980,       6982, 6983, 6984,       6986, 6987, 6988, 6989,
							     6990, 6991, 6992, 6993,       6995, 6996, 6997, 6998, 6999,
							     7000, 7001, 7002, 7003, 7004, 7005,       7007, 7008, 7009,
							     7010, 7011, 7012, 7013, 7014, 7015, 7016, 7017, 7018, 7019 );
        }

		public override void OnDoubleClick( Mobile from )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
			}

			else
			{
				ItemID = Utility.RandomList( 				   6975, 6976, 6977, 6978, 6979,
							     6980,       6982, 6983, 6984,       6986, 6987, 6988, 6989,
							     6990, 6991, 6992, 6993,       6995, 6996, 6997, 6998, 6999,
							     7000, 7001, 7002, 7003, 7004, 7005,       7007, 7008, 7009,
							     7010, 7011, 7012, 7013, 7014, 7015, 7016, 7017, 7018, 7019 );
			}
		}

           	public TreasureTile( Serial serial ) : base( serial )
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