using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[FlipableAttribute( 0x2647, 0x2648 )]
    public class GreenDragoonGreaves : BaseDragoonArmor
	{
		[Constructable]
		public GreenDragoonGreaves() : base( 0x2647 )
        {
            Resource = CraftResource.GreenScales;
            Name = "Greaves "+Settings.DragoonEquipmentName.Suffix;
			Weight = 6.0;
		}

        public GreenDragoonGreaves(Serial serial)
            : base(serial)
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