using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class RedDragoonGorget : BaseDragoonArmor
	{
		[Constructable]
		public RedDragoonGorget() : base( 0x13D6 )
        {
            Resource = CraftResource.RedScales;
            Name = "Gorget "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public RedDragoonGorget(Serial serial)
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