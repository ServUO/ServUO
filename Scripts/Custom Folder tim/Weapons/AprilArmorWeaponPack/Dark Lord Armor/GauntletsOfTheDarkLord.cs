// Scripted by SPanky
using System;
using Server;

namespace Server.Items
{
	public class gauntletsofthedarklord : PlateGloves
	{
		public override int ArtifactRarity{ get{ return 40; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public gauntletsofthedarklord()
		{
			Weight = 9.0; 
            		Name = "Gauntlets Of The Dark Lord"; 
            		Hue = 1175;

			//Attributes.AttackChance = nn;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			//Attributes.BonusMana = nn;
			//Attributes.BonusStam = nn;
			Attributes.BonusStr = 12;
			Attributes.CastRecovery = 14;
			Attributes.CastSpeed = 19;
			Attributes.DefendChance = 15;
			//Attributes.EnhancePotions = nn;
			//Attributes.LowerManaCost = nn;
			//Attributes.LowerRegCost = nn;
			//Attributes.Luck = nn;
			//Attributes.Nightsight = 1;
			//Attributes.ReflectPhysical = nn;
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
			//ArmorAttributes.SelfRepair = nn;

			//ColdBonus = nn;
			//DexBonus = nn;
			//DexRequirement = nn;
			//EnergyBonus = nn;
			//FireBonus = nn;
			//IntBonus = nn;
			IntRequirement = 40;
			//PhysicalBonus = nn;
			//PoisonBonus = nn;
			//StrBonus = nn;
			StrRequirement = 30;

			LootType = LootType.Blessed;

		}

		public gauntletsofthedarklord( Serial serial ) : base( serial )
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