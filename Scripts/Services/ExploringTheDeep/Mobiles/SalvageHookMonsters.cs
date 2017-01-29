using Server.Items;
using System;

namespace Server.Mobiles
{
    [CorpseName("a krakens corpse")]
    public class HPKraken : Kraken
    {
        private Timer m_Timer;
        private bool m_QuestItem;

        [Constructable]
        public HPKraken(bool questitem) : base()
        {
            this.m_QuestItem = questitem;
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPKraken(Serial serial) : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            if (m_QuestItem && 0.05 >= Utility.RandomDouble())
            {
                c.AddItem(new BrokenShipwreckRemains());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_QuestItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_QuestItem = reader.ReadBool();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    [CorpseName("a deep sea serpents corpse")]
    public class HPDeepSeaSerpent : DeepSeaSerpent
    {
        private Timer m_Timer;
        private bool m_QuestItem;

        [Constructable]
        public HPDeepSeaSerpent(bool questitem) : base()
        {
            this.m_QuestItem = questitem;
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPDeepSeaSerpent(Serial serial) : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            if (m_QuestItem && 0.05 >= Utility.RandomDouble())
            {
                c.AddItem(new BrokenShipwreckRemains());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_QuestItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_QuestItem = reader.ReadBool();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    [CorpseName("a sea serpents corpse")]
    public class HPSeaSerpent : SeaSerpent
    {
        private Timer m_Timer;
        private bool m_QuestItem;

        [Constructable]
        public HPSeaSerpent(bool questitem) : base()
        {
            this.m_QuestItem = questitem;
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPSeaSerpent(Serial serial) : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            if (m_QuestItem && 0.05 >= Utility.RandomDouble())
            {
                c.AddItem(new BrokenShipwreckRemains());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_QuestItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_QuestItem = reader.ReadBool();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    [CorpseName("a water elemental corpse")]
    public class HPWaterElemental : WaterElemental
    {
        private Timer m_Timer;
        private bool m_QuestItem;

        [Constructable]
        public HPWaterElemental(bool questitem) : base()
        {
            this.m_QuestItem = questitem;
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPWaterElemental(Serial serial) : base(serial)
        {
        }

        public override void OnDeath(Container c)
        {
            if (m_QuestItem && 0.05 >= Utility.RandomDouble())
            {
                c.AddItem(new BrokenShipwreckRemains());
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(m_QuestItem);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            m_QuestItem = reader.ReadBool();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    public class InternalDeleteTimer : Timer
    {
        private Mobile m_Mobile;

        public InternalDeleteTimer(Mobile m) : base(TimeSpan.FromMinutes(30))
        {
            Priority = TimerPriority.FiveSeconds;
            m_Mobile = m;
        }
        protected override void OnTick()
        {
            if (m_Mobile.Map != Map.Internal)
            {
                m_Mobile.Delete();
                this.Stop();
            }
        }
    }
}