namespace Server.Items
{
    public class WeddingStandingBouquet : Item, IDyable
    {
        public override int LabelNumber => 1023127; // flowers

        [Constructable]
        public WeddingStandingBouquet()
            : base(0x9EA4)
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

        public WeddingStandingBouquet(Serial serial)
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
