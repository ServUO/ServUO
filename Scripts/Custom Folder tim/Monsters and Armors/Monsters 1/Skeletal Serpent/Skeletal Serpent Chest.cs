//Created by Neptune
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class SkeletalSerpentChest : BoneChest
  {


		public override int BaseColdResistance{ get{ return 14; } } 
		public override int BaseEnergyResistance{ get{ return 14; } } 
		public override int BasePhysicalResistance{ get{ return 14; } } 
		public override int BasePoisonResistance{ get{ return 14; } } 
		public override int BaseFireResistance{ get{ return 14; } } 
      
      [Constructable]
		public SkeletalSerpentChest()
		{
          Name = "Skeletal Serpent Chest";
      ArmorAttributes.SelfRepair = 5;
      Attributes.AttackChance = 25;
      Attributes.BonusDex = 10;
      Attributes.BonusHits = 15;
      Attributes.BonusInt = 10;
      Attributes.BonusMana = 15;
      Attributes.BonusStam = 15;
      Attributes.CastRecovery = 2;
      Attributes.CastSpeed = 2;
      Attributes.DefendChance = 25;
      Attributes.LowerManaCost = 15;
      Attributes.LowerRegCost = 25;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 25;
      Attributes.SpellDamage = 25;
      Attributes.WeaponDamage = 25;
      Attributes.BonusMana = 15;
		}

		public SkeletalSerpentChest( Serial serial ) : base( serial )
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
