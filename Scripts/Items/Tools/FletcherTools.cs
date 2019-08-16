using System;
using Server.Engines.Craft;

namespace Server.Items
{
    [FlipableAttribute(0x1022, 0x1023)]
    public class FletcherTools : BaseTool
    {
		public override CraftSystem CraftSystem { get { return DefBowFletching.CraftSystem; } }
		public override int LabelNumber { get { return 1044559; } } // Fletcher's Tools
		
        [Constructable]
        public FletcherTools()
            : base(0x1022)
        {
        }

        [Constructable]
        public FletcherTools(int uses)
            : base(uses, 0x1022)
        {
            Weight = 2.0;
        }

        public FletcherTools(Serial serial)
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