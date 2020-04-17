namespace Server.Items
{
    public class DuelistsEdge : BaseTalisman
    {
        public override bool IsArtifact => true;

        [Constructable]
        public DuelistsEdge()
            : base(0x2F58)
        {
            Hue = 1902;
            SkillBonuses.SetValues(0, SkillName.Anatomy, 10.0);
            Attributes.RegenStam = 2;
            Attributes.AttackChance = 5;
            Attributes.WeaponDamage = 20;
        }

        public DuelistsEdge(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154727;// Duelist's Edge
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
