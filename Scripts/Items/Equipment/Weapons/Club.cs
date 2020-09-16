namespace Server.Items
{
    [Flipable(0x13b4, 0x13b3)]
    public class Club : BaseBashing
    {
        [Constructable]
        public Club()
            : base(0x13B4)
        {
            Weight = 9.0;
        }

        public Club(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ShadowStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 40;
        public override int MinDamage => 10;
        public override int MaxDamage => 14;
        public override float Speed => 2.50f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 40;
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
