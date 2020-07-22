namespace Server.Items
{
    public class TorcOfTheGuardians : GoldNecklace
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113721;  // Torc of the Guardians

        [Constructable]
        public TorcOfTheGuardians()
        {
            Hue = 1837;
            Attributes.BonusInt = 5;
            Attributes.BonusStr = 5;
            Attributes.BonusDex = 5;
            Attributes.RegenStam = 2;
            Attributes.RegenMana = 2;
            Attributes.LowerManaCost = 5;
            Resistances.Physical = 5;
            Resistances.Fire = 5;
            Resistances.Cold = 5;
            Resistances.Poison = 5;
            Resistances.Energy = 5;
        }

        public TorcOfTheGuardians(Serial serial)
            : base(serial)
        {
        }

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

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
