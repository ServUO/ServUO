namespace Server.Items
{
    [Flipable(0x2D35, 0x2D29)]
    public class ElvenMachete : BaseSword
    {
        [Constructable]
        public ElvenMachete()
            : base(0x2D35)
        {
            Weight = 6.0;
        }

        public ElvenMachete(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DefenseMastery;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Bladeweave;
        public override int StrengthReq => 20;
        public override int MinDamage => 11;
        public override int MaxDamage => 15;
        public override float Speed => 2.75f;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x239;
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