namespace Server.Items
{
    public class AnniversaryBag18th : Bag
    {
        public override int LabelNumber => 1156141;  // 18th Anniversary Gift Bag

        [Constructable]
        public AnniversaryBag18th()
            : this(null)
        {
        }

        [Constructable]
        public AnniversaryBag18th(Mobile m)
        {
            Hue = 1164;
            DropItem(new RecipeScroll(Utility.RandomBool() ? 701 : 702));
            DropItem(new AnniversaryPlate(m));
            DropItem(new EnchantedTimepiece());
            DropItem(new AnniversaryCard(m));
        }

        public AnniversaryBag18th(Serial serial)
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