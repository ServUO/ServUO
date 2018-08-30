//JJ
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AngelTunic : PlateChest
  {
public override int ArtifactRarity{ get{ return 567; } }

		public override int InitMinHits{ get{ return 5000; } }
		public override int InitMaxHits{ get{ return 5000; } }

		public override int BaseColdResistance{ get{ return 50; } } 
		public override int BaseEnergyResistance{ get{ return 50; } } 
		public override int BasePhysicalResistance{ get{ return 50; } } 
		public override int BasePoisonResistance{ get{ return 50; } } 
		public override int BaseFireResistance{ get{ return 50; } } 
      
      [Constructable]
		public AngelTunic()
		{
			Weight = 10;
          Name = "Angel Tunic";
          Hue = 1153;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 450;
      Attributes.AttackChance = 45;
      Attributes.BonusDex = 450;
      Attributes.BonusHits = 500;
      Attributes.BonusInt = 100;
      Attributes.BonusMana = 350;
      Attributes.BonusStam = 450;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 3;
      Attributes.DefendChance = 78;
      Attributes.LowerManaCost = 55;
      Attributes.LowerRegCost = 50;
      Attributes.Luck = 500;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 50;
      Attributes.RegenHits = 250;
      Attributes.RegenMana = 100;
      Attributes.RegenStam = 450;
      Attributes.SpellDamage = 111;
      Attributes.WeaponDamage = 100;
      Attributes.BonusMana = 350;
      LootType = LootType.Blessed;
		}

		public AngelTunic( Serial serial ) : base( serial )
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
