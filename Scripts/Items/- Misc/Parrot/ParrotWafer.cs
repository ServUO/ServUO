using System;

namespace Server.Items
{
    public class ParrotWafer : Item
    {
        [Constructable]
        public ParrotWafer()
            : base(0x2FD6)
        {
            this.Hue = 0x38;
            this.Stackable = true;
        }

        public ParrotWafer(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1072904;
            }
        }// Parrot Wafers
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