namespace Server.Items
{
    [Flipable(0x906, 0x406F)]
    public class SerpentStoneStaff : BaseStaff
    {
        [Constructable]
        public SerpentStoneStaff()
            : base(0x906)
        {
            //Weight = 3.0;
        }

        public SerpentStoneStaff(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 35;
        public override int MinDamage => 16;
        public override int MaxDamage => 19;
        public override float Speed => 3.50f;

        public override int InitMinHits => 31;
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
