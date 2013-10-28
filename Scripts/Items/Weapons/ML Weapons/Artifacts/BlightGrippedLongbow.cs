using System;

namespace Server.Items
{
    public class BlightGrippedLongbow : ElvenCompositeLongbow
    {
        [Constructable]
        public BlightGrippedLongbow()
        {
            this.Hue = 0x8A4;

            this.WeaponAttributes.HitPoisonArea = 20;
            this.Attributes.RegenStam = 3;
            this.Attributes.NightSight = 1;
            this.Attributes.WeaponSpeed = 20;
            this.Attributes.WeaponDamage = 35;
        }

        public BlightGrippedLongbow(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072907;
            }
        }// Blight Gripped Longbow
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