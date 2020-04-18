namespace Server.Items
{
    public class CrystalShards : Item, ICommodity
    {
        [Constructable]
        public CrystalShards()
            : this(1)
        {
        }

        [Constructable]
        public CrystalShards(int amount)
            : base(0x5738)
        {
            Stackable = true;
            Amount = amount;
        }

        public CrystalShards(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113347;// crystal shards
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
