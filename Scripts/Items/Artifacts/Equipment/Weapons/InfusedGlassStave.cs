namespace Server.Items
{
    [Flipable(0x905, 0x4070)]
    public class InfusedGlassStave : BaseStaff
    {
        public override int LabelNumber => 1112909;  // infused glass stave

        [Constructable]
        public InfusedGlassStave()
            : base(0x905)
        {
            Weight = 4.0;
            Hue = 23;
        }

        public InfusedGlassStave(Serial serial)
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
