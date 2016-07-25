using System;

namespace Server.Items
{
    public class TatteredAncientScroll : Item
    {
        public override int LabelNumber { get { return 1112991; } } // Tattered Remnants of an Ancient Scroll

        [Constructable]
        public TatteredAncientScroll()
            : base(0x1700)
        {
            this.Weight = 1.0;
            this.Hue = 2405;
        }

        public TatteredAncientScroll(Serial serial)
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