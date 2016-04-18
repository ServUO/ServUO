using System;

namespace Server.Items
{
    public class VoidCore : Item
    {
        [Constructable]
        public VoidCore()
            : this(1)
        {
        }

        [Constructable]
        public VoidCore(int amount)
            : base(0x5728)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public VoidCore(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113334;
            }
        }// void core
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