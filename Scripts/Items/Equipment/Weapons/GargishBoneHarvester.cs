namespace Server.Items
{
    [Flipable(0x48C6, 0x48C7)]
    public class GargishBoneHarvester : BaseSword
    {
        [Constructable]
        public GargishBoneHarvester()
            : base(0x48C6)
        {
            Weight = 3.0;
        }

        public GargishBoneHarvester(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ParalyzingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 25;
        public override int MinDamage => 12;
        public override int MaxDamage => 16;
        public override float Speed => 3.00f;

        public override int DefHitSound => 0x23B;
        public override int DefMissSound => 0x23A;
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
