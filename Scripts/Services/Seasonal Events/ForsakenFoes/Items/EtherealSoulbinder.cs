using System;

namespace Server.Items
{
    public class EtherealSoulbinder : Item
    {
        public override int LabelNumber { get { return 1159167; } } // ethereal soulbinder

        public double MaxSoulPoint { get; set; } = 100;

        private double m_SoulPoint;

        [CommandProperty(AccessLevel.GameMaster)]
        public double SoulPoint
        {
            get
            {
                return m_SoulPoint;
            }
            set
            {
                if (value < 0)
                    value = 0;
                else if (value > MaxSoulPoint)
                    value = MaxSoulPoint;

                m_SoulPoint += value;

                SetHue();
                InvalidateProperties();
            }
        }

        private void SetHue()
        {
            if (SoulPoint <= 0)
                Hue = 0;
            else if (SoulPoint <= 1)
                Hue = 1909; // Meager
            else if (SoulPoint <= 25)
                Hue = 1917; // Grand
            else if (SoulPoint <= 50)
                Hue = 1914; // Exalted
            else if (SoulPoint <= 90)
                Hue = 1922; // Legendary
            else
                Hue = 1919; // Mythical
        }

        private int GetDescription()
        {
            if (SoulPoint <= 0)
                return 1159177; // An Empty Soulbinder
            else if (SoulPoint <= 33)
                return 1159176; // Meager
            else if (SoulPoint <= 66)
                return 1159175; // Grand
            else if (SoulPoint <= 66)
                return 1159174; // Exalted
            else if (SoulPoint <= 66)
                return 1159173; // Legendary
            else
                return 1159172; // Mythical
        }

        [Constructable]
        public EtherealSoulbinder()
            : base(0xA1E7)
        {
        }

        public EtherealSoulbinder(Serial serial)
            : base(serial)
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            int desc = GetDescription();

            if (desc == 1159177)
            {
                list.Add(1159177); // An Empty Soulbinder
            }
            else
            {
                list.Add(1159178, string.Format("#{0}", desc)); // Contains a ~1_TYPE~ Soul
            }
        }
        
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version

            writer.Write(m_SoulPoint);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_SoulPoint = reader.ReadDouble();
        }
    }
}
