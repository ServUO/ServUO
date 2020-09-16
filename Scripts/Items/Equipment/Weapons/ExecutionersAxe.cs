using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DualShortAxes))]
    [Flipable(0xf45, 0xf46)]
    public class ExecutionersAxe : BaseAxe
    {
        [Constructable]
        public ExecutionersAxe()
            : base(0xF45)
        {
            Weight = 8.0;
        }

        public ExecutionersAxe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 40;
        public override int MinDamage => 15;
        public override int MaxDamage => 18;
        public override float Speed => 3.25f;

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
