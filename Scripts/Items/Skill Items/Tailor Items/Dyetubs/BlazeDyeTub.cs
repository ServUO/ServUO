using System;

namespace Server.Items
{
    public class BlazeDyeTub : DyeTub
    {
        [Constructable]
        public BlazeDyeTub()
        {
            this.Hue = this.DyedHue = 0x489;
            this.Redyable = false;
        }

        public BlazeDyeTub(Serial serial)
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