using System;

namespace Server.Items
{
    public class DecoObsidian : Item
    {
        [Constructable]
        public DecoObsidian()
            : base(0xF89)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoObsidian(Serial serial)
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