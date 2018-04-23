using System;
using Server;

namespace Server.Items
{
	public class GhastlyBascinet : BaseArmor
	{
		public override int BasePhysicalResistance{ get{ return 5; } }
		public override int BaseFireResistance{ get{ return 0; } }
		public override int BaseColdResistance{ get{ return 10; } }
		public override int BasePoisonResistance{ get{ return 8; } }
		public override int BaseEnergyResistance{ get{ return 3; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override int AosStrReq{ get{ return 40; } }
		public override int OldStrReq{ get{ return 10; } }

		public override int ArmorBase{ get{ return 18; } }

		public override ArmorMaterialType MaterialType{ get{ return ArmorMaterialType.Plate; } }

		[Constructable]
		public GhastlyBascinet() : base( 0x140C )
		{
			Weight = 5.0;
			Name = "Ghastly Bascinet";
			Hue = Utility.RandomList( 1642, 2114 );
			ArmorAttributes.MageArmor = 1;
			Attributes.LowerRegCost = 10;
			Attributes.LowerManaCost = 5;
			Attributes.Luck = -40;

			if ( Hue == 2114 )
				Attributes.CastSpeed = 1;

			if ( Hue == 1642 )
				Attributes.CastRecovery = 1;
		}

		public GhastlyBascinet( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
				Weight = 5.0;
		}
	}
}