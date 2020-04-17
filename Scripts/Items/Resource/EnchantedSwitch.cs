namespace Server.Items
{
    public class EnchantedSwitch : Item
    {
        [Constructable]
        public EnchantedSwitch()
            : base(0x2F5C)
        {
            Weight = 1.0;
        }

        public EnchantedSwitch(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072893;// enchanted switch
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