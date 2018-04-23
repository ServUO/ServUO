using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class GlovesOfCorruption : LeafGloves, ITokunoDyable
  {
public override int ArtifactRarity{ get{ return 13; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		public override int BaseColdResistance{ get{ return 10; } } 
		public override int BaseEnergyResistance{ get{ return 8; } } 
		public override int BasePhysicalResistance{ get{ return 10; } } 
		public override int BasePoisonResistance{ get{ return 10; } } 
		public override int BaseFireResistance{ get{ return 7; } } 
      
      [Constructable]
		public GlovesOfCorruption()
		{
          Name = "Gloves Of Corruption";
          Hue = 2070;
      ArmorAttributes.MageArmor = 1;
      Attributes.AttackChance = 5;
      Attributes.DefendChance = 5;
      Attributes.SpellDamage = 10;
      Attributes.WeaponDamage = 15;
		}

		public GlovesOfCorruption( Serial serial ) : base( serial )
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
