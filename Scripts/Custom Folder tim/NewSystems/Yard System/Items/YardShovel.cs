using System;
using Server.Network;
using Server.Items;
using Server.Gumps;
using Server.Misc;

namespace Server.ACC.YS
{
    public class YardShovel : Item
    {
        private string m_Category;
        public string Category
        {
            get { return m_Category; }
            set { m_Category = value; }
        }

        private int m_Page;
        public int Page
        {
            get { return m_Page; }
            set { m_Page = value; }
        }

        private int m_XStart;
        public int XStart
        {
            get { return m_XStart < 0 ? 0 : m_XStart; }
            set { m_XStart = value < 0 ? 0 : value; }
        }

        private int m_YStart;
        public int YStart
        {
            get { return m_YStart < 0 ? 0 : m_YStart; }
            set { m_YStart = value < 0 ? 0 : value; }
        }

        [Constructable]
        public YardShovel()
            : base(3897)
        {
            Movable = true;
            Name = "Yard Shovel";
            XStart = 50;
            YStart = 10;
            Category = "";
            Page = 0;
        }

        public YardShovel(Serial serial)
            : base(serial)
        {
        }

        public override void OnDoubleClick(Mobile from)
        {
            YardTarget yt;
            
            if (m_Category != null)
            {
                yt = new YardTarget(this, from, 0, 0, Category, Page);
            }
            else
            {
                yt = new YardTarget(this, from, 0, 0, "", 0);
            }

            yt.GumpUp();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
            writer.Write(Category);
            writer.Write(Page);
            writer.Write(XStart);
            writer.Write(YStart);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        Category = reader.ReadString();
                        Page = reader.ReadInt();
                        XStart = reader.ReadInt();
                        YStart = reader.ReadInt();
                        break;
                    }
            }
        }
    }
}