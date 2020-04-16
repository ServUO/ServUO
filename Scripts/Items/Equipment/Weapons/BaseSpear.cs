namespace Server.Items
{
    public abstract class BaseSpear : BaseMeleeWeapon
    {
        public BaseSpear(int itemID)
            : base(itemID)
        {
        }

        public BaseSpear(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound => 0x23C;
        public override int DefMissSound => 0x238;

        public override SkillName DefSkill => SkillName.Fencing;

        public override WeaponType DefType => WeaponType.Piercing;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce2H;

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