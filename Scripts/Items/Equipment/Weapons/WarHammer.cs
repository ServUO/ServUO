using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishWarHammer))]
    [Flipable(0x1439, 0x1438)]
    public class WarHammer : BaseBashing
    {
        [Constructable]
        public WarHammer()
            : base(0x1439)
        {
            Weight = 10.0;
            Layer = Layer.TwoHanded;
        }

        public WarHammer(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.WhirlwindAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.CrushingBlow;
        public override int StrengthReq => 95;
        public override int MinDamage => 17;
        public override int MaxDamage => 20;
        public override float Speed => 3.75f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;
        public override WeaponAnimation DefAnimation => WeaponAnimation.Bash2H;
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