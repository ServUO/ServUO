namespace Server.Items
{
    [Flipable(0xE87, 0xE88)]
    public class Pitchfork : BaseSpear
    {
        [Constructable]
        public Pitchfork()
            : base(0xE87)
        {
            Weight = 11.0;
        }

        public Pitchfork(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 55;
        public override int MinDamage => 12;
        public override int MaxDamage => 15;
        public override float Speed => 2.50f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;
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
