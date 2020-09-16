namespace Server.Items
{
    [Flipable(0xF62, 0xF63)]
    public class TribalSpear : BaseSpear
    {
        public override int LabelNumber => 1062474;  // Tribal Spear

        [Constructable]
        public TribalSpear()
            : base(0xF62)
        {
            Weight = 7.0;
            Hue = 837;
            Attributes.WeaponDamage = 20;
            WeaponAttributes.DurabilityBonus = 20;
        }

        public TribalSpear(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ParalyzingBlow;
        public override int StrengthReq => 50;
        public override int MinDamage => 13;
        public override int MaxDamage => 15;
        public override float Speed => 2.75f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

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
