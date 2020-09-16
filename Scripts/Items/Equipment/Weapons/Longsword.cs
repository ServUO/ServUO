using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DreadSword))]
    [Flipable(0xF61, 0xF60)]
    public class Longsword : BaseSword
    {
        [Constructable]
        public Longsword()
            : base(0xF61)
        {
            Weight = 7.0;
        }

        public Longsword(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.ConcussionBlow;
        public override int StrengthReq => 35;
        public override int MinDamage => 14;
        public override int MaxDamage => 18;
        public override float Speed => 3.50f;

        public override int DefHitSound => 0x237;
        public override int DefMissSound => 0x23A;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 110;
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