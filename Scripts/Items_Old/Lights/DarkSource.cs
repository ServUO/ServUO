using System;

namespace Server.Items
{
    public class DarkSource : Item
    {
        [Constructable]
        public DarkSource()
            : base(0x1646)
        {
            this.Layer = Layer.TwoHanded;
            this.Movable = false;
        }

        public DarkSource(Serial serial)
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