using System;

namespace Server.Items
{
    public class BlackDyeTub : DyeTub
    {
        [Constructable]
        public BlackDyeTub()
        {
            this.Hue = this.DyedHue = 0x0001;
            this.Redyable = false;
        }

        public BlackDyeTub(Serial serial)
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