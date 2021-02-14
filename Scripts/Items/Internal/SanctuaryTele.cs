namespace Server.Items
{
    public class SanctuaryTele : Teleporter
    {
        [Constructable]
        public SanctuaryTele()
            : base(new Point3D(6172, 22, 0), Map.Trammel)
        {
        }

        public SanctuaryTele(Serial serial)
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
            reader.ReadInt();
        }
    }
}
