using System;

namespace Server.Items
{
    [FlipableAttribute(0x403A, 0x4046)]
    public class GargoyleFloorMirror : Item
    {
        [Constructable]
        public GargoyleFloorMirror()
            : base(0x403A)
        {
            this.Weight = 10;
        }

        public GargoyleFloorMirror(Serial serial)
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