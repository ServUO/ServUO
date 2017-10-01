using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0xE8A, 0xE89)]
    public class Blowpipe : BaseTool
    {
        public override int LabelNumber { get { return 1044609; } } // Blow Pipe

        [Constructable]
        public Blowpipe()
            : base(0xE8A)
        {
            Weight = 1.0;
            Hue = 2301;
        }

        [Constructable]
        public Blowpipe(int uses)
            : base(uses, 0xE8A)
        {
            Weight = 4.0;
            Hue = 0x3B9;
        }

        public Blowpipe(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefGlassblowing.CraftSystem;
            }
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

            if (Weight != 1.0)
                Weight = 1.0;
        }
    }
}