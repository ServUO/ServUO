namespace Server.Items
{
    public class MysticsGuard : GargishWoodenShield
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113536;
        public override int ArtifactRarity => 5;
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 0;
        public override int BaseColdResistance => 0;
        public override int BasePoisonResistance => 0;
        public override int BaseEnergyResistance => 1;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public MysticsGuard()
            : base()
        {
            ArmorAttributes.SoulCharge = 30;
            Attributes.SpellChanneling = 1;
            Attributes.DefendChance = 10;
            Attributes.CastRecovery = 2;
            Hue = 0x671;
        }

        public MysticsGuard(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);//version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
