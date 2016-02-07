using System;

namespace Server.Items
{
    public class SlithEye : Item
    {
        [Constructable]
        public SlithEye()
            : this(1)
        {
        }

        [Constructable]
        public SlithEye(int amount)
			: base(0x5749)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public SlithEye(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112396;
            }
        }// slith's eye
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