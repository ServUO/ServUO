using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishTalwar))]
    [Flipable(0x143E, 0x143F)]
    public class Halberd : BasePoleArm
    {
        [Constructable]
        public Halberd()
            : base(0x143E)
        {
            Weight = 16.0;
        }

        public Halberd(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.WhirlwindAttack;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;
        public override int StrengthReq => 95;
        public override int MinDamage => 18;
        public override int MaxDamage => 21;
        public override float Speed => 4.00f;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;
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