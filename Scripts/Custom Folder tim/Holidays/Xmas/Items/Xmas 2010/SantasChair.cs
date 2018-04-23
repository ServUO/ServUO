/*
 * Created by SharpDevelop.
 * User: Sharon
 * Date: 12/4/2007
 * Time: 7:21 AM
 * http://www.shazzyshard.org
 * Santa Claus Quest 2007
 */
 
using System;
using Server;

namespace Server.Items
{
	public class SantasChairAddon : BaseAddon
	{
		public override BaseAddonDeed Deed{ get{ return new SantasChairAddonDeed(); } }
				
		[Constructable]
		public SantasChairAddon()
		{
			AddonComponent ac;
			
			ac = new AddonComponent( 0x1526 );
			ac.Name = "Santa's Chair";
			ac.Hue = 32;
			AddComponent( ac, 0, 1, 0 );
			ac = new AddonComponent( 0x1527 );
			ac.Name = "Santa's Chair";
			ac.Hue = 32;
			AddComponent( ac, 0, 0, 0 );
			//AddComponent( new AddonComponent( 0x1526 ) ,  0, 1,  0  );
			//AddComponent( new AddonComponent( 0x1527 ) ,  0,  0,  0 );
			
			
		}

		public SantasChairAddon( Serial serial ) : base( serial )
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

	public class SantasChairAddonDeed : BaseAddonDeed
	{
		public override BaseAddon Addon{ get{ return new SantasChairAddon(); } }
	

		[Constructable]
		public SantasChairAddonDeed()
		{
			Name = "Santa's Chair Deed ";
			LootType = LootType.Blessed;
			Hue = 32;
		}

		public SantasChairAddonDeed( Serial serial ) : base( serial )
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
