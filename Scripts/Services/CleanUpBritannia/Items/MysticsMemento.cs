namespace Server.Items
{
    public class MysticsMemento : BaseTalisman
    {
        public override bool IsArtifact => true;

        [Constructable]
        public MysticsMemento()
            : base(0x2F5B)
        {
            this.Hue = 1912;
            this.SkillBonuses.SetValues(0, SkillName.Focus, 10.0);
            this.Attributes.RegenMana = 1;
            this.Attributes.LowerRegCost = 10;
            this.Attributes.SpellDamage = 5;
        }

        public MysticsMemento(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154730;// Mystic's Memento
        public override bool ForceShowName => true;
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