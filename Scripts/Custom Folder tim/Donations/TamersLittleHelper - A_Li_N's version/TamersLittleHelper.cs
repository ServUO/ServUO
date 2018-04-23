//Created by Ashlar, beloved of Morrigan
using System;
using Server;
using Server.Gumps;
using Server.Network;

namespace Server.Items
{
	   public class TamersLittleHelper : Item
	   {
			[Constructable]
			public TamersLittleHelper() : base( 0x139A )
			{
				Hue = 2125;
				Name = "Tamers Little Helper";
				Movable = true;
         		LootType = LootType.Blessed;
         		Weight = 0.1;
			}
			public TamersLittleHelper( Serial serial ) : base( serial )
			{
			}
			public override void OnDoubleClick( Mobile from )
			{
				from.SendGump( new TamersLittleHelperGump( from, this ) );
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

