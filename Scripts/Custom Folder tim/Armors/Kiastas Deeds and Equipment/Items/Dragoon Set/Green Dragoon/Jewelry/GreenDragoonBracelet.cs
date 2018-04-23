using System;
using Server;
using Server.Items;

namespace Server.Kiasta
{
    public class GreenDragoonBracelet : BaseDragoonJewel
    {
        [Constructable]
        public GreenDragoonBracelet() : base(0x1086, Layer.Bracelet)
        {
            Name = "Bracelet "+Settings.DragoonEquipmentName.Suffix;
            Weight = 0.1;
        }

        public GreenDragoonBracelet(Serial serial)
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
