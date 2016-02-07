using System;

namespace Server.Items
{
    public class GrizzledSkullCollection : Item
    {
        [Constructable]
        public GrizzledSkullCollection()
            : base(0x21FC)
        {
        }

        public GrizzledSkullCollection(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072116;
            }
        }// Grizzled Skull collection
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