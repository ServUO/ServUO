
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x153B, 0x153C )]
	public class SilverCincture : BaseWaist
	{
 		public override void GetProperties(ObjectPropertyList list)
		{
		base.GetProperties(list); list.Add(1075085);
		}

              	[Constructable]
              	public SilverCincture() : base ( 0x153B )
		{
			Weight = 2.0;
			Name = "Silver Cincture";
			Hue = 1154;

			Attributes.RegenHits = 2;
			Attributes.RegenMana = 2;
			Attributes.RegenStam = 2; 
		}

		public SilverCincture( Serial serial ) : base( serial )
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
