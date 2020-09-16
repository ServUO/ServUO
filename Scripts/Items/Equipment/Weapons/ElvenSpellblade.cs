using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualPointedSpear))]
    [Flipable(0x2D20, 0x2D2C)]
    public class ElvenSpellblade : BaseKnife
    {
        [Constructable]
        public ElvenSpellblade()
            : base(0x2D20)
        {
            Weight = 5.0;
            Layer = Layer.TwoHanded;
        }

        public ElvenSpellblade(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.PsychicAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.BleedAttack;
        public override int StrengthReq => 35;
        public override int MinDamage => 12;
        public override int MaxDamage => 15;
        public override float Speed => 2.50f;

        public override int DefMissSound => 0x239;
        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;
        public override SkillName DefSkill => SkillName.Fencing;
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
