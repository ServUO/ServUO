using System;

// Other fur Hues: green: 58, Red: 1541

namespace Server.Items
{
    public class BouraFur : Item
    {
        [Constructable]
        public BouraFur()
            : this(1)
        {
        }

        [Constructable]
        public BouraFur(int amount)
            : base(6261)
        {
            Stackable = true;
            Amount = amount;

            Hue = 153;
        }

        public BouraFur(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 1;
            }
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

    public class KepetchFur : Item
    {
        [Constructable]
        public KepetchFur()
            : this(1)
        {
        }

        [Constructable]
        public KepetchFur(int amount)
            : base(6261)
        {
            Stackable = true;
            Amount = amount;

            Hue = 58;
        }

        public KepetchFur(Serial serial)
            : base(serial)
        {
        }

        public override double DefaultWeight
        {
            get
            {
                return 1;
            }
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