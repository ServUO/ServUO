// Scripted by Grim. Ask me before you edit my scripts please.
using System;
using Server;

namespace Server.Items
{
	public class HandsOfJupiter : DragonGloves
	{
		public override int ArtifactRarity{ get{ return 9016; } }

		public override int InitMinHits{ get{ return 300; } }
		public override int InitMaxHits{ get{ return 300; } }

		[Constructable]
		public HandsOfJupiter()
		{
			Weight = 0.0; 
            		Name = "Hands Of Jupiter"; 
            		Hue = 1152;

			Attributes.AttackChance = 100;
			Attributes.BonusDex = 100;
			Attributes.BonusHits = 100;
			Attributes.BonusInt = 75;
			Attributes.BonusMana = 75;
			Attributes.BonusStam = 75;
			Attributes.BonusStr = 75;
			Attributes.CastRecovery = 3;
			Attributes.CastSpeed = 2;
			Attributes.DefendChance = 50;
			Attributes.LowerManaCost = 350;
			Attributes.Luck = 100;
			Attributes.ReflectPhysical = 15;
			Attributes.RegenHits = 50;
			Attributes.RegenMana = 50;
			Attributes.RegenStam = 50;
			Attributes.SpellChanneling = 1;
			Attributes.SpellDamage = 35;
			Attributes.WeaponDamage = 35;
			Attributes.WeaponSpeed = 50;

			ArmorAttributes.MageArmor = 1;

			ColdBonus = 80;
			DexBonus = 10;
			DexRequirement = 95;
			EnergyBonus = 75;
			FireBonus = 70;
			IntBonus = 10;
			IntRequirement = 95;
			PhysicalBonus = 10;
			PoisonBonus = 70;
			StrBonus = 10;
			StrRequirement = 95;

			LootType = LootType.Blessed;

		}

		public HandsOfJupiter( Serial serial ) : base( serial )
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
