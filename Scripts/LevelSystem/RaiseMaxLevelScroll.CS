using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Server.Items
{
    public class RaiseMaxLevelScroll : Item
    {
        private int m_RML;

        [CommandProperty(AccessLevel.GameMaster)]
        public int RML
        {
            get { return m_RML; }
            set { m_RML = value; InvalidateProperties(); }
        }
        public RaiseMaxLevelScroll(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((int)m_RML);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            LootType = LootType.Blessed;

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_RML = reader.ReadInt();
                        break;
                    }
            }
        }
        [Constructable]
        public RaiseMaxLevelScroll()
            : base(0x14F0)
        {
            if (!(Off.Enabled))
                Name = "Raise Max Level Scroll";
            else
                Name = "An Unknown Scroll";

            Weight = 1.0;
            Hue = 1153;
            LootType = LootType.Blessed;

            m_RML = Utility.RandomMinMax(1, 5);
        }
        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (!(Off.Enabled))
                list.Add("+{0}", m_RML.ToString()); // value: ~1_val~
        }
        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            Configured c = new Configured();

            if (IsChildOf(pm.Backpack))
            {
                if (!(Off.Enabled || Cl.Enabled))
                {
                    if (pm.charMaxLevel >= c.EndMaxLvl)
                        pm.SendMessage("You cannot raise any farther with this!");
                    else
                    {
                        pm.charMaxLevel += m_RML;
                        pm.SendMessage("Your MaxLevel has increased by {0}", m_RML);
                        this.Delete();
                    }
                }
            }
            else
                pm.SendMessage("This must be in your pack!");
        }
    }
}