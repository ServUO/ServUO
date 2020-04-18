namespace Server.Items
{
    public class LegacyOfTheDreadLord : Bardiche
    {
        public override bool IsArtifact => true;
        [Constructable]
        public LegacyOfTheDreadLord()
        {
            Hue = 0x676;
            Attributes.SpellChanneling = 1;
            Attributes.CastRecovery = 3;
            Attributes.WeaponSpeed = 30;
            Attributes.WeaponDamage = 50;
        }

        public LegacyOfTheDreadLord(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1060860;// Legacy of the Dread Lord
        public override int ArtifactRarity => 10;
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