using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public class RandomColorDragoonNecklace : BaseDragoonJewel
    {
        [Constructable]
        public RandomColorDragoonNecklace() : base(0x1088, Layer.Neck)
        {
            Name = "Necklace "+Settings.DragoonEquipmentName.Suffix;
            Weight = 0.1;
        }

        public RandomColorDragoonNecklace(Serial serial) : base(serial)
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
        }
    }
}