namespace Server.Items
{
    public class StandardOfChaos : DoubleBladedStaff
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113522;  // Standard of Chaos

        [Constructable]
        public StandardOfChaos()
        {
            Hue = 2209;
            WeaponAttributes.HitHarm = 30;
            WeaponAttributes.HitFireball = 20;
            WeaponAttributes.HitLightning = 10;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = -40;
            Attributes.CastSpeed = 1;
            AosElementDamages.Chaos = 100;
        }

        public StandardOfChaos(Serial serial)
            : base(serial)
        {
        }

        //TODO: DoubleBladedSpear
        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}