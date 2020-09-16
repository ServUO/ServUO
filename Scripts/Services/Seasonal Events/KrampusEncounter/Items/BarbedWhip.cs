namespace Server.Items
{
    public class BarbedWhip : BaseBashing, Engines.Craft.IRepairable
    {
        public Engines.Craft.CraftSystem RepairSystem => Engines.Craft.DefTinkering.CraftSystem;
        public override int LabelNumber => 1125641;  // Barbed Whip		

        [Constructable]
        public BarbedWhip()
            : base(0xA289)
        {
            Weight = 5.0;
        }

        public BarbedWhip(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ConcussionBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.WhirlwindAttack;
        public override int StrengthReq => 20;
        public override int MinDamage => 13;
        public override int MaxDamage => 17;
        public override float Speed => 3.25f;
        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;

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
