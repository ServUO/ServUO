using System;

namespace Server.Items
{
    [Flipable(0x232A, 0x232B)]
    public class WinterGiftPackage2003 : GiftBox
    {
        [Constructable]
        public WinterGiftPackage2003()
        {
            this.DropItem(new Snowman());
            this.DropItem(new WreathDeed());
            this.DropItem(new BlueSnowflake());
            this.DropItem(new RedPoinsettia());
        }

        public WinterGiftPackage2003(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}