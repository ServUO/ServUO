using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualShortAxes))]
    [Flipable(0x13FB, 0x13FA)]
    public class LargeBattleAxe : BaseAxe
    {
        [Constructable]
        public LargeBattleAxe()
            : base(0x13FB)
        {
            Weight = 6.0;
        }

        public LargeBattleAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.WhirlwindAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.BleedAttack;
        public override int StrengthReq => 80;
        public override int MinDamage => 17;
        public override int MaxDamage => 20;
        public override float Speed => 3.75f;

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