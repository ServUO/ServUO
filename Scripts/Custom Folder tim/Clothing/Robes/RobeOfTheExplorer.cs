using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class RobeoftheExplorer : HoodedShroudOfShadows
  {
public override int ArtifactRarity{ get{ return 97; } }


		public override int BaseColdResistance{ get{ return 2; } } 
		public override int BaseEnergyResistance{ get{ return 2; } } 
		public override int BasePhysicalResistance{ get{ return 2; } } 
		public override int BasePoisonResistance{ get{ return 2; } } 
		public override int BaseFireResistance{ get{ return 2; } } 
      
      [Constructable]
		public RobeoftheExplorer()
		{
          Name = "Robe of the Explorer";
          Hue = 2006;
      Attributes.AttackChance = 5;
      Attributes.BonusHits = 15;
      Attributes.ReflectPhysical = 10;
      Attributes.RegenHits = 5;
      Attributes.WeaponDamage = 10;
		}

		public RobeoftheExplorer( Serial serial ) : base( serial )
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
