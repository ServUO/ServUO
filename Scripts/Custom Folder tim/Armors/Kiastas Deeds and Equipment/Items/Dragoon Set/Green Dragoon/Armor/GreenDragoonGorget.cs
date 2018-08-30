using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class GreenDragoonGorget : BaseDragoonArmor
	{
		[Constructable]
		public GreenDragoonGorget() : base( 0x13D6 )
        {
            Resource = CraftResource.GreenScales;
            Name = "Gorget "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public GreenDragoonGorget(Serial serial)
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