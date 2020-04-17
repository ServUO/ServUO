namespace Server.Items
{
    public class SoldiersMedal : BaseTalisman
    {
        public override bool IsArtifact => true;

        [Constructable]
        public SoldiersMedal()
            : base(0x2F5B)
        {
            Hue = 1902;
            SkillBonuses.SetValues(0, SkillName.Tactics, 10.0);
            Attributes.AttackChance = 5;
            Attributes.RegenStam = 2;
            Attributes.WeaponDamage = 20;
        }

        public SoldiersMedal(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber => 1154726;// Soldier's Medal
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
