using System;

namespace Server.Items
{
    public class DecoBridle2 : Item
    {
        [Constructable]
        public DecoBridle2()
            : base(0x1375)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoBridle2(Serial serial)
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