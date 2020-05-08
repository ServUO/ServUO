namespace Server.Items
{
    public class OrnateCrownOfTheHarrower : BoneHelm
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1061095;// Ornate Crown of the Harrower
        public override int ArtifactRarity => 11;
        public override int BasePoisonResistance => 17;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public OrnateCrownOfTheHarrower()
        {
            Hue = 0x4F6;
            Attributes.RegenHits = 2;
            Attributes.RegenStam = 3;
            Attributes.WeaponDamage = 25;
        }

        public OrnateCrownOfTheHarrower(Serial serial)
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
