using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishButcherKnife))]
    [Flipable(0x13F6, 0x13F7)]
    public class ButcherKnife : BaseKnife
    {
        [Constructable]
        public ButcherKnife()
            : base(0x13F6)
        {
            Weight = 1.0;
        }

        public ButcherKnife(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.InfectiousStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 10;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.25f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 40;
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