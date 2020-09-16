namespace Server.Items
{
    [Flipable(0x48B4, 0x48B5)]
    public class GargishBardiche : BasePoleArm
    {
        [Constructable]
        public GargishBardiche()
            : base(0x48B4)
        {
            Weight = 7.0;
        }

        public GargishBardiche(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ParalyzingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 45;
        public override int MinDamage => 17;
        public override int MaxDamage => 20;
        public override float Speed => 3.75f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;

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
