using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefTinkering), typeof(GargishCleaver))]
    [Flipable(0xEC3, 0xEC2)]
    public class Cleaver : BaseKnife
    {
        [Constructable]
        public Cleaver()
            : base(0xEC3)
        {
            Weight = 2.0;
        }

        public Cleaver(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.InfectiousStrike;
        public override int StrengthReq => 10;
        public override int MinDamage => 10;
        public override int MaxDamage => 14;
        public override float Speed => 2.50f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 50;
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
