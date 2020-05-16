namespace Server.Items
{
    public class SaltedSerpentSteaks : FishSteak
    {
        public override int LabelNumber => 1159163; // salted serpent steak

        [Constructable]
        public SaltedSerpentSteaks()
            : this(1)
        {
        }

        [Constructable]
        public SaltedSerpentSteaks(int amount)
            : base(amount)
        {
        }

        public SaltedSerpentSteaks(Serial serial)
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
