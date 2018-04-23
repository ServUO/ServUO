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
    public class SilverBag : Bag
    {		
		[Constructable]
		public SilverBag() : base()
		{
			Weight = 0.0;
			Hue = 1150;
			Name = "silver bag";
			LootType = ClaimConfig.SilverBagBlessed ? LootType.Blessed : LootType.Regular;
		}

		public SilverBag( Serial serial ) : base( serial )
		{
		}
		
		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is Server.Factions.Silver )
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
