using Server.Engines.Craft;
using Server.Engines.Harvest;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishScythe))]
    [Flipable(0x26BA, 0x26C4)]
    public class Scythe : BasePoleArm
    {
        [Constructable]
        public Scythe()
            : base(0x26BA)
        {
            Weight = 5.0;
        }

        public Scythe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.BleedAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ParalyzingBlow;
        public override int StrengthReq => 45;
        public override int MinDamage => 16;
        public override int MaxDamage => 19;
        public override float Speed => 3.50f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;
        public override HarvestSystem HarvestSystem => null;
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
