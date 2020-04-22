using System;

namespace Server.Items
{
    public class GobletOfCelebration : Item
    {
        public override int LabelNumber => 1075430;  // Goblet of Celebration

        private bool m_Full;
        private DateTime m_NextFill;
        private Timer m_FillTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Full
        {
            get
            {
                return m_Full;
            }
            set
            {
                if (m_Full != value)
                {
                    m_Full = value;
                    InvalidateProperties();

                    if (m_FillTimer != null)
                        m_FillTimer.Stop();

                    if (!m_Full)
                    {
                        m_NextFill = DateTime.Now + TimeSpan.FromDays(1.0);

                        m_FillTimer = Timer.DelayCall(TimeSpan.FromDays(1.0), delegate { Full = true; });
                    }
                }
            }
        }

        [Constructable]
        public GobletOfCelebration()
            : base(0x99A)
        {
            LootType = LootType.Blessed;
            Weight = 1.0;
            Hue = 19;

            m_Full = true;
        }

        public override void OnDoubleClick(Mobile from)
        {
            base.OnDoubleClick(from);

            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1075438); // You can only drink from the goblet of celebration when its in your inventory.
            }
            else
            {
                if (m_Full)
                {
                    from.SendLocalizedMessage(1075272); // You drink from the goblet of celebration

                    Full = false;

                    from.BAC = 60;
                    BaseBeverage.CheckHeaveTimer(from);
                }
                else
                {
                    from.SendLocalizedMessage(1075439); // You need to wait a day for the goblet of celebration to be replenished.
                }
            }
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_Full)
                list.Add(1042972); // It's full.
            else
                list.Add(1042975); // It's empty.
        }

        public GobletOfCelebration(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Full);

            if (!m_Full)
            {
                writer.Write(m_NextFill);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_Full = reader.ReadBool();

            if (!m_Full)
            {
                m_NextFill = reader.ReadDateTime();

                m_FillTimer = Timer.DelayCall(m_NextFill - DateTime.Now, delegate { Full = true; });
            }
        }
    }
}
