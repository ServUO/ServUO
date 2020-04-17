namespace Server.Items
{
    public class StoneSlithClaw : Cyclone
    {
        public override bool IsArtifact => true;
        [Constructable]
        public StoneSlithClaw()
        {
            Hue = 1150;
            WeaponAttributes.HitHarm = 40;
            Slayer = SlayerName.DaemonDismissal;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.WeaponSpeed = 25;
            Attributes.WeaponDamage = 45;
        }

        public StoneSlithClaw(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112393; // Stone Slith Claw

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
