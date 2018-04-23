
using System;
using Server;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x153B, 0x153C )]
	public class IvoryCincture : BaseWaist
	{
 		public override void GetProperties(ObjectPropertyList list)
		{
		base.GetProperties(list); list.Add(1075085);
		}

              	[Constructable]
              	public IvoryCincture() : base ( 0x153B )
		{
			Weight = 2.0;
			Name = "Ivory Cincture";
			Hue = 1153;

			Attributes.LowerManaCost = 10; 
		}

		public IvoryCincture( Serial serial ) : base( serial )
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
