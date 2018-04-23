//Created by Neptune
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class OblivionChest : LeatherChest
  {


		public override int BaseColdResistance{ get{ return 25; } } 
		public override int BaseEnergyResistance{ get{ return 25; } } 
		public override int BasePhysicalResistance{ get{ return 25; } } 
		public override int BasePoisonResistance{ get{ return 25; } } 
		public override int BaseFireResistance{ get{ return 25; } } 
      
      [Constructable]
		public OblivionChest()
		{
          Name = "Chest of Oblivion";
          Hue = 1261;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 5;
      Attributes.AttackChance = 25;
      Attributes.BonusHits = 50;
      Attributes.BonusMana = 25;
      Attributes.BonusStam = 25;
      Attributes.CastRecovery = 5;
      Attributes.CastSpeed = 5;
      Attributes.DefendChance = 25;
      Attributes.EnhancePotions = 15;
      Attributes.LowerManaCost = 20;
      Attributes.LowerRegCost = 30;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 35;
      Attributes.RegenHits = 5;
      Attributes.RegenMana = 5;
      Attributes.RegenStam = 25;
      Attributes.SpellDamage = 25;
      Attributes.WeaponDamage = 35;
      Attributes.BonusMana = 25;
		}

		public OblivionChest( Serial serial ) : base( serial )
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
