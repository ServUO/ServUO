using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[FlipableAttribute( 0x2657, 0x2658 )]
	public class WhiteDragoonPaulders : BaseDragoonArmor
	{
		[Constructable]
		public WhiteDragoonPaulders() : base( 0x2657 )
        {
            Resource = CraftResource.WhiteScales;
            Name = "Paulders "+Settings.DragoonEquipmentName.Suffix;
			Weight = 5.0;
		}

        public WhiteDragoonPaulders(Serial serial)
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