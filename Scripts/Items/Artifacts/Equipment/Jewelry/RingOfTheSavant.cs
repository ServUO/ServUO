namespace Server.Items
{
    public class RingOfTheSavant : GoldRing
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RingOfTheSavant()
        {
            LootType = LootType.Blessed;
            Attributes.BonusInt = 3;
            Attributes.CastRecovery = 1;
            Attributes.CastSpeed = 1;
        }

        public RingOfTheSavant(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1077608;// Ring of the Savant
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