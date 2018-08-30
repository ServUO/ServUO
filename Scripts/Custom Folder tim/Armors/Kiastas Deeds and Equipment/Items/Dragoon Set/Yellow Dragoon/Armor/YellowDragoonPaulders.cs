using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[FlipableAttribute( 0x2657, 0x2658 )]
	public class YellowDragoonPaulders : BaseDragoonArmor
	{
		[Constructable]
		public YellowDragoonPaulders() : base( 0x2657 )
        {
            Resource = CraftResource.YellowScales;
            Name = "Paulders "+Settings.DragoonEquipmentName.Suffix;
			Weight = 5.0;
		}

        public YellowDragoonPaulders(Serial serial)
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