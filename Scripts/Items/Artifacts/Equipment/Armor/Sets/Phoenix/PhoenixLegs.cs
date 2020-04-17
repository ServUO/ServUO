namespace Server.Items
{
    public class PhoenixLegs : RingmailLegs
    {
        [Constructable]
        public PhoenixLegs()
        {
            Hue = 0x8E;
            LootType = LootType.Blessed;
        }

        public PhoenixLegs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041608;// ringmail leggings of the phoenix

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}