using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishKryss))]
    [Flipable(0x1401, 0x1400)]
    public class Kryss : BaseSword
    {
        [Constructable]
        public Kryss()
            : base(0x1401)
        {
            Weight = 2.0;
        }

        public Kryss(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.InfectiousStrike;
        public override int StrengthReq => 10;
        public override int MinDamage => 10;
        public override int MaxDamage => 12;
        public override float Speed => 2.00f;

        public override int DefHitSound => 0x23C;
        public override int DefMissSound => 0x238;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;
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