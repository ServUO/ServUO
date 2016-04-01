using System;

namespace Server.Items
{
    public class DecoMandrake2 : Item
    {
        [Constructable]
        public DecoMandrake2()
            : base(0x18E0)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoMandrake2(Serial serial)
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