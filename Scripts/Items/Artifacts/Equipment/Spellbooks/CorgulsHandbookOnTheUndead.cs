namespace Server.Items
{
    public class CorgulsHandbookOnTheUndead : NecromancerSpellbook
    {
        public override int LabelNumber => 1149780;

        [Constructable]
        public CorgulsHandbookOnTheUndead()
        {
            Hue = 2953;
            Attributes.RegenMana = 3;
            Attributes.DefendChance = 5;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 20;
        }

        public CorgulsHandbookOnTheUndead(Serial serial)
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