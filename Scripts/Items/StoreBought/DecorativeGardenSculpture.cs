namespace Server.Items
{
    [Flipable(0xA565, 0xA566)]
    public class DecorativeGardenSculpture : Item, IDyable
    {
        public override string DefaultName => "Garden Sculpture";

        [Constructable]
        public DecorativeGardenSculpture()
            : base(0xA565)
        {
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public DecorativeGardenSculpture(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            reader.ReadInt();
        }
    }
}
