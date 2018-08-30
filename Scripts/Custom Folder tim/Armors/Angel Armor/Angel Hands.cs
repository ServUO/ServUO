//JJ
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AngelHands : PlateGloves
  {
public override int ArtifactRarity{ get{ return 654; } }

		public override int InitMinHits{ get{ return 500; } }
		public override int InitMaxHits{ get{ return 500; } }

		public override int BaseColdResistance{ get{ return 50; } } 
		public override int BaseEnergyResistance{ get{ return 50; } } 
		public override int BasePhysicalResistance{ get{ return 50; } } 
		public override int BasePoisonResistance{ get{ return 50; } } 
		public override int BaseFireResistance{ get{ return 50; } } 
      
      [Constructable]
		public AngelHands()
		{
			Weight = 10;
          Name = "Angel Hands";
          Hue = 1153;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 500;
      Attributes.AttackChance = 100;
      Attributes.BonusDex = 500;
      Attributes.BonusHits = 250;
      Attributes.BonusInt = 100;
      Attributes.BonusMana = 300;
      Attributes.BonusStam = 500;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 3;
      Attributes.DefendChance = 100;
      Attributes.LowerManaCost = 50;
      Attributes.LowerRegCost = 50;
      Attributes.Luck = 500;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 50;
      Attributes.RegenHits = 100;
      Attributes.SpellDamage = 300;
      Attributes.WeaponDamage = 200;
      Attributes.BonusMana = 300;
      LootType = LootType.Blessed;
		}

		public AngelHands( Serial serial ) : base( serial )
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
