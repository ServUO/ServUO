using System;

namespace Server.Items
{
    public class DecoIronIngots5 : Item
    {
        [Constructable]
        public DecoIronIngots5()
            : base(0x1BF3)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoIronIngots5(Serial serial)
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