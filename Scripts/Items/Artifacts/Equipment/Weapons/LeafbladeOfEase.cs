namespace Server.Items
{
    public class LeafbladeOfEase : Leafblade
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LeafbladeOfEase()
        {
            WeaponAttributes.UseBestSkill = 1;
        }

        public LeafbladeOfEase(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073524;// leafblade of ease
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