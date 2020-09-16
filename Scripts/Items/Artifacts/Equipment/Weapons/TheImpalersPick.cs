namespace Server.Items
{
    [Flipable(0x143D, 0x143C)]
    public class TheImpalersPick : HammerPick
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113822;  // The Impaler's Pick

        [Constructable]
        public TheImpalersPick()
        {
            Hue = 2101;
            WeaponAttributes.HitManaDrain = 10;
            Slayer = SlayerName.Repond;
            WeaponAttributes.HitLightning = 40;
            WeaponAttributes.HitLowerDefend = 40;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 45;
        }

        public TheImpalersPick(Serial serial)
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