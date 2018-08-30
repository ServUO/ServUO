// Scripted by SPanky
using System;
using Server;

namespace Server.Items
{
	public class helmofthelight : PlateHelm
	{
		public override int ArtifactRarity{ get{ return 60; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public helmofthelight()
		{
			Weight = 4.0; 
            		Name = "Helm of the Light"; 
            		Hue = 1153;

			//Attributes.AttackChance = nn;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			//Attributes.BonusMana = nn;
			//Attributes.BonusStam = nn;
			//Attributes.BonusStr = nn;
			Attributes.CastRecovery = 20;
			Attributes.CastSpeed = 16;
			Attributes.DefendChance = 18;
			//Attributes.EnhancePotions = nn;
			//Attributes.LowerManaCost = nn;
			//Attributes.LowerRegCost = nn;
			//Attributes.Luck = nn;
			//Attributes.Nightsight = 1;
			Attributes.ReflectPhysical = 10;
			//Attributes.RegenHits = nn;
			//Attributes.RegenMana = nn;
			//Attributes.RegenStam = nn;
			Attributes.SpellChanneling = 1;
			//Attributes.SpellDamage = nn;
			//Attributes.WeaponDamage = nn;
			//Attributes.WeaponSpeed = nn;

			//ArmorAttributes.DurabilityBonus = nn;
			//ArmorAttributes.LowerStatReq = nn;
			ArmorAttributes.MageArmor = 1;
			ArmorAttributes.SelfRepair = 14;

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
			StrRequirement = 40;

			LootType = LootType.Blessed;

		}

		public helmofthelight( Serial serial ) : base( serial )
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