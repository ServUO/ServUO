namespace Server.Items
{
    [Flipable(0x4D0C, 0x4D0D)]
    public class TheTokenBox : BaseContainer
    {
        public override int LabelNumber => 1154209;  // The King's Collection

        public override int DefaultGumpID => 0x4D0C;

        [Constructable]
        public TheTokenBox()
            : base(0x4D0C)
        {
            Weight = 4.0;

            DropItem(new FifteenthAnniversaryLithographDeed());
            DropItem(new AlchemistsBookshelfDeed());
            DropItem(new BarrelSpongeDeed());
            DropItem(new BirdLampDeed());
            DropItem(new BullTapestryDeed());
            DropItem(new CactusSpongeDeed());
            DropItem(new CastlePaintingDeed());
            DropItem(new DragonLanternDeed());
            DropItem(new EmbroideredTapestryDeed());
            DropItem(new FirePaintingDeed());
            DropItem(new FluffySpongeDeed());
            DropItem(new FourPostBedDeed());
            DropItem(new GoldenTableDeed());
            DropItem(new HorsePaintingDeed());
            DropItem(new KoiLampDeed());
            DropItem(new MarbleTableDeed());
            DropItem(new MonasteryBellDeed());
            DropItem(new OrnateBedDeed());
            DropItem(new ShelfSpongeDeed());
            DropItem(new ShipPaintingDeed());
            DropItem(new TallLampDeed());
        }

        public TheTokenBox(Serial serial)
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
