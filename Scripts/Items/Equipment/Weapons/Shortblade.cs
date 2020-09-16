namespace Server.Items
{
    [Flipable(0x907, 0x4076)]
    public class Shortblade : BaseSword
    {
        [Constructable]
        public Shortblade()
            : base(0x907)
        {
            //Weight = 9.0;
        }

        public Shortblade(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 45;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.25f;

        public override int DefHitSound => 0x236;
        public override int DefMissSound => 0x238;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;
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
