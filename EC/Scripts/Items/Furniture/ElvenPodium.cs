using System;

namespace Server.Items
{
    [Furniture]
    [Flipable(0x2D4B, 0x2D4C)]
    public class ElvenPodium : CraftableFurniture
    {
        [Constructable]
        public ElvenPodium()
            : base(0x2D4B)
        {
            this.Weight = 1.0;
        }

        public ElvenPodium(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1073399;
            }
        }// elven podium
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