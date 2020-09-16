using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualPointedSpear))]
    [Flipable(0x26BD, 0x26C7)]
    public class BladedStaff : BaseSpear
    {
        [Constructable]
        public BladedStaff()
            : base(0x26BD)
        {
            Weight = 4.0;
        }

        public BladedStaff(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 40;
        public override int MinDamage => 14;
        public override int MaxDamage => 17;
        public override float Speed => 3.00f;

        public override int InitMinHits => 21;
        public override int InitMaxHits => 110;
        public override SkillName DefSkill => SkillName.Swords;
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
