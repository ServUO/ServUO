using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	public class WhiteDragoonShield : BaseDragoonShield
	{
		[Constructable]
        public WhiteDragoonShield()
            : base(0x1B79)
        {
            Resource = CraftResource.WhiteScales;
            Name = "Shield " + Settings.DragoonEquipmentName.Suffix;
			Weight = 5.0;
		}

		public WhiteDragoonShield( Serial serial ) : base(serial)
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
