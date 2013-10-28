using System;

namespace Server.Items
{
    public class DecoTray2 : Item
    {
        [Constructable]
        public DecoTray2()
            : base(0x991)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoTray2(Serial serial)
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