namespace Server.Items
{
    public class SpikedWhip : BaseSword, Engines.Craft.IRepairable
    {
        public Engines.Craft.CraftSystem RepairSystem => Engines.Craft.DefTinkering.CraftSystem;
        public override int LabelNumber => 1125634;  // Spiked Whip

        [Constructable]
        public SpikedWhip()
            : base(0xA292)
        {
            Weight = 5.0;
        }

        public SpikedWhip(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorPierce;
        public override WeaponAbility SecondaryAbility => WeaponAbility.WhirlwindAttack;
        public override int StrengthReq => 20;
        public override int MinDamage => 13;
        public override int MaxDamage => 17;
        public override float Speed => 3.25f;
        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;
        public override SkillName DefSkill => SkillName.Fencing;
        public override WeaponType DefType => WeaponType.Piercing;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Bash1H;

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
