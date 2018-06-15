using System;
using Server;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	[Flipable]
	public class WoodenBookcase : BaseContainer
	{
		public override int LabelNumber { get { return 1071102; } } // Wooden Bookcase

		[Constructable]
		public WoodenBookcase()
			: this( true )
		{
		}

		[Constructable]
		public WoodenBookcase( bool south )
			: base( south ? 0xA9D : 0xA9E )
		{
			Weight = 1.0;
		}

		public WoodenBookcase( Serial serial )
			: base( serial )
		{
		}

		public override void AddItem( Item item )
		{
			base.AddItem( item );

			if ( Items.Count > 0 )
			{
				if ( ItemID == 0xA9D )
					ItemID = 0xA9B;
				else if ( ItemID == 0xA9E )
					ItemID = 0xA9C;
			}
		}

		public override void RemoveItem( Item item )
		{
			base.RemoveItem( item );

			if ( Items.Count == 0 )
			{
				if ( ItemID == 0xA9B )
					ItemID = 0xA9D;
				else if ( ItemID == 0xA9C )
					ItemID = 0xA9E;
			}
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

		public void Flip()
		{
			switch ( ItemID )
			{
				case 0xA9B: ItemID = 0xA9C; break;
				case 0xA9C: ItemID = 0xA9B; break;
				case 0xA9D: ItemID = 0xA9E; break;
				case 0xA9E: ItemID = 0xA9D; break;
			}
		}
	}
}