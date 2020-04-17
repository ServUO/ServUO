namespace Server.Items
{
    public class OrcishMachete : ElvenMachete
    {
        public override bool IsArtifact => true;
        [Constructable]
        public OrcishMachete()
        {
            Attributes.BonusInt = -5;
            Attributes.WeaponDamage = 10;
        }

        public OrcishMachete(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073534;// Orcish Machete
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