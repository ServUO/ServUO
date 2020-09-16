using Server.Engines.Harvest;

namespace Server.Items
{
    [Flipable(0x48C4, 0x48C5)]
    public class GargishScythe : BasePoleArm
    {
        [Constructable]
        public GargishScythe()
            : base(0x48C4)
        {
            Weight = 5.0;
        }

        public GargishScythe(Serial serial)
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
