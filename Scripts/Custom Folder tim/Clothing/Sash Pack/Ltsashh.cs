
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x153B, 0x153C )]
	public class Ltsashh : BaseMiddleTorso
	{
 		public override void GetProperties(ObjectPropertyList list)
		{
		base.GetProperties(list); list.Add(1075085);
		}

              	[Constructable]
              	public Ltsashh() : base ( 0x1541 )
		{
			Weight = 2.0;
			Name = "Holy Sash";
			Hue = 1153;

			Attributes.BonusMana = 7;
			Attributes.BonusHits = 7;
			Attributes.BonusStam = 7; 
		}

		public Ltsashh( Serial serial ) : base( serial )
		{
		}
              
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}
              
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
