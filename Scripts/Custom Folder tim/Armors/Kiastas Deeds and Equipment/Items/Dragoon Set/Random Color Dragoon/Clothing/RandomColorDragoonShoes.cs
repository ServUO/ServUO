using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
	[Flipable]
	public class RandomColorDragoonThighBoots : BaseDragoonClothing
	{
        public override CraftResource DefaultResource { get { return Settings.BaseEquipmentAttribute.ClothingDefaultResource; } }

		[Constructable]
		public RandomColorDragoonThighBoots() : this(0)
		{
		}

		[Constructable]
        public RandomColorDragoonThighBoots(int hue)  : base(0x1711, Layer.Shoes)
		{
            Name = "Boots "+Settings.DragoonEquipmentName.Suffix;
			Weight = 2.0;
		}

		public RandomColorDragoonThighBoots(Serial serial) : base(serial)
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
