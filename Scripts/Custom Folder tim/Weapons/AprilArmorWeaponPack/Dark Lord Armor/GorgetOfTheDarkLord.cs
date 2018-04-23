// Scripted by SPanky
using System;
using Server;

namespace Server.Items
{
	public class gorgetofthedarklord : PlateGorget
	{
		public override int ArtifactRarity{ get{ return 40; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public gorgetofthedarklord()
		{
			Weight = 3.0; 
            		Name = "Gorget Of The Dark Lord"; 
            		Hue = 1175;

			//Attributes.AttackChance = nn;
			//Attributes.BonusDex = nn;
			//Attributes.BonusHits = nn;
			//Attributes.BonusInt = nn;
			//Attributes.BonusMana = nn;
			//Attributes.BonusStam = nn;
			//Attributes.BonusStr = nn;
			Attributes.CastRecovery = 12;
			Attributes.CastSpeed = 16;
			Attributes.DefendChance = 6;
			//Attributes.EnhancePotions = nn;
			//Attributes.LowerManaCost = nn;
			//Attributes.LowerRegCost = nn;
			//Attributes.Luck = nn;
			//Attributes.Nightsight = 1;
			Attributes.ReflectPhysical = 8;
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
			IntRequirement = 14;
			//PhysicalBonus = nn;
			//PoisonBonus = nn;
			//StrBonus = nn;
			StrRequirement = 20;

			LootType = LootType.Blessed;

		}

		public gorgetofthedarklord( Serial serial ) : base( serial )
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