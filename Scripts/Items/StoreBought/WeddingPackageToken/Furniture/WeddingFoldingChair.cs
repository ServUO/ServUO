namespace Server.Items
{
    [Furniture]
    [Flipable(0x9E9F, 0x9EA0, 0x9EA1, 0x9EA2)]
    public class WeddingFoldingChair : Item, IDyable
    {
        public override int LabelNumber => 1022895; // chair

        [Constructable]
        public WeddingFoldingChair()
            : base(0x9E9F)
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

        public WeddingFoldingChair(Serial serial)
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
