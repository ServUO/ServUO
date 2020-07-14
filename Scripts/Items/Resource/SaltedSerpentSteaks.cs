namespace Server.Items
{
    public class SaltedSerpentSteaks : Item
    {
        public override int LabelNumber => 1159163; // salted serpent steak

        [Constructable]
        public SaltedSerpentSteaks()
            : base(0xA421)
        {
            Hue = 1150;
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
            reader.ReadInt();
        }
    }
}
