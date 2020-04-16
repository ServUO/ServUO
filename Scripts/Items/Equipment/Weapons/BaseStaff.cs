namespace Server.Items
{
    public abstract class BaseStaff : BaseMeleeWeapon
    {
        public BaseStaff(int itemID)
            : base(itemID)
        {
        }

        public BaseStaff(Serial serial)
            : base(serial)
        {
        }

        public override int DefHitSound => 0x233;
        public override int DefMissSound => 0x239;

        public override SkillName DefSkill => SkillName.Macing;

        public override WeaponType DefType => WeaponType.Staff;

        public override WeaponAnimation DefAnimation => WeaponAnimation.Bash2H;

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

        public override void OnHit(Mobile attacker, IDamageable defender, double damageBonus)
        {
            base.OnHit(attacker, defender, damageBonus);

            if (defender is Mobile)
                ((Mobile)defender).Stam -= Utility.Random(3, 5); // 3-5 points of stamina loss
        }
    }
}