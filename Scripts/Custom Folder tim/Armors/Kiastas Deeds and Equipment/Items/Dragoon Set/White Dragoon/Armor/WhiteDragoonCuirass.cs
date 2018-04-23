using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    [FlipableAttribute (0x2641, 0x2642)]
    public class WhiteDragoonCuirass : BaseDragoonArmor
	{
		[Constructable]
		public WhiteDragoonCuirass() : base( 0x2641 )
        {
            Resource = CraftResource.WhiteScales;
            Name = "Cuirass "+Settings.DragoonEquipmentName.Suffix;
			Weight = 10.0;
		}

        public WhiteDragoonCuirass(Serial serial)
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