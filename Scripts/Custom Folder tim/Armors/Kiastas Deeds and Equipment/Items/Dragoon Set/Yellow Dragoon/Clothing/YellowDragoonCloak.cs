using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable]
	public class YellowDragoonCloak : BaseDragoonClothing
	{
		[Constructable]
		public YellowDragoonCloak() : this(0)
		{
		}
		[Constructable]
		public YellowDragoonCloak(int hue) : base(0x1515, Layer.Cloak)
        {
            Resource = CraftResource.YellowScales;
            Name = "Cloak "+Settings.DragoonEquipmentName.Suffix;
			Weight = 1.0;
		}
        public YellowDragoonCloak(Serial serial)
            : base(serial)
		{
		}
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
            writer.Write((int)0);
		}
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			if ( Weight == 4.0 )
				Weight = 3.0;
		}
	}
}