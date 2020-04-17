namespace Server.Items
{
    public class Matchcord : Item, ICommodity
    {
        public override int LabelNumber => 1095184;

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        [Constructable]
        public Matchcord() : this(1) { }

        [Constructable]
        public Matchcord(int amount)
            : base(5153)
        {
            Hue = 1171;
            Stackable = true;
            Amount = amount;
        }

        public Matchcord(Serial serial) : base(serial) { }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}