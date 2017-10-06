using System;

namespace Server.Items
{
    [TypeAlias("Server.Items.Dressform")]
    public class DressformFront : Item
    {
        [Constructable]
        public DressformFront()
            : base(0xec6)
        {
            this.Weight = 10;
        }

        public DressformFront(Serial serial)
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
    
    public class DressformSide : Item
    {
        [Constructable]
        public DressformSide()
            : base(0xec7)
        {
            Weight = 10;
        }

        public DressformSide(Serial serial)
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