namespace Server.Items
{
    public class TokenOfHolyFavor : GoldBracelet
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113652;  // Token of Holy Favor

        [Constructable]
        public TokenOfHolyFavor()
        {
            Hue = 96;
            Attributes.BonusHits = 5;
            Attributes.CastRecovery = 2;
            Attributes.CastSpeed = 1;
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;
            Attributes.SpellDamage = 4;
            Resistances.Cold = 5;
            Resistances.Poison = 5;
        }

        public TokenOfHolyFavor(Serial serial)
            : base(serial)
        {
        }

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