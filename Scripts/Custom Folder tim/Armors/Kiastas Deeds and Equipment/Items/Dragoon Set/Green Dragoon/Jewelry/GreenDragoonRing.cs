using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class GreenDragoonRing : BaseDragoonJewel
	{
		[Constructable]
		public GreenDragoonRing() : base( 0x108a, Layer.Ring )
        {
            Name = "Ring "+Settings.DragoonEquipmentName.Suffix;
			Weight = 0.1;
		}

        public GreenDragoonRing(Serial serial)
            : base(serial)
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
