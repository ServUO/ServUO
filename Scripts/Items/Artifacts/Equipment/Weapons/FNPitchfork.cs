namespace Server.Items
{
    [Flipable(0xE87, 0xE88)]
    public class FNPitchfork : BaseSpear
    {
        public override bool IsArtifact => true;
        public override int LabelNumber => 1113498; // Farmer Nash's Pitchfork		

        [Constructable]
        public FNPitchfork()
            : base(0xE87)
        {
            Weight = 11.0;
        }

        public FNPitchfork(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;
        public override int StrengthReq => 50;
        public override int MinDamage => 13;
        public override int MaxDamage => 14;
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
