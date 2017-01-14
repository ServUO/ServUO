using System;
using Server;
using Server.Mobiles;
using Server.Engines.Plants;

namespace Server.Items
{
    public class PlantClippings : Item, IPlantHue
    {
        private PlantHue m_PlantHue;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue { get { return m_PlantHue; } set { m_PlantHue = value; InvalidatePlantHue(); InvalidateProperties(); } }

        public override int LabelNumber { get { return 1112131; } } // plant clippings

        [Constructable]
        public PlantClippings()
            : this(PlantHue.Plain)
        {
        }

        [Constructable]
        public PlantClippings(PlantHue hue)
            : base(0x4022)
        {
            m_PlantHue = hue;
            InvalidatePlantHue();
            Stackable = true;
        }

        public void InvalidatePlantHue()
        {
            PlantHueInfo info = PlantHueInfo.GetInfo(m_PlantHue);

            if (info == null)
            {
                m_PlantHue = PlantHue.Plain;
                Hue = 0;
            }
            else
                Hue = info.Hue;

            InvalidateProperties();
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            PlantHueInfo info = PlantHueInfo.GetInfo(m_PlantHue);

            if (this.Amount > 1)
                list.Add(info.IsBright() ? 1113272 : 1113274, String.Format("{0}\t#{1}", Amount.ToString(), info.Name)); //~1_AMOUNT~ bright ~2_COLOR~ plant clippings
            else
                list.Add(info.IsBright() ? 1112121 : 1112122, String.Format("#{0}", info.Name)); //bright ~1_COLOR~ plant clippings
        }

        public PlantClippings(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)1);

            writer.Write((int)m_PlantHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if(v > 0)
                m_PlantHue = (PlantHue)reader.ReadInt();
        }
    }
}