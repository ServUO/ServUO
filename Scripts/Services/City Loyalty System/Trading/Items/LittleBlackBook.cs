using System;
using Server;
using Server.Mobiles;
using Server.Engines.CityLoyalty;

namespace Server.Items
{
    public class LittleBlackBook : Item
    {
        public override int LabelNumber { get { return 1151751; } } // Slim the Fence's little black book

        [Constructable]
        public LittleBlackBook()
            : base(4030)
        {
            Hue = 1932;
        }

        public LittleBlackBook(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}