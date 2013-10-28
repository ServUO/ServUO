using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0xE8A, 0xE89)]
    public class Blowpipe : BaseTool
    {
        [Constructable]
        public Blowpipe()
            : base(0xE8A)
        {
            this.Weight = 4.0;
            this.Hue = 0x3B9;
        }

        [Constructable]
        public Blowpipe(int uses)
            : base(uses, 0xE8A)
        {
            this.Weight = 4.0;
            this.Hue = 0x3B9;
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
        public override int LabelNumber
        {
            get
            {
                return 1044608;
            }
        }// blow pipe
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            if (this.Weight == 2.0)
                this.Weight = 4.0;
        }
    }
}