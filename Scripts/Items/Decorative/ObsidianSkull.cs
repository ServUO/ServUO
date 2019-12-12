using System;

namespace Server.Items
{
    [FlipableAttribute(0x9A1E, 0x9A1F)]
    public class ObsidianSkull : Item
    {
        public override int LabelNumber { get { return 1123478; } } // Obsidian Skull

        [Constructable]
        public ObsidianSkull()
            : base(0x9A1E)
        {          
        }

        public ObsidianSkull(Serial serial)
            : base(serial)
        {
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}
