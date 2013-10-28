using System;

namespace Server.Items
{
    public class DecoGinsengRoot : Item
    {
        [Constructable]
        public DecoGinsengRoot()
            : base(0x18EB)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoGinsengRoot(Serial serial)
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