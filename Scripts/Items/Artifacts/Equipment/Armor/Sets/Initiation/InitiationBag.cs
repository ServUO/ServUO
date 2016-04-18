using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class InitiationSuitBag : Bag
	{
		public override bool IsArtifact { get { return true; } }
		public override string DefaultName
		{
			get { return "Initiation Suit Bag"; }
		}

		[Constructable]
		public InitiationSuitBag() : this( 1 )
		{
			Movable = true;
			Hue = 0x30;
		}

		[Constructable]
		public InitiationSuitBag( int amount )
		{
			DropItem( new InitiationArms() );
			DropItem( new InitiationCap() );
			DropItem( new InitiationChest() );
			DropItem( new InitiationGloves() );
			DropItem( new InitiationGorget() );
			DropItem( new InitiationLegs() );
		}
		
		public InitiationSuitBag( Serial serial ) : base( serial )
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