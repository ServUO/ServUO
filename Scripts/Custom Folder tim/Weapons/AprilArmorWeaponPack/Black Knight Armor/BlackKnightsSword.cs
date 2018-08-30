
using System;
using Server;

namespace Server.Items
{
	public class Blackknightssword : VikingSword  
	{
		public override int ArtifactRarity{ get{ return 20; } }


		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ShadowStrike; } } 

		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		

		public override int InitMinHits{ get{ return 100; } } 
		public override int InitMaxHits{ get{ return 300; } } 

		[Constructable]
		public Blackknightssword()
		{
			Weight = 5.0;
            		Name = "Black Knights Sword";
            		Hue = 4455;

			WeaponAttributes.DurabilityBonus = 1000; 
			WeaponAttributes.HitFireArea = 10;
			WeaponAttributes.HitHarm = 10;
			WeaponAttributes.SelfRepair = 10;

			Attributes.AttackChance = 10;
			Attributes.BonusMana = 5;
			Attributes.CastSpeed = 5;
			Attributes.DefendChance = 10;
			Attributes.Luck = 10;
			Attributes.WeaponDamage = 10;
			Attributes.WeaponSpeed = 10;


			StrRequirement = 60;

		}

		public Blackknightssword( Serial serial ) : base( serial )
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