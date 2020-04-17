namespace Server.Items
{
    public class AncientWildStaff : WildStaff
    {
        public override bool IsArtifact => true;
        [Constructable]
        public AncientWildStaff()
        {
            WeaponAttributes.ResistPoisonBonus = 5;
        }

        public AncientWildStaff(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073550;// ancient wild staff
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