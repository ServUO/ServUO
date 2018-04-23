using System;
using Server;

namespace Server.Items
{
	public class Whirlwind : Spear
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public Whirlwind()
		{
			Name = "Whirlwind";
			Hue = 7;
			WeaponAttributes.HitLightning = 20;
			WeaponAttributes.HitFireball = 20;
			WeaponAttributes.HitHarm = 20;	
			Attributes.AttackChance = 15;
			Attributes.DefendChance = 15;
			Attributes.WeaponDamage = 50;
			Attributes.WeaponSpeed = 15;
		}

		public Whirlwind( Serial serial ) : base( serial )
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