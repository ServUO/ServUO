using System;

namespace Server.Items
{
    public class RelicFragment : Item
    {
        public override int LabelNumber { get { return 1031699; } } // Relic Fragment

        [Constructable]
        public RelicFragment()
            : this(1)
        {
        }

        [Constructable]
        public RelicFragment(int amount)
            : base(0x2DB3)
        {
            this.Stackable = true;
            this.Amount = amount;
            this.Weight = 1.0;
        }

        public RelicFragment(Serial serial)
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