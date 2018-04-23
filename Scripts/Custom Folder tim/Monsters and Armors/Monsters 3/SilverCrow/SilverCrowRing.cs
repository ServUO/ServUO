
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowRing : GoldRing
	{
		
		public override int ArtifactRarity{ get{ return 62; } }

		[Constructable]
		public SilverCrowRing()
		{
			Weight = 1.0;
			Name = "Silver Crow Ring";
			Hue = 1953;

			Attributes.AttackChance = 10;
            //Attributes.BonusStr = 5;
            //Attributes.BonusDex = 5;
            //Attributes.BonusMana = 5;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 3;
			//Attributes.WeaponDamage = 100;
			//Attributes.WeaponSpeed = 50;


            //Resistances.Cold = 5;
            //Resistances.Energy = 2;
            //Resistances.Fire = 8;
            //Resistances.Physical = 15;
            //Resistances.Poison = 2;
			
		}

		public SilverCrowRing( Serial serial ) : base( serial )
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