using System;

namespace Server.Items
{
    public class ReflectiveWolfEye : Item
    {
        [Constructable]
        public ReflectiveWolfEye()
            : this(1)
        {
        }

        [Constructable]
        public ReflectiveWolfEye(int amount)
            : base(0x5749)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public ReflectiveWolfEye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113362;
            }
        }// reflective wolf eye
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