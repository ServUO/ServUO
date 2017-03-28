using System;
using Server.Mobiles;

namespace Server.Items
{
    public class KotlBlackRod : BlackStaff
    {
        public override int LabelNumber { get { return 1156990; } } // kotl black rod

        [Constructable]
        public KotlBlackRod()
        {
            WeaponAttributes.MageWeapon = 30;
            Attributes.SpellChanneling = 1;
            Attributes.CastSpeed = 2;
            Attributes.LowerManaCost = 5;
            Attributes.LowerRegCost = 10;
        }

        public KotlBlackRod(Serial serial)
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
