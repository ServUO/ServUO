using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualPointedSpear))]
    [Flipable(0x1403, 0x1402)]
    public class ShortSpear : BaseSpear
    {
        [Constructable]
        public ShortSpear()
            : base(0x1403)
        {
            Weight = 4.0;
        }

        public ShortSpear(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ShadowStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 40;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.00f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Pierce1H;
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