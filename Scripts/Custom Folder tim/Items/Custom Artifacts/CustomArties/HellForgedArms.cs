using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class HellForgedArms : PlateArms, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 12; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int BaseColdResistance{ get{ return 5; } } 
		public override int BaseEnergyResistance{ get{ return 10; } } 
		public override int BasePhysicalResistance{ get{ return 9; } } 
		public override int BasePoisonResistance{ get{ return 9; } } 
		public override int BaseFireResistance{ get{ return 13; } } 
      
      [Constructable]
		public HellForgedArms()
		{
          Name = "Hell Forged Arms";
          Hue = 1208;
      ArmorAttributes.SelfRepair = 3;
      Attributes.AttackChance = 5;
      Attributes.DefendChance = 10;
      Attributes.EnhancePotions = 15;
      Attributes.LowerManaCost = 5;
      Attributes.SpellDamage = 15;
      Attributes.WeaponDamage = 10;
		}

		public HellForgedArms( Serial serial ) : base( serial )
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
