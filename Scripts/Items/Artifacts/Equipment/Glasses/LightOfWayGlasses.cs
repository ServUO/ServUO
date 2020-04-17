namespace Server.Items
{
    public class LightOfWayGlasses : ElvenGlasses
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LightOfWayGlasses()
        {
            Attributes.BonusStr = 7;
            Attributes.BonusInt = 5;
            Attributes.WeaponDamage = 30;
            Hue = 0x256;
        }

        public LightOfWayGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073378;//Light Of Way Reading Glasses
        public override int BasePhysicalResistance => 10;
        public override int BaseFireResistance => 10;
        public override int BaseColdResistance => 10;
        public override int BasePoisonResistance => 10;
        public override int BaseEnergyResistance => 10;
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

            if (version == 0 && Hue == 0)
                Hue = 0x256;
        }
    }
}