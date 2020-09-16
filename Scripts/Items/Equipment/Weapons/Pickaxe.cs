using Server.Engines.Harvest;

namespace Server.Items
{
    [Flipable(0xE86, 0xE85)]
    public class Pickaxe : BaseAxe, IUsesRemaining, IHarvestTool
    {
        [Constructable]
        public Pickaxe()
            : base(0xE86)
        {
            Weight = 11.0;
            UsesRemaining = 50;
            ShowUsesRemaining = true;
        }

        public Pickaxe(Serial serial)
            : base(serial)
        {
        }

        public override HarvestSystem HarvestSystem => Mining.System;
        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 50;
        public override int MinDamage => 12;
        public override int MaxDamage => 16;
        public override float Speed => 3.00f;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Slash1H;
		
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
