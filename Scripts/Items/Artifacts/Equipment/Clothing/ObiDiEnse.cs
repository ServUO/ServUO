namespace Server.Items
{
    [Flipable(0x1515, 0x1530)]
    public class ObiDiEnse : Obi
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ObiDiEnse()
            : base(0x27A0)
        {
            Hue = 0;
            Attributes.BonusInt = 5;
            Attributes.NightSight = 1;
            SkillBonuses.SetValues(0, SkillName.Focus, 5.0);
        }

        public ObiDiEnse(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112406;// Obi Di Ense [Replica]

        public override int InitMinHits => 150;
        public override int InitMaxHits => 150;
        public override bool CanFortify => false;

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}