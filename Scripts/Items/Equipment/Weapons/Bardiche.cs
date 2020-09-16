using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishBardiche))]
    [Flipable(0xF4D, 0xF4E)]
    public class Bardiche : BasePoleArm
    {
        [Constructable]
        public Bardiche()
            : base(0xF4D)
        {
            Weight = 7.0;
        }

        public Bardiche(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ParalyzingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;

        public override int StrengthReq => 45;

        public override int MinDamage => 17;
        public override int MaxDamage => 20;

        public override float Speed => 3.75f;

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