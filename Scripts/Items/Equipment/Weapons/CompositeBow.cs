using System;

namespace Server.Items
{
    [Flipable(0x26C2, 0x26CC)]
    public class CompositeBow : BaseRanged
    {
        [Constructable]
        public CompositeBow()
            : base(0x26C2)
        {
            Weight = 5.0;
        }

        public CompositeBow(Serial serial)
            : base(serial)
        {
        }

        public override int EffectID => 0xF42;
        public override Type AmmoType => typeof(Arrow);
        public override Item Ammo => new Arrow();
        public override WeaponAbility PrimaryAbility => WeaponAbility.ArmorIgnore;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MovingShot;
        public override int StrengthReq => 45;
        public override int MinDamage => 16;
        public override int MaxDamage => 20;
        public override float Speed => 4.00f;

        public override int DefMaxRange => 10;
        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;
        public override WeaponAnimation DefAnimation => WeaponAnimation.ShootBow;
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
