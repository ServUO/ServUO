namespace Server.Items
{
    public class JackalsCollar : PlateGorget
    {
        public override bool IsArtifact => true;
        [Constructable]
        public JackalsCollar()
        {
            Hue = 0x6D1;
            Attributes.BonusDex = 15;
            Attributes.RegenHits = 2;
        }

        public JackalsCollar(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061594;// Jackal's Collar
        public override int ArtifactRarity => 11;
        public override int BaseFireResistance => 23;
        public override int BaseColdResistance => 17;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (version < 1)
            {
                if (Hue == 0x54B)
                    Hue = 0x6D1;

                FireBonus = 0;
                ColdBonus = 0;
            }
        }
    }
}