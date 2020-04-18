namespace Server.Items
{
    public class PendantOfTheMagi : GoldNecklace
    {
        public override bool IsArtifact => true;
        [Constructable]
        public PendantOfTheMagi()
        {
            Hue = 0x48D;
            Attributes.BonusInt = 10;
            Attributes.RegenMana = 3;
            Attributes.SpellDamage = 5;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 30;
        }

        public PendantOfTheMagi(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1072937;// Pendant of the Magi
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();
        }
    }
}