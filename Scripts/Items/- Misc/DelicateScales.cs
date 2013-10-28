using System;

namespace Server.Items
{
    public class DelicateScales : Item
    {
        [Constructable]
        public DelicateScales()
            : this(1)
        {
        }

        [Constructable]
        public DelicateScales(int amount)
            : base(0x573A)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public DelicateScales(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113349;
            }
        }// delicate scales
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}