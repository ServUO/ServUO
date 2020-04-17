namespace Server.Items
{
    [TypeAlias("Server.Items.SeedRenewal")]
    public class SeedOfRenewal : Item, ICommodity
    {
        [Constructable]
        public SeedOfRenewal()
            : this(1)
        {
        }

        [Constructable]
        public SeedOfRenewal(int amount)
            : base(0x5736)
        {
            Stackable = true;
            Amount = amount;
        }

        public SeedOfRenewal(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113345;// seed of renewal
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}
