using Server.Engines.Harvest;

namespace Server.Items
{
    public class SturdyPickaxe : BaseAxe, IUsesRemaining
    {
        public override int LabelNumber => 1045126; // sturdy pickaxe

        [Constructable]
        public SturdyPickaxe()
            : this(180)
        {
        }

        [Constructable]
        public SturdyPickaxe(int uses)
            : base(0xE86)
        {
            Weight = 11.0;
            Hue = 0x973;
            UsesRemaining = uses;
            ShowUsesRemaining = true;
        }

        public SturdyPickaxe(Serial serial)
            : base(serial)
        {
        }

        public override HarvestSystem HarvestSystem => Mining.System;
        public override WeaponAbility PrimaryAbility => WeaponAbility.DoubleStrike;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Disarm;
        public override int StrengthReq => 50;
        public override int MinDamage => 13;
        public override int MaxDamage => 15;
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
