//JJ
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AngelLegs : PlateLegs
  {
public override int ArtifactRarity{ get{ return 567; } }

		public override int InitMinHits{ get{ return 1000; } }
		public override int InitMaxHits{ get{ return 1000; } }

		public override int BaseColdResistance{ get{ return 50; } } 
		public override int BaseEnergyResistance{ get{ return 50; } } 
		public override int BasePhysicalResistance{ get{ return 50; } } 
		public override int BasePoisonResistance{ get{ return 50; } } 
		public override int BaseFireResistance{ get{ return 50; } } 
      
      [Constructable]
		public AngelLegs()
		{
			Weight = 60;
          Name = "Angel Legs";
          Hue = 1153;
      ArmorAttributes.DurabilityBonus = 300;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 500;
      Attributes.AttackChance = 100;
      Attributes.BonusDex = 300;
      Attributes.BonusStam = 500;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 3;
      Attributes.DefendChance = 100;
      Attributes.LowerManaCost = 111;
      Attributes.LowerRegCost = 111;
      Attributes.Luck = 5000;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 50;
      Attributes.RegenStam = 500;
      Attributes.SpellDamage = 200;
      Attributes.WeaponDamage = 90;
      LootType = LootType.Blessed;
		}

		public AngelLegs( Serial serial ) : base( serial )
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
