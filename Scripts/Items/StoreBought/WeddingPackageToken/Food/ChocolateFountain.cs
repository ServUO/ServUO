namespace Server.Items
{
    public class ChocolateFountain : Item
    {
        public override int LabelNumber => 1124655; // Chocolate Fountain

        [Constructable]
        public ChocolateFountain()
            : base(0x9EBF)
        {
            Weight = 10;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.AddToBackpack(new ChocolateStrawberry());
        }

        public ChocolateFountain(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }

    public class ChocolateStrawberry : Food
    {
        public override int LabelNumber => 1124664; // Chocolate Covered Strawberry

        [Constructable]
        public ChocolateStrawberry()
            : this(1)
        {
        }

        [Constructable]
        public ChocolateStrawberry(int amount)
            : base(0x9EC0)
        {
            Amount = amount;
            Stackable = true;
            FillFactor = 1;
        }

        public ChocolateStrawberry(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
