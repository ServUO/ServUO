using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(StoneWarSword))]
    [Flipable(0x13B9, 0x13Ba)]
    public class VikingSword : BaseSword
    {
        [Constructable]
        public VikingSword()
            : base(0x13B9)
        {
            Weight = 6.0;
        }

        public VikingSword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ParalyzingBlow;
        public override int StrengthReq => 40;
        public override int MinDamage => 15;
        public override int MaxDamage => 19;
        public override float Speed => 3.75f;

        public override int DefHitSound => 0x237;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;
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