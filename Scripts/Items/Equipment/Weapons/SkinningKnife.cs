namespace Server.Items
{
    [Flipable(0xEC4, 0xEC5)]
    public class SkinningKnife : BaseKnife
    {
        [Constructable]
        public SkinningKnife()
            : base(0xEC4)
        {
            Weight = 1.0;
        }

        public SkinningKnife(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ShadowStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.BleedAttack;
        public override int StrengthReq => 5;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.25f;

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
