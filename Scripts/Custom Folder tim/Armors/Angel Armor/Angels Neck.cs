//JJ
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AngelsNeck : PlateGorget
  {
public override int ArtifactRarity{ get{ return 543; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		public override int BaseColdResistance{ get{ return 50; } } 
		public override int BaseEnergyResistance{ get{ return 50; } } 
		public override int BasePhysicalResistance{ get{ return 50; } } 
		public override int BasePoisonResistance{ get{ return 50; } } 
		public override int BaseFireResistance{ get{ return 50; } } 
      
      [Constructable]
		public AngelsNeck()
		{
			Weight = 10;
          Name = "Angels Neck";
          Hue = 1153;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 500;
      Attributes.AttackChance = 200;
      Attributes.BonusDex = 250;
      Attributes.BonusHits = 600;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 3;
      Attributes.DefendChance = 200;
      Attributes.LowerManaCost = 100;
      Attributes.LowerRegCost = 100;
      Attributes.Luck = 500;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 50;
      Attributes.RegenHits = 500;
      Attributes.SpellDamage = 50;
      Attributes.WeaponDamage = 100;
      LootType = LootType.Blessed;
		}

		public AngelsNeck( Serial serial ) : base( serial )
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
