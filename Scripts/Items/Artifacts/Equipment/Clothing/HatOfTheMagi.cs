namespace Server.Items
{
    public class HatOfTheMagi : WizardsHat
    {
        public override bool IsArtifact => true;
		public override int LabelNumber => 1061597;// Hat of the Magi
        public override int ArtifactRarity => 11;
        public override int BasePoisonResistance => 20;
        public override int BaseEnergyResistance => 20;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
		
        [Constructable]
        public HatOfTheMagi()
        {
            Hue = 0x481;
            Attributes.BonusInt = 8;
            Attributes.RegenMana = 4;
            Attributes.SpellDamage = 10;
        }

        public HatOfTheMagi(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(1);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}