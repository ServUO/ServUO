using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(40607, 40608, 40609, 40610)]
    public class WeddingChair : Item
    {
        [Constructable]
        public WeddingChair()
            : base(40607)
        {
            Name = "Wedding Chair";
			this.Weight = 10.0;
        }

        public WeddingChair(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 10.0;
        }
    }

    [Furniture]
    [Flipable(40590, 40591, 40592, 40593)]
    public class CoveredWeddingChair : Item
    {
        [Constructable]
        public CoveredWeddingChair()
            : base(40590)
        {
            Name = "Wedding Chair";
			this.Weight = 10.0;
        }

        public CoveredWeddingChair(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 6.0)
                this.Weight = 10.0;
        }
    }
}