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
            foreach (Item item in this.GetItemsInRange(8))
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

            writer.Write((int)0); // version
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
                this.item = sphere;
                this.num = coord;
            }

            protected override void OnTick()
            {
                if (this.item.Deleted)
                {
                    this.Stop();
                    return;
                }

                this.m_Stage++;
				
                if (this.m_Cicle == 0)
                    this.item.Z += 1;
                else if (this.m_Cicle == 1)
                    this.item.Z += 0;
                else
                    this.item.Z += -1;

                if (this.m_Stage == 8)
                    this.m_Cicle++;
                else if (this.m_Stage == 14)
                    this.m_Cicle++;
                else if (this.m_Stage == 22)
                    this.Stop();
            }
        }
    }
}