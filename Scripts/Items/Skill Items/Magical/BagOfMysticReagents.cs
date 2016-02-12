using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class BagOfMysticReagents : Bag
	{
		[Constructable]
		public BagOfMysticReagents()
			: this( 50 )
		{
		}

		[Constructable]
		public BagOfMysticReagents( int amount )
		{
			DropItem( new BlackPearl( amount ) );
			DropItem( new Bloodmoss( amount ) );
			DropItem( new Garlic( amount ) );
			DropItem( new Ginseng( amount ) );
			DropItem( new MandrakeRoot( amount ) );
			DropItem( new Nightshade( amount ) );
			DropItem( new SulfurousAsh( amount ) );
			DropItem( new SpidersSilk( amount ) );
			DropItem( new DaemonBone( amount ) );
			DropItem( new Bone( amount ) );
			DropItem( new FertileDirt( amount ) );
			DropItem( new DragonBlood( amount ) );
		}

		public BagOfMysticReagents( Serial serial )
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