using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class MerlinsPants : LeatherLegs, ITokunoDyable
  {


		public override int BaseColdResistance{ get{ return 14; } } 
		public override int BaseEnergyResistance{ get{ return 8; } } 
		public override int BasePhysicalResistance{ get{ return 6; } } 
		public override int BasePoisonResistance{ get{ return 6; } } 
		public override int BaseFireResistance{ get{ return 7; } } 
		public override int ArtifactRarity{ get{ return 15; } }
      
      [Constructable]
		public MerlinsPants()
		{
          Name = "Merlin's Pants";
          Hue = 1265;
      ArmorAttributes.MageArmor = 1;
      Attributes.LowerManaCost = 5;
      Attributes.LowerRegCost = 15;
      Attributes.Luck = 150;
      Attributes.RegenMana = 5;
      Attributes.SpellDamage = 10;
      Attributes.BonusMana = 15;
		}

		public MerlinsPants( Serial serial ) : base( serial )
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
