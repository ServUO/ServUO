using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0x1034, 0x1035)]
    public class Saw : BaseTool
    {
        [Constructable]
        public Saw()
            : base(0x1034)
        {
            this.Weight = 2.0;
        }

        [Constructable]
        public Saw(int uses)
            : base(uses, 0x1034)
        {
            this.Weight = 2.0;
        }

        public Saw(Serial serial)
            : base(serial)
        {
        }

        public override CraftSystem CraftSystem
        {
            get
            {
                return DefCarpentry.CraftSystem;
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
        }
    }
}