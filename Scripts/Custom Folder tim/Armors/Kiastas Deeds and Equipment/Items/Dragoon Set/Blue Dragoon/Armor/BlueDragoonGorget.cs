using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class BlueDragoonGorget : BaseDragoonArmor
	{
		[Constructable]
		public BlueDragoonGorget() : base( 0x13D6 )
        {
            Resource = CraftResource.BlueScales;
            Name = "Gorget "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}

        public BlueDragoonGorget(Serial serial)
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