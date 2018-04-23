//Bale
using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class BalesHatred : HoodedShroudOfShadows
  {
public override int ArtifactRarity{ get{ return 187; } }

		public override int InitMinHits{ get{ return 20; } }
		public override int InitMaxHits{ get{ return 20; } }

        //public override int BaseColdResistance{ get{ return 5; } } 
        //public override int BaseEnergyResistance{ get{ return 5; } } 
        //public override int BasePhysicalResistance{ get{ return 5; } } 
        //public override int BasePoisonResistance{ get{ return 5; } } 
        //public override int BaseFireResistance{ get{ return 5; } } 
      
      [Constructable]
		public BalesHatred()
		{
      Name = "Bales Hatred";
      Hue = 1366;
      Attributes.AttackChance = 10;
      //Attributes.BonusDex = 2;
      //Attributes.BonusHits = 2;
      //Attributes.BonusInt = 2;
      Attributes.BonusMana = 2;
      Attributes.BonusStam = 2;
      Attributes.CastRecovery = 3;
      Attributes.CastSpeed = 2;
      Attributes.DefendChance = 10;
      //Attributes.EnhancePotions = 5;
      //Attributes.LowerManaCost = 10;
      Attributes.LowerRegCost = 10;
      Attributes.Luck = 187;
      Attributes.NightSight = 1;
      Attributes.ReflectPhysical = 10;
      Attributes.RegenHits = 2;
      Attributes.RegenMana = 2;
      Attributes.RegenStam = 2;
      //Attributes.SpellDamage = 25;
      Attributes.WeaponDamage = 20;
      //Attributes.BonusMana = 5;
      //LootType = LootType.Blessed;
		}

		public BalesHatred( Serial serial ) : base( serial )
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
