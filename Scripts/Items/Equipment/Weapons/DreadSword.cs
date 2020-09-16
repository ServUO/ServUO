namespace Server.Items
{
    [Flipable(0x90B, 0x4074)]
    public class DreadSword : BaseSword
    {
        [Constructable]
        public DreadSword()
            : base(0x90B)
        {
            //Weight = 7.0;
        }

        public DreadSword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;
        public override int StrengthReq => 35;
        public override int MinDamage => 14;
        public override int MaxDamage => 18;
        public override float Speed => 3.50f;

        public override int DefHitSound => 0x237;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;

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
