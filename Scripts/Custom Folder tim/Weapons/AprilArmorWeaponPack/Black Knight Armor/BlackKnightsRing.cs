
using System;
using Server;

namespace Server.Items
{
	public class Blackknightsring : GoldRing
	{
		
		public override int ArtifactRarity{ get{ return 20; } }

		[Constructable]
		public Blackknightsring()
		{
			Weight = 1.0;
			Name = "Black Knights Ring";
			Hue = 4455;

			Attributes.AttackChance = 10;
			Attributes.BonusDex = 15;
			Attributes.CastSpeed = 15;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 10;
			Attributes.WeaponSpeed = 15;


			Resistances.Cold = 5;
			Resistances.Energy = 2;
			Resistances.Fire = 8;
			Resistances.Physical = 15;
			Resistances.Poison = 2;
			
		}

		public Blackknightsring( Serial serial ) : base( serial )
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