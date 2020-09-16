using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualShortAxes))]
    [Flipable(0xf4b, 0xf4c)]
    public class DoubleAxe : BaseAxe
    {
        [Constructable]
        public DoubleAxe()
            : base(0xF4B)
        {
            Weight = 8.0;
        }

        public DoubleAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.WhirlwindAttack;
        public override int StrengthReq => 45;
        public override int MinDamage => 15;
        public override int MaxDamage => 18;
        public override float Speed => 3.25f;

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