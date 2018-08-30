using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[FlipableAttribute( 0x2647, 0x2648 )]
    public class BlueDragoonGreaves : BaseDragoonArmor
	{
		[Constructable]
		public BlueDragoonGreaves() : base( 0x2647 )
        {
            Resource = CraftResource.BlueScales;
            Name = "Greaves "+Settings.DragoonEquipmentName.Suffix;
			Weight = 6.0;
		}

        public BlueDragoonGreaves(Serial serial)
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