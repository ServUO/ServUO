namespace Server.Items
{
    public class ChagaMushroom : Item, ICommodity
    {
        [Constructable]
        public ChagaMushroom()
            : this(1)
        {
        }

        [Constructable]
        public ChagaMushroom(int amount)
            : base(0x5743)
        {
            Stackable = true;
            Amount = amount;
        }

        public ChagaMushroom(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113356;// chaga mushroom
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
