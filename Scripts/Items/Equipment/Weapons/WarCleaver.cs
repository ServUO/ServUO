using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(Shortblade))]
    [Flipable(0x2D2F, 0x2D23)]
    public class WarCleaver : BaseKnife
    {
        [Constructable]
        public WarCleaver()
            : base(0x2D2F)
        {
            Weight = 10.0;
            Layer = Layer.TwoHanded;
        }

        public WarCleaver(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Disarm;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Bladeweave;
        public override int StrengthReq => 15;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.25f;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x239;
        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;
        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(1); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
