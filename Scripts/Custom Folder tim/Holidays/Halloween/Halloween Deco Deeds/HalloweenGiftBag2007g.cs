//=========================Created By Beast==================================//
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class HalloweenGiftBag2007 : Container
	{
	
	    public override int DefaultGumpID{ get{ return 0x42; } }
		public override int DefaultDropSound{ get{ return 0x42; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 105, 150, 180 ); }
		}
		
		private static void PlaceItemIn( Container parent, int x, int y, Item item )
		{
			parent.AddItem( item );
			item.Location = new Point3D( x, y, 0 );
		}
		
		[Constructable]
        public HalloweenGiftBag2007(): base(3701)
		{
			Name = "Halloween Gift Bag 2007";
            LootType = LootType.Blessed;
			Hue = 0;
            PlaceItemIn(this, 44, 79, new SkullDeed());
            PlaceItemIn(this, 75, 76, new SpiderWebDeed());
            PlaceItemIn(this, 103, 80, new MummyDeed());
            PlaceItemIn(this, 44, 182, new WeirdDecoDeed());
            PlaceItemIn(this, 73, 190, new GraveYardDeed());
            PlaceItemIn(this, 100, 191, new CreepyCritterDeed());
		}

        public HalloweenGiftBag2007(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); 
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}