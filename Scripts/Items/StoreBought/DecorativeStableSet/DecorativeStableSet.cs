namespace Server.Items
{
    public class DecorativeStableSet : Backpack
    {
        public override int LabelNumber => 1159272;  // Decorative Stable Set

        [Constructable]
        public DecorativeStableSet()
        {
            DropItem(new CowStatue());
            DropItem(new HorseStatue());
            DropItem(new ChickenStatue());
            DropItem(new MetalTubDeed());
            DropItem(new Feedbag());
            DropItem(new CowPie());

            var box = new WoodenBox();

            var item = new DecorativeStableFencing(FencingType.CornerPiece);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.EastFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.SouthFacingPieces);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.NWCornerPiece);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.NWCornerPiece);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.GateSouth);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.GateSouth);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.GateEast);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.GateEast);
            box.DropItem(item);

            item = new DecorativeStableFencing(FencingType.Arch);
            box.DropItem(item);

            DropItem(box);
        }


        public DecorativeStableSet(Serial serial)
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
