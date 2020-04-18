namespace Server.Items
{
    [TypeAlias("Server.Items.LavaSerpenCrust")]
    public class LavaSerpentCrust : Item, ICommodity
    {
        [Constructable]
        public LavaSerpentCrust()
            : this(1)
        {
        }

        [Constructable]
        public LavaSerpentCrust(int amount)
            : base(0x572D)
        {
            Stackable = true;
            Amount = amount;
        }

        public LavaSerpentCrust(Serial serial)
            : base(serial)
        {
        }

        TextDefinition ICommodity.Description => LabelNumber;
        bool ICommodity.IsDeedable => true;

        public override int LabelNumber => 1113336;// lava serpent crust
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
