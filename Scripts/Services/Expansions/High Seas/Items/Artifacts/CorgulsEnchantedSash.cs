namespace Server.Items
{
    public class CorgulsEnchantedSash : BodySash
    {
        public override int LabelNumber => 1149781;

        [Constructable]
        public CorgulsEnchantedSash()
        {
            Attributes.BonusStam = 1;
            Attributes.DefendChance = 5;
        }

        public CorgulsEnchantedSash(Serial serial)
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
            int version = reader.ReadInt();
        }
    }
}