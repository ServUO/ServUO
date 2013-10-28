using System;

namespace Server.Items
{
    public class ColorFixative : Item
    {
        [Constructable]
        public ColorFixative()
            : base(0x182D)
        {
            this.Weight = 1.0;
            this.Hue = 473;  // ...make this the proper shade of green
        }

        public ColorFixative(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1112135;
            }
        }// color fixative
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