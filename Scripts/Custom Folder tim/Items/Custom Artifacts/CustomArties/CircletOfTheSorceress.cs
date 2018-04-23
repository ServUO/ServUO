using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class CircletOfTheSorceress : Circlet, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 19; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int BaseColdResistance{ get{ return 11; } } 
		public override int BaseEnergyResistance{ get{ return 12; } } 
		public override int BasePhysicalResistance{ get{ return 2; } } 
		public override int BasePoisonResistance{ get{ return 8; } } 
		public override int BaseFireResistance{ get{ return 12; } } 
      
      [Constructable]
		public CircletOfTheSorceress()
		{
          Name = "Circlet Of The Sorceress";
          Hue = 2062;
      ArmorAttributes.MageArmor = 1;
      ArmorAttributes.SelfRepair = 3;
      Attributes.BonusMana = 15;
      Attributes.LowerManaCost = 6;
      Attributes.LowerRegCost = 10;
		}

		public CircletOfTheSorceress( Serial serial ) : base( serial )
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
