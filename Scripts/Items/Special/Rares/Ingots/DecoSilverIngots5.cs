using System;

namespace Server.Items
{
    public class DecoSilverIngots5 : Item
    {
        [Constructable]
        public DecoSilverIngots5()
            : base(0x1BFA)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoSilverIngots5(Serial serial)
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