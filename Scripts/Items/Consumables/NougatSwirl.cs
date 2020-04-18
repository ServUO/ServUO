namespace Server.Items
{
    public class NougatSwirl : CandyCane
    {
        [Constructable]
        public NougatSwirl()
            : this(1)
        {
        }

        [Constructable]
        public NougatSwirl(int amount)
            : base(0x4690)
        {
            Stackable = true;
            Amount = amount;
        }

        public NougatSwirl(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1096936;/* nougat swirl */
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
