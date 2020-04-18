namespace Server.Items
{
    public class AnniversaryBag21st : Backpack
    {
        public override int LabelNumber => 1158481;  // 21st Anniversary Gift Bag

        [Constructable]
        public AnniversaryBag21st(Mobile m)
        {
            Hue = 1151;
            DropItem(new Anniversary21GiftToken());
            DropItem(new Anniversary21Card(m));
        }

        public AnniversaryBag21st(Serial serial)
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
