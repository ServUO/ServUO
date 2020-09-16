namespace Server.Items
{
    [Flipable(0x2D25, 0x2D31)]
    public class WildStaff : BaseStaff
    {
        [Constructable]
        public WildStaff()
            : base(0x2D25)
        {
            Weight = 8.0;
        }

        public WildStaff(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Block;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ForceOfNature;
        public override int StrengthReq => 15;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.25f;

        public override int InitMinHits => 30;
        public override int InitMaxHits => 60;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}