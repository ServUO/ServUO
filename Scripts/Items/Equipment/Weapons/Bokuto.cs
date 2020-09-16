namespace Server.Items
{
    [Flipable(0x27A8, 0x27F3)]
    public class Bokuto : BaseSword
    {
        [Constructable]
        public Bokuto()
            : base(0x27A8)
        {
            Weight = 7.0;
        }

        public Bokuto(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Feint;
        public override WeaponAbility SecondaryAbility => WeaponAbility.NerveStrike;
        public override int StrengthReq => 20;
        public override int MinDamage => 10;
        public override int MaxDamage => 12;
        public override float Speed => 2.00f;

        public override int DefHitSound => 0x536;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 25;
        public override int InitMaxHits => 50;
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