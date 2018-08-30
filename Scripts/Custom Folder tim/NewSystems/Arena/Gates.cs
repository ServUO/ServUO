using System;

namespace Server.Items
{
	public class ArenaGateEW : Item
	{
		[Constructable]
		public ArenaGateEW() : this( 1 )
		{
		}

		[Constructable]
		public ArenaGateEW( int amount ) : base( 0x6F6 )
		{
			Stackable = false;
			Weight = 1.0;
	
		}

		public ArenaGateEW( Serial serial ) : base( serial )
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

	public class ArenaGateNS : Item
	{
		[Constructable]
		public ArenaGateNS() : this( 1 )
		{
		}

		[Constructable]
		public ArenaGateNS( int amount ) : base( 0x6F5 )
		{
			Stackable = false;
			Weight = 1.0;
	
		}

		public ArenaGateNS( Serial serial ) : base( serial )
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