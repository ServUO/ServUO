using System;

namespace Server.Items
{
    public class NestWithEggs : Item
    {
        [Constructable]
        public NestWithEggs()
            : base(0x1AD4)
        {
            this.Hue = 2415;
            this.Weight = 2;
        }

        public NestWithEggs(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1026868;
            }
        }// nest with eggs
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