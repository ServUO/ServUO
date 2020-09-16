namespace Server.Items
{
    [Flipable(0x13B6, 0x13B5)]
    public class Scimitar : BaseSword
    {
        [Constructable]
        public Scimitar()
            : base(0x13B6)
        {
            Weight = 5.0;
        }

        public Scimitar(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ParalyzingBlow;
        public override int StrengthReq => 25;
        public override int MinDamage => 12;
        public override int MaxDamage => 16;
        public override float Speed => 3.00f;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;
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