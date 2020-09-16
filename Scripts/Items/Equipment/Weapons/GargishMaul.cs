namespace Server.Items
{
    [Flipable(0x48C2, 0x48C3)]
    public class GargishMaul : BaseBashing
    {
        [Constructable]
        public GargishMaul()
            : base(0x48C2)
        {
            Weight = 10.0;
        }

        public GargishMaul(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;
        public override int StrengthReq => 45;
        public override int MinDamage => 14;
        public override int MaxDamage => 18;
        public override float Speed => 3.50f;

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
