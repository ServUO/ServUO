using System;

namespace Server.Items
{
    public class BouraPelt : Item
    {
        public override int LabelNumber { get { return 1113355; } } // boura pelt

        [Constructable]
        public BouraPelt()
            : this(1)
        {
        }

        [Constructable]
        public BouraPelt(int amount)
            : base(0x5742)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 0.1;
        }

        public BouraPelt(Serial serial)
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