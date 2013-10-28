using System;

namespace Server.Items
{
    public class DecoRock : Item
    {
        [Constructable]
        public DecoRock()
            : base(0x1778)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoRock(Serial serial)
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