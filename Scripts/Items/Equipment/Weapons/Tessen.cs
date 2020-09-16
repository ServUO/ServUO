using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishTessen))]
    [Flipable(0x27A3, 0x27EE)]
    public class Tessen : BaseBashing
    {
        [Constructable]
        public Tessen()
            : base(0x27A3)
        {
            Weight = 6.0;
            Layer = Layer.TwoHanded;
        }

        public Tessen(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.Feint;
        public override WeaponAbility SecondaryAbility => WeaponAbility.DualWield;
        public override int StrengthReq => 10;
        public override int MinDamage => 10;
        public override int MaxDamage => 13;
        public override float Speed => 2.00f;

        public override int DefHitSound => 0x232;
        public override int DefMissSound => 0x238;
        public override int InitMinHits => 55;
        public override int InitMaxHits => 60;
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