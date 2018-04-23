using System;

namespace Server.Items
{
    [Furniture]
    public class WeddingFlowers : Item
    {
        [Constructable]
        public WeddingFlowers()
            : base(40611)
        {
            Name = "Wedding Flowers";
			this.Weight = 10.0;
        }

        public WeddingFlowers(Serial serial)
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
    public class WeddingFlowersStanding : Item
    {
        [Constructable]
        public WeddingFlowersStanding()
            : base(40612)
        {
            Name = "Wedding Flowers";
			this.Weight = 10.0;
        }

        public WeddingFlowersStanding(Serial serial)
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