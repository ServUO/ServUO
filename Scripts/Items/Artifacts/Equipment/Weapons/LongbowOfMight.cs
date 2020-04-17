namespace Server.Items
{
    public class LongbowOfMight : ElvenCompositeLongbow
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LongbowOfMight()
        {
            Attributes.WeaponDamage = 5;
        }

        public LongbowOfMight(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073508;// longbow of might
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