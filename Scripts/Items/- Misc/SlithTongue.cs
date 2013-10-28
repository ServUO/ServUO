using System;

namespace Server.Items
{
    public class SlithTongue : Item
    {
        [Constructable]
        public SlithTongue()
            : this(1)
        {
        }

        [Constructable]
        public SlithTongue(int amount)
            : base(0x5746)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public SlithTongue(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113359;
            }
        }// slith tongue
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