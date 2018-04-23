using System;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
	public class GeishasObi : Obi, ITokunoDyable

  {
      public override int ArtifactRarity{ get{ return 12; } }


		public override int BasePhysicalResistance{ get{ return 5; } } 
      
      [Constructable]
		public GeishasObi()
		{
          Name = "Geishas Obi";
          Hue = 31;
      Attributes.BonusInt = 5;
      Attributes.DefendChance = 5;
      Attributes.ReflectPhysical = 10;
      Attributes.RegenHits = 3;
		}

		public GeishasObi( Serial serial ) : base( serial )
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
