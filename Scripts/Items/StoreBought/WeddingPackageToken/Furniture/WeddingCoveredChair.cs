namespace Server.Items
{
    [Furniture]
    [Flipable(0x9E8E, 0x9E8F, 0x9E90, 0x9E91)]
    public class WeddingCoveredChair : Item, IDyable
    {
        public override int LabelNumber => 1022895; // chair

        [Constructable]
        public WeddingCoveredChair()
            : base(0x9E8E)
        {
            Weight = 20;
            LootType = LootType.Blessed;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public WeddingCoveredChair(Serial serial)
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
