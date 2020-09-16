namespace Server.Items
{
    [Flipable(0x48B8, 0x48B9)]
    public class GargishGnarledStaff : BaseStaff
    {
        [Constructable]
        public GargishGnarledStaff()
            : base(0x48B8)
        {
            Weight = 3.0;
        }

        public GargishGnarledStaff(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ConcussionBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ForceOfNature;
        public override int StrengthReq => 20;
        public override int MinDamage => 15;
        public override int MaxDamage => 18;
        public override float Speed => 3.25f;

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
