using System;

namespace Server.Items
{
    [FlipableAttribute(0x4044, 0x4045)]
    public class GargoyleWallMirror : Item
    {
        [Constructable]
        public GargoyleWallMirror()
            : base(0x4044)
        {
            this.Weight = 10;
        }

        public GargoyleWallMirror(Serial serial)
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