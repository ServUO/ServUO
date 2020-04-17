namespace Server.Items
{
    public class DeathsHead : DiscMace
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113526;  // Death's Head

        [Constructable]
        public DeathsHead()
        {
            Hue = 1154;
            WeaponAttributes.HitFatigue = 10;
            WeaponAttributes.HitLightning = 45;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 45;
        }

        public DeathsHead(Serial serial)
            : base(serial)
        {
        }

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
