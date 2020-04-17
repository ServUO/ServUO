namespace Server.Items
{
    public class RaptorClaw : Boomerang
    {
        public override bool IsArtifact => true;
        [Constructable]
        public RaptorClaw()
        {
            Hue = 53;
            Slayer = SlayerName.Silver;
            Attributes.AttackChance = 12;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 35;
            WeaponAttributes.HitLeechStam = 40;
        }

        public RaptorClaw(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1112394; // Raptor Claw

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
