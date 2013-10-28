using System;

namespace Server.Items
{
    public class EmptyToolKit : Item
    {
        [Constructable]
        public EmptyToolKit()
            : base(0x1EB6)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public EmptyToolKit(Serial serial)
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