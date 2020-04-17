namespace Server.Items
{
    public class SerratedWarCleaver : WarCleaver
    {
        public override int LabelNumber => 1073527; // serrated war cleaver

        [Constructable]
        public SerratedWarCleaver()
        {
            Attributes.WeaponDamage = 7;
        }

        public SerratedWarCleaver(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}