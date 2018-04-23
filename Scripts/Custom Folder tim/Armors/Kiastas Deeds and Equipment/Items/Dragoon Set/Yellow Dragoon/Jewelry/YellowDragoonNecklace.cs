using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public class YellowDragoonNecklace : BaseDragoonJewel
    {
        [Constructable]
        public YellowDragoonNecklace() : base(0x1088, Layer.Neck)
        {
            Name = "Necklace "+Settings.DragoonEquipmentName.Suffix;
            Weight = 0.1;
        }

        public YellowDragoonNecklace(Serial serial)
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
        }
    }
}