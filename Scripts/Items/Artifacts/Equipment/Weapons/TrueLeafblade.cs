namespace Server.Items
{
    public class TrueLeafblade : Leafblade
    {
        public override bool IsArtifact => true;
        [Constructable]
        public TrueLeafblade()
        {
            WeaponAttributes.ResistPoisonBonus = 5;
        }

        public TrueLeafblade(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073521;// true leafblade
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