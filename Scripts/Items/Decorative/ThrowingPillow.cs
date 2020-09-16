namespace Server.Items
{
    public class ThrowPillow : Item, IDyable
    {
        public override int LabelNumber => 1075496;  // Throw Pillow

        [Constructable]
        public ThrowPillow()
            : base(0x1944)
        {
            LootType = LootType.Blessed;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public ThrowPillow(Serial serial)
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
