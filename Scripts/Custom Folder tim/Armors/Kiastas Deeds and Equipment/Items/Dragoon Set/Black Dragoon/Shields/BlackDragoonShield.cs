using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class BlackDragoonShield : BaseDragoonShield
	{
		[Constructable]
        public BlackDragoonShield()
            : base(0x1B79)
        {
            Resource = CraftResource.BlackScales;
            Name = "Shield " + Settings.DragoonEquipmentName.Suffix;
			Weight = 5.0;
		}

		public BlackDragoonShield( Serial serial ) : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 );
		}

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
	}
}
