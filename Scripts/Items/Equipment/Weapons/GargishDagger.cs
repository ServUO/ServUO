namespace Server.Items
{
    [Flipable(0x902, 0x406A)]
    public class GargishDagger : BaseKnife
    {
        [Constructable]
        public GargishDagger()
            : base(0x902)
        {
        }

        public GargishDagger(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ShadowStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.InfectiousStrike;
        public override int StrengthReq => 10;
        public override int MinDamage => 10;
        public override int MaxDamage => 12;
        public override float Speed => 2.00f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 40;
        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;

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
