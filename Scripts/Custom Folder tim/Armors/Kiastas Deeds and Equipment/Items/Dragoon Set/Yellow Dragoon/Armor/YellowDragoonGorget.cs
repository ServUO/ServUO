using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class YellowDragoonGorget : BaseDragoonArmor
	{
		[Constructable]
		public YellowDragoonGorget() : base( 0x13D6 )
        {
            Resource = CraftResource.YellowScales;
            Name = "Gorget "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public YellowDragoonGorget(Serial serial)
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