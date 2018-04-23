using System;
using Server;

namespace Server.Items
{
	public class BarbaricAxe : TwoHandedAxe
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public BarbaricAxe()
		{
			Name = "Barbaric Axe";
			Hue = 1164;
			Attributes.WeaponSpeed = 30;
			Attributes.AttackChance = 15;
			Attributes.DefendChance = 15;
			Attributes.WeaponDamage = 55;
		}

		public BarbaricAxe( Serial serial ) : base( serial )
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
		}
	}
}