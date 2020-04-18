namespace Server.Items
{
    public class RunedDriftwoodBow : Bow
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1149961;

        public override int InitMinHits => 255;
        public override int InitMaxHits => 255;

        [Constructable]
        public RunedDriftwoodBow()
        {
            Hue = 2955;
            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
            Attributes.LowerAmmoCost = 15;
        }

        public RunedDriftwoodBow(Serial serial)
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