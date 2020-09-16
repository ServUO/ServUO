using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualShortAxes))]
    [Flipable(0x1443, 0x1442)]
    public class TwoHandedAxe : BaseAxe
    {
        [Constructable]
        public TwoHandedAxe()
            : base(0x1443)
        {
            Weight = 8.0;
        }

        public TwoHandedAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ShadowStrike;
        public override int StrengthReq => 40;
        public override int MinDamage => 16;
        public override int MaxDamage => 19;
        public override float Speed => 3.50f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 90;
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