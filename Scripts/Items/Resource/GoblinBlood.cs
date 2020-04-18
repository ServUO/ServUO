namespace Server.Items
{
    public class GoblinBlood : Item, ICommodity
    {
        [Constructable]
        public GoblinBlood()
            : this(1)
        {
        }

        [Constructable]
        public GoblinBlood(int amount)
            : base(0x572C)
        {
            Stackable = true;
            Amount = amount;
        }

        public GoblinBlood(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113335;// goblin blood
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
