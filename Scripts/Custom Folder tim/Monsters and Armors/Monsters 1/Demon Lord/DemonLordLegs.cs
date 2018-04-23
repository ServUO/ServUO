//Created by Neptune
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class DemonLordLegs : BoneLegs
  {
public override int ArtifactRarity{ get{ return 777; } }


		public override int BaseColdResistance{ get{ return 5; } } 
		public override int BaseEnergyResistance{ get{ return 5; } } 
		public override int BasePhysicalResistance{ get{ return 5; } } 
		public override int BasePoisonResistance{ get{ return 5; } } 
		public override int BaseFireResistance{ get{ return 5; } } 
      
      [Constructable]
		public DemonLordLegs()
		{
          Name = "Legs of the Demon Lord";
	  Hue = 1795;
      ArmorAttributes.SelfRepair = 5;
      Attributes.AttackChance = 25;
      Attributes.BonusDex = 10;
      Attributes.BonusHits = 10;
      Attributes.BonusInt = 10;
      Attributes.BonusMana = 10;
      Attributes.BonusStam = 10;
      Attributes.CastRecovery = 2;
      Attributes.CastSpeed = 2;
      Attributes.DefendChance = 25;
      Attributes.EnhancePotions = 10;
      Attributes.LowerManaCost = 10;
      Attributes.LowerRegCost = 20;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 5;
      Attributes.RegenHits = 5;
      Attributes.RegenMana = 5;
      Attributes.RegenStam = 10;
      Attributes.SpellDamage = 10;
      Attributes.WeaponDamage = 20;
      Attributes.BonusMana = 10;
		}

		public DemonLordLegs( Serial serial ) : base( serial )
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
