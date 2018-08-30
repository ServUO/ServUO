using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class GlovesOfRegeneration : LeatherGloves, ITokunoDyable

  {
      public override int ArtifactRarity{ get{ return 12; } }

		public override int BaseColdResistance{ get{ return 8; } } 
		public override int BaseEnergyResistance{ get{ return 10; } } 
		public override int BasePhysicalResistance{ get{ return 5; } } 
		public override int BasePoisonResistance{ get{ return 15; } } 
		public override int BaseFireResistance{ get{ return 10; } }

      
      [Constructable]
		public GlovesOfRegeneration()
		{
          Name = "Gloves Of Regeneration";
          Hue = 1284;
      Attributes.RegenHits = 5;
      Attributes.RegenMana = 5;
      Attributes.RegenStam = 5;
		}

		public GlovesOfRegeneration( Serial serial ) : base( serial )
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
