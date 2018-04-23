using System;
using Server;

namespace Server.Items
{
	public class ShieldofCoffinwood : BaseShield
	{
		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get { return 0; } }
		public override int BaseColdResistance{ get{ return 15; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 5; } }

		public override int InitMinHits{ get{ return 150; } }
		public override int InitMaxHits{ get{ return 150; } }

		public override int AosStrReq{ get{ return 20; } }

		public override int ArmorBase{ get{ return 8; } }

		[Constructable]
		public ShieldofCoffinwood() : base( 0x1B7A )
		{
			Weight = 5.0;
			Name = "Shield of Coffinwood";
			Hue = 1127;
			Attributes.BonusStam = -5;
			Attributes.DefendChance = -15;
		}

		public ShieldofCoffinwood( Serial serial ) : base(serial)
		{
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int)0 );//version
		}
	}
}
