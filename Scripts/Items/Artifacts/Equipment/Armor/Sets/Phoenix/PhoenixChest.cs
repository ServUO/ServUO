namespace Server.Items
{
    public class PhoenixChest : RingmailChest
    {
        [Constructable]
        public PhoenixChest()
        {
            Hue = 0x8E;
            LootType = LootType.Blessed;
        }

        public PhoenixChest(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1041606;// ringmail tunic of the phoenix

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