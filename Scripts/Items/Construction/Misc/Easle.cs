using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0xF65, 0xF67, 0xF69)]
    public class Easle : Item
    {
        [Constructable]
        public Easle()
            : base(0xF65)
        {
            this.Weight = 25.0;
        }

        public Easle(Serial serial)
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

            if (this.Weight == 10.0)
                this.Weight = 25.0;
        }
    }
}