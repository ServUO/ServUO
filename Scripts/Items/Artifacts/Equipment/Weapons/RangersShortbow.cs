namespace Server.Items
{
    public class RangersShortbow : MagicalShortbow
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RangersShortbow()
        {
            Attributes.WeaponSpeed = 5;
        }

        public RangersShortbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073509;// ranger's shortbow
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