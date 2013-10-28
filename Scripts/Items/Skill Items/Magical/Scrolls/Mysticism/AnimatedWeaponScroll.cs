using System;

namespace Server.Items
{
    public class AnimatedWeaponScroll : SpellScroll
    {
        [Constructable]
        public AnimatedWeaponScroll()
            : this(1)
        {
        }

        [Constructable]
        public AnimatedWeaponScroll(int amount)
            : base(683, 0x2DA4, amount)
        {
        }

        public AnimatedWeaponScroll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}