using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    [FlipableAttribute (0x2641, 0x2642)]
    public class BlueDragoonCuirass : BaseDragoonArmor
	{
		[Constructable]
		public BlueDragoonCuirass() : base( 0x2641 )
        {
            Resource = CraftResource.BlueScales;
            Name = "Cuirass "+Settings.DragoonEquipmentName.Suffix;
			Weight = 10.0;
		}

        public BlueDragoonCuirass(Serial serial)
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