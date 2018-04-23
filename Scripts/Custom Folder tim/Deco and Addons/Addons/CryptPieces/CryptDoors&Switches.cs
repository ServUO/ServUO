using System;

namespace Server.Items
{
	public class SecretCryptDoorNS : BaseDoor
	{
		public override bool UseChainedFunctionality{ get{ return true; } }

		[Constructable]
		public SecretCryptDoorNS() : base( 0xC8, 1309, 0xED, 0xF4, new Point3D( 0, 0, 0 ) )
		{
		}

		public SecretCryptDoorNS( Serial serial ) : base( serial )
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

	public class SecretCryptDoorEW : BaseDoor
	{
		public override bool UseChainedFunctionality{ get{ return true; } }

		[Constructable]
		public SecretCryptDoorEW() : base( 0xC9, 1309, 0xED, 0xF4, new Point3D( 0, 0, 0 ) )
		{
		}

		public SecretCryptDoorEW( Serial serial ) : base( serial )
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
	public class CryptLeverEW : BaseDoor
	{
		public override bool UseChainedFunctionality{ get{ return true; } }

		[Constructable]
		public CryptLeverEW() : base( 0x108D, 0x108E, 0x4B, 0x4B, new Point3D( 0, 0, 0 ) )
		{
		}

		public CryptLeverEW( Serial serial ) : base( serial )
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

	public class CryptLeverNS : BaseDoor
	{
		public override bool UseChainedFunctionality{ get{ return true; } }

		[Constructable]
		public CryptLeverNS() : base( 0x1094, 0x1095, 0x4B, 0x4B, new Point3D( 0, 0, 0 ) )
		{
		}

		public CryptLeverNS( Serial serial ) : base( serial )
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