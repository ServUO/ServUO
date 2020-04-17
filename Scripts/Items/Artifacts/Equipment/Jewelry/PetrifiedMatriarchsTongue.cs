namespace Server.Items
{
    public class PetrifiedMatriarchsTongue : GoldRing
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1115776;  // Petrified Matriarch's Tongue

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public PetrifiedMatriarchsTongue()
        {
            Hue = 2006; //TODO: get proper hue, this is a guess
            Attributes.RegenMana = 2;
            Attributes.AttackChance = 10;
            Attributes.CastSpeed = 1;
            Attributes.CastRecovery = 2;
            Attributes.LowerManaCost = 4;
            Resistances.Poison = 5;
        }

        public PetrifiedMatriarchsTongue(Serial serial)
            : base(serial)
        {
        }

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