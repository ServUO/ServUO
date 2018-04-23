#region AuthorHeader
//
//	Claim System version 2.1, by Xanthos
//
//
#endregion AuthorHeader
using System;
using Server;
using Server.Items;

namespace Xanthos.Claim
{
    public class GoldBag : Bag
    {		
		[Constructable]
		public GoldBag() : base()
		{
			Weight = 0.0;
			Hue = 1174;
			Name = "gold bag";
			LootType = ClaimConfig.GoldBagBlessed? LootType.Blessed : LootType.Regular;
		}

		public GoldBag( Serial serial ) : base( serial )
		{
		}
		
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Gold )
				return base.OnDragDrop( from, dropped );

			return false;
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
