//JJ
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AngelArms : PlateArms
  {
public override int ArtifactRarity{ get{ return 519; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		public override int BaseColdResistance{ get{ return 50; } } 
		public override int BaseEnergyResistance{ get{ return 50; } } 
		public override int BasePhysicalResistance{ get{ return 50; } } 
		public override int BasePoisonResistance{ get{ return 50; } } 
		public override int BaseFireResistance{ get{ return 50; } } 
      
      [Constructable]
		public AngelArms()
		{
			Weight = 10;
          Name = "Angel Arms";
          Hue = 1153;
      ArmorAttributes.DurabilityBonus = 20;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 500;
      Attributes.AttackChance = 100;
      Attributes.BonusDex = 450;
      Attributes.BonusHits = 300;
      Attributes.BonusMana = 300;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 3;
      Attributes.DefendChance = 100;
      Attributes.LowerManaCost = 100;
      Attributes.LowerRegCost = 100;
      Attributes.Luck = 500;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 50;
      Attributes.RegenHits = 100;
      Attributes.RegenMana = 250;
      Attributes.SpellDamage = 200;
      Attributes.WeaponDamage = 250;
      Attributes.BonusMana = 300;
      LootType = LootType.Blessed;
		}

		public AngelArms( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}
