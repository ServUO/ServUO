using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class RandomColorDragoonRing : BaseDragoonJewel
	{
		[Constructable]
		public RandomColorDragoonRing() : base( 0x108a, Layer.Ring )
		{
            Name = "Ring "+Settings.DragoonEquipmentName.Suffix;
			Weight = 0.1;
		}

		public RandomColorDragoonRing( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}
