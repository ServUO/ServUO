namespace Server.Items
{
    public class LightInTheVoid : GargishTalwar
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113521;  // Light in the Void

        [Constructable]
        public LightInTheVoid()
        {
            Slayer = SlayerName.Silver;
            WeaponAttributes.HitLightning = 45;
            WeaponAttributes.HitLowerDefend = 30;
            Attributes.BonusStr = 8;
            Attributes.AttackChance = 10;
            Attributes.CastSpeed = 1;
            Attributes.WeaponSpeed = 20;
            Attributes.WeaponDamage = 35;
            Hue = 1072; //Hue not exact
        }

        public LightInTheVoid(Serial serial)
            : base(serial)
        {
        }

        public override int ArtifactRarity => 5;

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