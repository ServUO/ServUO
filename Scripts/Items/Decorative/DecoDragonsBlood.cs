using System;

namespace Server.Items
{
    public class DecoDragonsBlood : Item
    {
        [Constructable]
        public DecoDragonsBlood()
            : base(0x4077)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public DecoDragonsBlood(Serial serial)
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