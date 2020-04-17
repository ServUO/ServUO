namespace Server.Items
{
    [TypeAlias("Server.Items.ElvenFletchings")]
    public class ElvenFletching : Item, ICommodity
    {
        [Constructable]
        public ElvenFletching()
            : this(1)
        {
        }

        [Constructable]
        public ElvenFletching(int amount)
            : base(0x5737)
        {
            Stackable = true;
            Amount = amount;
        }

        public ElvenFletching(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113346;// elven fletching
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
