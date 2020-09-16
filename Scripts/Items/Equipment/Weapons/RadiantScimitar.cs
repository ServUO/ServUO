namespace Server.Items
{
    [Flipable(0x2D33, 0x2D27)]
    public class RadiantScimitar : BaseSword
    {
        [Constructable]
        public RadiantScimitar()
            : base(0x2D33)
        {
            Weight = 9.0;
        }

        public RadiantScimitar(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.WhirlwindAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Bladeweave;
        public override int StrengthReq => 20;
        public override int MinDamage => 10;
        public override int MaxDamage => 14;
        public override float Speed => 2.50f;

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
