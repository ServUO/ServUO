namespace Server.Items
{
    public class BreathOfTheDead : BoneHarvester
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BreathOfTheDead()
        {
            Hue = 0x455;
            WeaponAttributes.HitLeechHits = 100;
            WeaponAttributes.HitHarm = 25;
            Attributes.SpellDamage = 5;
            Attributes.WeaponDamage = 50;
        }

        public BreathOfTheDead(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1061109;// Breath of the Dead
        public override int ArtifactRarity => 11;
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
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