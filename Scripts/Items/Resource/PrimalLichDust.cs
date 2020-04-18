namespace Server.Items
{
    public class PrimalLichDust : Item
    {
        [Constructable]
        public PrimalLichDust()
            : this(1)
        {
        }

        [Constructable]
        public PrimalLichDust(int amount)
            : base(0x2DB5)
        {
            Stackable = true;
            Amount = amount;
        }

        public PrimalLichDust(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1031701;// Primeval Lich Dust
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