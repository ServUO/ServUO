using System;

namespace Server.Items
{
    public class DecoMandrakeRoot2 : Item
    {
        [Constructable]
        public DecoMandrakeRoot2()
            : base(0x18DD)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoMandrakeRoot2(Serial serial)
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