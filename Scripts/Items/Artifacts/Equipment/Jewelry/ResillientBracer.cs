namespace Server.Items
{
    public class ResilientBracer : GoldBracelet
    {
        public override bool IsArtifact => true;
        [Constructable]
        public ResilientBracer()
        {
            Hue = 0x488;
            SkillBonuses.SetValues(0, SkillName.MagicResist, 15.0);
            Attributes.BonusHits = 5;
            Attributes.RegenHits = 2;
            Attributes.DefendChance = 10;
        }

        public ResilientBracer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072933;// Resillient Bracer
        public override int PhysicalResistance => 20;
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