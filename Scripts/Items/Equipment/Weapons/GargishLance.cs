namespace Server.Items
{
    [Flipable(0x48CA, 0x48CB)]
    public class GargishLance : BaseSword
    {
        [Constructable]
        public GargishLance()
            : base(0x48CA)
        {
            Weight = 12.0;
        }

        public GargishLance(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Dismount;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;
        public override int StrengthReq => 95;
        public override int MinDamage => 18;
        public override int MaxDamage => 22;
        public override float Speed => 4.25f;

        public override int DefHitSound => 0x23C;
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
