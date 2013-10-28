using System;

namespace Server.Items
{
    public class SeaChart : MapItem
    {
        [Constructable]
        public SeaChart()
        {
            this.SetDisplay(0, 0, 5119, 4095, 400, 400);
        }

        public SeaChart(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1015232;
            }
        }// sea chart
        public override void CraftInit(Mobile from)
        {
            double skillValue = from.Skills[SkillName.Cartography].Value;
            int dist = 64 + (int)(skillValue * 10);

            if (dist < 200)
                dist = 200;

            int size = 24 + (int)(skillValue * 3.3);

            if (size < 200)
                size = 200;
            else if (size > 400)
                size = 400;

            this.SetDisplay(from.X - dist, from.Y - dist, from.X + dist, from.Y + dist, size, size);
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