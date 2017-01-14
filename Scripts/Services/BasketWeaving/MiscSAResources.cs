#region Header
// **********
// ServUO - MiscSAResources.cs
// **********
#endregion

#region References
using Server.Engines.Plants;
using Server.Engines.Craft;
using System;
#endregion

namespace Server.Items
{
    public class DryReeds : Item, IPlantHue
    {
        private PlantHue m_PlantHue;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue { get { return m_PlantHue; } set { m_PlantHue = value; InvalidatePlantHue(); InvalidateProperties(); } }

        public override int LabelNumber { get { return 1112248; } } //dry reeds

        public DryReeds(PlantHue hue)
            : base(0x1BD5)
        {
            PlantHue = hue;
            Stackable = true;
        }

        [Constructable]
        public DryReeds()
            : this(PlantHue.Plain)
        {
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
            int cliloc;

            if (Amount > 1)
            {
                cliloc = info.IsBright() ? 1113273 : 1113275;
                list.Add(cliloc, String.Format("{0}\t#{1}", Amount.ToString(), info.Name));
            }
            else
            {
                cliloc = info.IsBright() ? 1112288 : 1112289;
                list.Add(cliloc, String.Format("#{0}", info.Name));
            }
        }

        public DryReeds(Serial serial)
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

    public class SoftenedReeds : Item, IPlantHue
    {
        private PlantHue m_PlantHue;

        [CommandProperty(AccessLevel.GameMaster)]
        public PlantHue PlantHue { get { return m_PlantHue; } set { m_PlantHue = value; InvalidatePlantHue(); InvalidateProperties(); } }

        public override int LabelNumber { get { return 1112249; } } //Softened reeds

        [Constructable]
        public SoftenedReeds()
            : this(PlantHue.Plain)
        {
        }

        public SoftenedReeds(PlantHue hue)
            : base(0x4006)
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

        public void InvalidateHue()
        {
            PlantHueInfo info = PlantHueInfo.GetInfo(Hue);
            m_PlantHue = info.PlantHue;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {

            PlantHueInfo info = PlantHueInfo.GetInfo(m_PlantHue);
            int cliloc;

            if (Amount > 1)
            {
                cliloc = info.IsBright() ? 1113273 : 1113275;
                list.Add(cliloc, String.Format("{0}\t#{1}", Amount.ToString(), info.Name));
            }
            else
            {
                cliloc = info.IsBright() ? 1112288 : 1112289;
                list.Add(cliloc, String.Format("#{0}", info.Name));
            }
        }

        public SoftenedReeds(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);

            writer.Write((int)m_PlantHue);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int v = reader.ReadInt();

            if(v > 1)
                m_PlantHue = (PlantHue)reader.ReadInt();
        }
    }
}