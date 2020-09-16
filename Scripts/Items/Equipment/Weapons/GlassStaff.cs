namespace Server.Items
{
    [Flipable(0x905, 0x4070)]
    public class GlassStaff : BaseStaff
    {
        [Constructable]
        public GlassStaff()
            : base(0x905)
        {
            Weight = 4.0;
        }

        public GlassStaff(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 20;
        public override int MinDamage => 11;
        public override int MaxDamage => 14;
        public override float Speed => 2.25f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

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
