namespace Server.Items
{
    [Flipable(0x48B2, 0x48B3)]
    public class GargishAxe : BaseAxe
    {
        [Constructable]
        public GargishAxe()
            : base(0x48B2)
        {
            Weight = 4.0;
        }

        public GargishAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 35;
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
