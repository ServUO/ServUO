namespace Server.Items
{
    [Flipable(0x48C8, 0x48C9)]
    public class GargishPike : BaseSpear
    {
        [Constructable]
        public GargishPike()
            : base(0x48C8)
        {
            Weight = 8.0;
        }

        public GargishPike(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ParalyzingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.InfectiousStrike;
        public override int StrengthReq => 50;
        public override int MinDamage => 14;
        public override int MaxDamage => 17;
        public override float Speed => 3.00f;

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
