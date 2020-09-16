namespace Server.Items
{
    [Flipable(0x8FD, 0x4068)]
    public class DualShortAxes : BaseAxe
    {
        [Constructable]
        public DualShortAxes()
            : base(0x8FD)
        {
        }

        public DualShortAxes(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.InfectiousStrike;
        public override int StrengthReq => 35;
        public override int MinDamage => 14;
        public override int MaxDamage => 17;
        public override float Speed => 3.00f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

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
