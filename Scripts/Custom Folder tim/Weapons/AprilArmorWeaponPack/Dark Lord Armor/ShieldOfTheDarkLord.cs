// Scripted by SPanky
using System;
using Server;

namespace Server.Items
{
	public class shieldofthedarklord : MetalKiteShield
	{
		public override int ArtifactRarity{ get{ return 60; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public shieldofthedarklord()
		{
			Weight = 8.0; 
            		Name = "Shield Of The Dark Lord"; 
            		Hue = 1175;

			//Attributes.AttackChance = nn;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			Attributes.BonusMana = 20;
			//Attributes.BonusStam = nn;
			Attributes.BonusStr = 10;
			Attributes.CastRecovery = 14;
			Attributes.CastSpeed = 20;
			Attributes.DefendChance = 24;
			//Attributes.EnhancePotions = nn;
			//Attributes.LowerManaCost = nn;
			//Attributes.LowerRegCost = nn;
			//Attributes.Luck = nn;
			//Attributes.Nightsight = 1;
			Attributes.ReflectPhysical = 16;
			//Attributes.RegenHits = nn;
			//Attributes.RegenMana = nn;
			//Attributes.RegenStam = nn;
			Attributes.SpellChanneling = 1;
			//Attributes.SpellDamage = nn;
			//Attributes.WeaponDamage = nn;
			//Attributes.WeaponSpeed = nn;

			//ArmorAttributes.DurabilityBonus = nn;
			//ArmorAttributes.LowerStatReq = nn;
			//ArmorAttributes.MageArmor = 1;
			ArmorAttributes.SelfRepair = 26;

			//ColdBonus = nn;
			//DexBonus = nn;
			//DexRequirement = nn;
			//EnergyBonus = nn;
			//FireBonus = nn;
			//IntBonus = nn;
			IntRequirement = 60;
			//PhysicalBonus = nn;
			//PoisonBonus = nn;
			//StrBonus = nn;
			StrRequirement = 50;

			LootType = LootType.Blessed;

		}

		public shieldofthedarklord( Serial serial ) : base( serial )
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