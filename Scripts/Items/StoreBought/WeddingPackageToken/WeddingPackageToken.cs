namespace Server.Items
{
    public class WeddingPackageToken : Item
    {
        public override int LabelNumber => 1157339; // Wedding Package Token

        [Constructable]
        public WeddingPackageToken()
            : base(0x2AAA)
        {
            Weight = 5;
            LootType = LootType.Blessed;
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1062334); // This item must be in your backpack to be used.
            }
            else
            {
                var chest = new WoodenChest();

                chest.DropItem(new MagicalWeddingSewingKit());
                chest.DropItem(new MagicalWeddingSewingKit());
                chest.DropItem(new MagicalCakeBatter());
                chest.DropItem(new ChocolateFountain());
                chest.DropItem(new MagicalBotanicalArchwaySeed());
                chest.DropItem(new WeddingShortBuffetTableDeed());
                chest.DropItem(new WeddingShortBuffetTableDeed());
                chest.DropItem(new WeddingLongBuffetTableDeed());                

                var tablebag = new Bag();
                tablebag.DropItem(new WeddingCocktailTable());
                tablebag.DropItem(new WeddingCocktailTable());
                tablebag.DropItem(new WeddingCocktailTable());
                tablebag.DropItem(new WeddingCocktailTable());
                tablebag.DropItem(new WeddingCocktailTable());
                tablebag.DropItem(new WeddingCocktailTable());
                chest.DropItem(tablebag);

                var covchairbag = new Bag();
                covchairbag.DropItem(new WeddingCoveredChair());
                covchairbag.DropItem(new WeddingCoveredChair());
                covchairbag.DropItem(new WeddingCoveredChair());
                covchairbag.DropItem(new WeddingCoveredChair());
                covchairbag.DropItem(new WeddingCoveredChair());
                covchairbag.DropItem(new WeddingCoveredChair());
                chest.DropItem(covchairbag);

                var folchairbag = new Bag();
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                folchairbag.DropItem(new WeddingFoldingChair());
                chest.DropItem(folchairbag);

                var decobag = new Bag();
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingTabletopBouquet());
                decobag.DropItem(new WeddingStandingBouquet());
                decobag.DropItem(new WeddingStandingBouquet());
                decobag.DropItem(new WeddingStandingBouquet());
                decobag.DropItem(new WeddingStandingBouquet());
                decobag.DropItem(new WeddingFancyCandelabra());
                decobag.DropItem(new WeddingFancyCandelabra());
                decobag.DropItem(new WeddingRegularCandelabra());
                decobag.DropItem(new WeddingRegularCandelabra());
                chest.DropItem(decobag);

                from.AddToBackpack(chest);

                Delete();
            }
        }

        public WeddingPackageToken(Serial serial)
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
