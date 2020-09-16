namespace Server.Items
{
    [Flipable(0x27A4, 0x27EF)]
    public class Wakizashi : BaseSword
    {
        [Constructable]
        public Wakizashi()
            : base(0x27A4)
        {
            Weight = 5.0;
            Layer = Layer.OneHanded;
        }

        public Wakizashi(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.FrenziedWhirlwind;
        public override WeaponAbility SecondaryAbility => WeaponAbility.DoubleStrike;
        public override int StrengthReq => 20;
        public override int MinDamage => 10;
        public override int MaxDamage => 14;
        public override float Speed => 2.50f;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 45;
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