namespace Server.Items
{
    public class NecromancersPhylactery : BaseTalisman
    {
        public override bool IsArtifact => true;

        [Constructable]
        public NecromancersPhylactery()
            : base(0x2F5A)
        {
            Hue = 1912;
            SkillBonuses.SetValues(0, SkillName.SpiritSpeak, 10.0);
            Attributes.RegenMana = 1;
            Attributes.LowerRegCost = 10;
            Attributes.SpellDamage = 5;
        }

        public NecromancersPhylactery(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154728;// Necromancer's Phylactery
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