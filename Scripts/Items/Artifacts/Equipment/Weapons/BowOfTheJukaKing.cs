namespace Server.Items
{
    public class BowOfTheJukaKing : Bow
    {
        public override bool IsArtifact => true;
        [Constructable]
        public BowOfTheJukaKing()
        {
            Hue = 0x460;
            WeaponAttributes.HitMagicArrow = 25;
            Slayer = SlayerName.ReptilianDeath;
            Attributes.AttackChance = 15;
            Attributes.WeaponDamage = 40;
        }

        public BowOfTheJukaKing(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1070636;
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