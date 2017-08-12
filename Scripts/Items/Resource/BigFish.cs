using System;

namespace Server.Items
{
    public class BigFish : Item, ICarvable
    {
        private Mobile m_Fisher;
        private DateTime m_DateCaught;

        [Constructable]
        public BigFish()
            : base(0x09CC)
        {
            Weight = Math.Max(20, GetWeight());

            Hue = Utility.RandomBool() ? 0x847 : 0x58C;
        }

        private int GetWeight()
        {
            int v = Utility.RandomMinMax(0, 10000);
            v = (int)Math.Sqrt(v);
            v = 100 - v;

            return (int)(225.0 * ((double)v / 100));
        }

        public BigFish(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public Mobile Fisher
        {
            get
            {
                return m_Fisher;
            }
            set
            {
                m_Fisher = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime DateCaught { get { return m_DateCaught; } set { m_DateCaught = value; } }

        public override int LabelNumber
        {
            get
            {
                return 1041112;
            }
        }// a big fish
        public bool Carve(Mobile from, Item item)
        {
            base.ScissorHelper(from, new RawFishSteak(), Math.Max(16, (int)Weight) / 4, false);
            return true;
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (Weight >= 20)
            {
                if (m_Fisher != null)
                    list.Add(1070857, m_Fisher.Name); // Caught by ~1_fisherman~

                if (m_DateCaught != DateTime.MinValue)
                    list.Add(1049644, m_DateCaught.ToShortDateString()); // [~1_stuff~]

                list.Add(1070858, ((int)Weight).ToString()); // ~1_weight~ stones
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)2); // version

            writer.Write(m_DateCaught);
            writer.Write((Mobile)m_Fisher);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 2:
                    {
                        m_DateCaught = reader.ReadDateTime();
                        goto case 1;
                    }
                case 1:
                    {
                        m_Fisher = reader.ReadMobile();
                        break;
                    }
                case 0:
                    {
                        Weight = Utility.RandomMinMax(3, 200);
                        break;
                    }
            }
        }
    }
}