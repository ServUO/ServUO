using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(Shortblade))]
    [Flipable(0x2D21, 0x2D2D)]
    public class AssassinSpike : BaseKnife
    {
        [Constructable]
        public AssassinSpike()
            : base(0x2D21)
        {
            Weight = 4.0;
        }

        public AssassinSpike(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.InfectiousStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ShadowStrike;

        public override int StrengthReq => 15;

        public override int MinDamage => 10;
        public override int MaxDamage => 12;

        public override float Speed => 2.00f;

        public override int DefMissSound => 0x239;

        public override SkillName DefSkill => SkillName.Fencing;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;

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