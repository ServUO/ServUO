using Server.Engines.Craft;
using Server.Engines.Harvest;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DiscMace))]
    [Flipable(0x13B0, 0x13AF)]
    public class WarAxe : BaseAxe
    {
        [Constructable]
        public WarAxe()
            : base(0x13B0)
        {
            Weight = 8.0;
        }

        public WarAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.BleedAttack;
        public override int StrengthReq => 35;
        public override int MinDamage => 12;
        public override int MaxDamage => 16;
        public override float Speed => 3.00f;

        public override int DefHitSound => 0x233;
        public override int DefMissSound => 0x239;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;
        public override SkillName DefSkill => SkillName.Macing;
        public override WeaponType DefType => WeaponType.Bashing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Bash1H;
        public override HarvestSystem HarvestSystem => null;
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
