namespace Server.Items
{
    public class WeddingTabletopBouquet : Item, IDyable
    {
        public override int LabelNumber => 1023127; // flowers

        [Constructable]
        public WeddingTabletopBouquet()
            : base(0x9EA3)
        {
            Weight = 1;
            LootType = LootType.Blessed;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public WeddingTabletopBouquet(Serial serial)
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
