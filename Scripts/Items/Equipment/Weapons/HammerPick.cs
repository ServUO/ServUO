using Server.Engines.Craft;

namespace Server.Items
{
    [Alterable(typeof(DefBlacksmithy), typeof(DiscMace))]
    [Flipable(0x143D, 0x143C)]
    public class HammerPick : BaseBashing
    {
        [Constructable]
        public HammerPick()
            : base(0x143D)
        {
            Weight = 9.0;
            Layer = Layer.OneHanded;
        }

        public HammerPick(Serial serial)
            : base(serial)
        {
        }

        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 45;
        public override int MinDamage => 13;
        public override int MaxDamage => 17;
        public override float Speed => 3.25f;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;
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