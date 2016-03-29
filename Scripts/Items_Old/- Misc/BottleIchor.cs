using System;

namespace Server.Items
{
    public class BottleIchor : Item
    {
        [Constructable]
        public BottleIchor()
            : this(1)
        {
        }

        [Constructable]
        public BottleIchor(int amount)
            : base(0x5748)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public BottleIchor(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113361;
            }
        }// bottle of ichor
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