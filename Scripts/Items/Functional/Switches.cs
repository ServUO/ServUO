using System;

namespace Server.Items
{
    public class UnderworldSwitchWE : BaseSwitch
    {
        [Constructable]
        public UnderworldSwitchWE()
            : base(0x1091, 0x1092, 1042901, 1042900, true)
        {
            //1042901 = You hear a deep rumbling as something seems to happen.
            //1042900 = There seems to be no further effect right now.
            //true = It do something, it is not useless or broken switch.
        }

        public UnderworldSwitchWE(Serial serial)
            : base(serial)
        {
        }

        public override void DoSomethingSpecial(Mobile from)
        {
            foreach (Item item in GetItemsInRange(8))
            {
                if (item.ItemID == 0x3660 && item.Hue == 1000) //Dark Globe of Sosaria
                {
                    Timer m_timerA = new MoveTimer(item, 1);
                    m_timerA.Start();
                }
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        private class MoveTimer : Timer
        {
            private readonly Item item;
            private readonly int num;
            private int m_Stage;
            private int m_Cicle;
            public MoveTimer(Item sphere, int coord)
                : base(TimeSpan.FromSeconds(0.0), TimeSpan.FromSeconds(1.5))
            {
                item = sphere;
                num = coord;
            }

            protected override void OnTick()
            {
                if (item.Deleted)
                {
                    Stop();
                    return;
                }

                m_Stage++;

                if (m_Cicle == 0)
                    item.Z += 1;
                else if (m_Cicle == 1)
                    item.Z += 0;
                else
                    item.Z += -1;

                if (m_Stage == 8)
                    m_Cicle++;
                else if (m_Stage == 14)
                    m_Cicle++;
                else if (m_Stage == 22)
                    Stop();
            }
        }
    }
}