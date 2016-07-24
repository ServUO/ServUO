using System;

namespace Server.Items
{
    public class MagicalResidue : Item
    {
        public override int LabelNumber { get { return 1031697; } } // Magical Residue

        [Constructable]
        public MagicalResidue()
            : this(1)
        {
        }

        [Constructable]
        public MagicalResidue(int amount)
            : base(0x2DB1)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public MagicalResidue(Serial serial)
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