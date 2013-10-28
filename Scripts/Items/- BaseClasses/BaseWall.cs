using System;

namespace Server.Items
{
    public abstract class BaseWall : Item
    {
        public BaseWall(int itemID)
            : base(itemID)
        {
            this.Movable = false;
        }

        public BaseWall(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}