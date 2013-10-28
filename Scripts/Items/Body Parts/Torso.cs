using System;

namespace Server.Items
{
    public class Torso : Item
    {
        [Constructable]
        public Torso()
            : base(0x1D9F)
        {
            this.Weight = 2.0;
        }

        public Torso(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}