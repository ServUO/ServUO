using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable]
	public class YellowDragoonThighBoots : BaseDragoonClothing
	{
        public override CraftResource DefaultResource { get { return Settings.BaseEquipmentAttribute.ClothingDefaultResource; } }

		[Constructable]
		public YellowDragoonThighBoots() : this(0)
		{
		}

		[Constructable]
        public YellowDragoonThighBoots(int hue)  : base(0x1711, Layer.Shoes)
        {
            Resource = CraftResource.YellowScales;
            Name = "Boots "+Settings.DragoonEquipmentName.Suffix;
			Weight = 2.0;
		}

        public YellowDragoonThighBoots(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write((int)0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
