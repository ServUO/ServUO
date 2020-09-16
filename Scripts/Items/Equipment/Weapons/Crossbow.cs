using System;

namespace Server.Items
{
    [Flipable(0xF50, 0xF4F)]
    public class Crossbow : BaseRanged
    {
        [Constructable]
        public Crossbow()
            : base(0xF50)
        {
            Weight = 7.0;
            Layer = Layer.TwoHanded;
        }

        public Crossbow(Serial serial)
            : base(serial)
        {
        }

        public override int EffectID => 0x1BFE;
        public override Type AmmoType => typeof(Bolt);
        public override Item Ammo => new Bolt();
        public override WeaponAbility PrimaryAbility => WeaponAbility.ConcussionBlow;
        public override WeaponAbility SecondaryAbility => WeaponAbility.MortalStrike;
        public override int StrengthReq => 35;
        public override int MinDamage => 18;
        public override int MaxDamage => 22;
        public override float Speed => 4.50f;

        public override int DefMaxRange => 8;
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