using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(GargishAxe))]
    [Flipable(0xF49, 0xF4A)]
    public class Axe : BaseAxe
    {
        [Constructable]
        public Axe()
            : base(0xF49)
        {
            Weight = 4.0;
        }

        public Axe(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.CrushingBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.Dismount;

        public override int StrengthReq => 35;

        public override int MinDamage => 14;
        public override int MaxDamage => 17;

        public override float Speed => 3.00f;

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