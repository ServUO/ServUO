using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(BloodBlade))]
    [Flipable(0x2D22, 0x2D2E)]
    public class Leafblade : BaseKnife
    {
        [Constructable]
        public Leafblade()
            : base(0x2D22)
        {
            Weight = 8.0;
        }

        public Leafblade(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Feint;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ArmorIgnore;
        public override int StrengthReq => 20;
        public override int MinDamage => 11;
        public override int MaxDamage => 15;
        public override float Speed => 2.75f;

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