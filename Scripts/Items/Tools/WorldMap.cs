using System;

namespace Server.Items
{
    public class WorldMap : MapItem
    {
        [Constructable]
        public WorldMap()
        {
            this.SetDisplay(0, 0, 5119, 4095, 400, 400);
        }

        public WorldMap(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1015233;
            }
        }// world map
        public override void CraftInit(Mobile from)
        {
            // Unlike the others, world map is not based on crafted location
            double skillValue = from.Skills[SkillName.Cartography].Value;
            int x20 = (int)(skillValue * 20);
            int size = 25 + (int)(skillValue * 6.6);

            if (size < 200)
                size = 200;
            else if (size > 400)
                size = 400;

            this.SetDisplay(1344 - x20, 1600 - x20, 1472 + x20, 1728 + x20, size, size);
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