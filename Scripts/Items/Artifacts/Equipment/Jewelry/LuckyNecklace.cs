namespace Server.Items
{
    public class LuckyNecklace : BaseJewel
    {
        public override int LabelNumber => 1075239;  //Lucky Necklace

        [Constructable]
        public LuckyNecklace()
            : base(0x1088, Layer.Neck)
        {
            Hue = 1150;
            Attributes.Luck = 200;
            LootType = LootType.Blessed;
        }

        public LuckyNecklace(Serial serial)
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
