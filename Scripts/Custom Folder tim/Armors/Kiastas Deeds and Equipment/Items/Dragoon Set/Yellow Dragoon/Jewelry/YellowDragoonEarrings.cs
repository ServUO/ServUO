using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public class YellowDragoonEarrings : BaseDragoonJewel
	{
		[Constructable]
		public YellowDragoonEarrings() : base( 0x1087, Layer.Earrings )
        {
            Name = "Earrings "+Settings.DragoonEquipmentName.Suffix;
			Weight = 0.1;
		}

        public YellowDragoonEarrings(Serial serial)
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