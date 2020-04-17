namespace Server.Items
{
    public class LyricalGlasses : ElvenGlasses
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LyricalGlasses()
        {
            WeaponAttributes.HitLowerDefend = 20;
            Attributes.NightSight = 1;
            Attributes.ReflectPhysical = 15;
            Hue = 0x47F;
        }

        public LyricalGlasses(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1073382;//Lyrical Reading Glasses
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
                Hue = 0x47F;
        }
    }
}