using System;

namespace Server.Items
{
    [Flipable(0x2D2B, 0x2D1F)]
    public class MagicalShortbow : BaseRanged
    {
        [Constructable]
        public MagicalShortbow()
            : base(0x2D2B)
        {
            Weight = 6.0;
        }

        public MagicalShortbow(Serial serial)
            : base(serial)
        {
        }

        public override int EffectID => 0xF42;
        public override Type AmmoType => typeof(Arrow);
        public override Item Ammo => new Arrow();
        public override WeaponAbility PrimaryAbility => WeaponAbility.LightningArrow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.PsychicAttack;
        public override int StrengthReq => 45;
        public override int MinDamage => 12;
        public override int MaxDamage => 16;
        public override float Speed => 3.00f;

        public override int DefMaxRange => 10;
        public override int InitMinHits => 41;
        public override int InitMaxHits => 90;
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.WriteEncodedInt(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadEncodedInt();
        }
    }
}
