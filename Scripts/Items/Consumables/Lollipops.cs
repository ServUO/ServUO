namespace Server.Items
{
    [TypeAlias("Server.Items.Lollipop")]
    public class Lollipops : CandyCane
    {
        [Constructable]
        public Lollipops()
            : this(1)
        {
        }

        [Constructable]
        public Lollipops(int amount)
            : base(0x468D + Utility.Random(3))
        {
            Stackable = true;
        }

        public Lollipops(Serial serial)
            : base(serial)
        {
        }

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