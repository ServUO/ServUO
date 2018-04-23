// Scripted by SPanky. Ask me before you edit my scripts please.
using System;
using Server;

namespace Server.Items
{
	public class chestplateofthelight : PlateChest
	{
		public override int ArtifactRarity{ get{ return 57; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public chestplateofthelight()
		{
			Weight = 8.0; 
            		Name = "Chest Plate of the Light"; 
            		Hue = 1153;

			//Attributes.AttackChance = nn;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			//Attributes.BonusMana = nn;
			//Attributes.BonusStam = nn;
			//Attributes.BonusStr = nn;
			Attributes.CastRecovery = 23;
			Attributes.CastSpeed = 16;
			Attributes.DefendChance = 36;
			//Attributes.EnhancePotions = nn;
			//Attributes.LowerManaCost = nn;
			//Attributes.LowerRegCost = nn;
			//Attributes.Luck = nn;
			//Attributes.Nightsight = 1;
			Attributes.ReflectPhysical = 14;
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
			ArmorAttributes.SelfRepair = 15;

			//ColdBonus = nn;
			//DexBonus = nn;
			//DexRequirement = nn;
			//EnergyBonus = nn;
			//FireBonus = nn;
			//IntBonus = nn;
			IntRequirement = 28;
			//PhysicalBonus = nn;
			//PoisonBonus = nn;
			//StrBonus = nn;
			StrRequirement = 40;

			LootType = LootType.Blessed;

		}

		public chestplateofthelight( Serial serial ) : base( serial )
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