
using System;
using Server;

namespace Server.Items
{
	public class SilverCrowScythe : Scythe  
	{
		public override int ArtifactRarity{ get{ return 62; } }


		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.BleedAttack; } } 

		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }

		public override float MlSpeed{ get{ return 2.75f; } }
		

		public override int InitMinHits{ get{ return 300; } } 
		public override int InitMaxHits{ get{ return 300; } } 

		[Constructable]
		public SilverCrowScythe()
		{
			Weight = 5.0;
            		Name = "Silver Crow Scythe";
            		Hue = 1953;

			//WeaponAttributes.DurabilityBonus = 100; 
			WeaponAttributes.HitLeechHits = 60;
			//WeaponAttributes.HitHarm = 60;
			WeaponAttributes.HitLightning = 55;
			WeaponAttributes.HitLowerAttack = 45;
			//WeaponAttributes.HitLowerDefend = 45;
			//WeaponAttributes.SelfRepair = 60;

			Attributes.AttackChance = 10;
			//Attributes.BonusStr = 10;
			Attributes.CastSpeed = 1;
			Attributes.DefendChance = 10;
			//Attributes.Luck = 100;
            //Attributes.WeaponDamage = 70;
            //Attributes.WeaponSpeed = 45;


			StrRequirement = 60;

		}

		public SilverCrowScythe( Serial serial ) : base( serial )
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