using System;

namespace Server.Items
{
    public class Lockpicks : Item
    {
        [Constructable]
        public Lockpicks()
            : base(Utility.Random(2) + 0x14FD)
        {
            this.Movable = true;
            this.Stackable = false;
        }

        public Lockpicks(Serial serial)
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