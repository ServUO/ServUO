namespace Server.Items
{
    public class UndyingFlesh : Item, ICommodity
    {
        [Constructable]
        public UndyingFlesh()
            : this(1)
        {
        }

        [Constructable]
        public UndyingFlesh(int amount)
            : base(0x5731)
        {
            Stackable = true;
            Amount = amount;
        }

        public UndyingFlesh(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113337;// undying flesh
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
