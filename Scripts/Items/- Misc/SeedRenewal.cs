using System;

namespace Server.Items
{
    public class SeedRenewal : Item
    {
        [Constructable]
        public SeedRenewal()
            : this(1)
        {
        }

        [Constructable]
        public SeedRenewal(int amount)
            : base(0x5736)
        {
            this.Stackable = true;
            this.Amount = amount;
        }

        public SeedRenewal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1113345;
            }
        }// seed of renewal
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