namespace Server.Items
{
    [Flipable(0x232A, 0x232B)]
    public class WinterGiftPackage2003 : GiftBox
    {
        [Constructable]
        public WinterGiftPackage2003()
        {
            DropItem(new Snowman());
            DropItem(new WreathDeed());
            DropItem(new BlueSnowflake());
            DropItem(new RedPoinsettia());
        }

        public WinterGiftPackage2003(Serial serial)
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