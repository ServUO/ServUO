using System;

namespace Server.Items
{
    public class UnfinishedBarrel : Item
    {
        [Constructable]
        public UnfinishedBarrel()
            : base(0x1EB5)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public UnfinishedBarrel(Serial serial)
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