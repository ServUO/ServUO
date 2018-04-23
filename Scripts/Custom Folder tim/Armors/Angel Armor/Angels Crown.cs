//JJ
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class AngelsCrown : StandardPlateKabuto
  {
public override int ArtifactRarity{ get{ return 547; } }

		public override int InitMinHits{ get{ return 1000; } }
		public override int InitMaxHits{ get{ return 1000; } }

		public override int BaseColdResistance{ get{ return 50; } } 
		public override int BaseEnergyResistance{ get{ return 50; } } 
		public override int BasePhysicalResistance{ get{ return 50; } } 
		public override int BasePoisonResistance{ get{ return 50; } } 
		public override int BaseFireResistance{ get{ return 50; } } 
      
      [Constructable]
		public AngelsCrown()
		{
			Weight = 60;
          Name = "Angels Crown";
          Hue = 1153;
      ArmorAttributes.DurabilityBonus = 200;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 500;
      Attributes.AttackChance = 250;
      Attributes.BonusDex = 350;
      Attributes.BonusHits = 350;
      Attributes.BonusInt = 350;
      Attributes.BonusMana = 350;
      Attributes.BonusStam = 350;
      Attributes.CastRecovery = 6;
      Attributes.CastSpeed = 6;
      Attributes.DefendChance = 250;
      Attributes.LowerManaCost = 100;
      Attributes.LowerRegCost = 100;
      Attributes.Luck = 500;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 50;
      Attributes.SpellDamage = 350;
      Attributes.WeaponDamage = 100;
      Attributes.BonusMana = 350;
      LootType = LootType.Blessed;
		}

		public AngelsCrown( Serial serial ) : base( serial )
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
