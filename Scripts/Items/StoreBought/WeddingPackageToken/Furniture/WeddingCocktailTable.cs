namespace Server.Items
{
    [Furniture]
    public class WeddingCocktailTable : Item, IDyable
    {
        public override int LabelNumber => 1022869; // table

        [Constructable]
        public WeddingCocktailTable()
            : base(0x9E8D)
        {
            Weight = 5;
            LootType = LootType.Blessed;
        }

        public bool Dye(Mobile from, DyeTub sender)
        {
            if (Deleted)
                return false;

            Hue = sender.DyedHue;

            return true;
        }

        public WeddingCocktailTable(Serial serial)
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
