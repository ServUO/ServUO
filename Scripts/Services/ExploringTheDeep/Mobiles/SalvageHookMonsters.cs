using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("a krakens corpse")]
    public class HPKraken : Kraken
    {
        private Timer m_Timer;

        [Constructable]
        public HPKraken() : base()
        {
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPKraken(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    [CorpseName("a deep sea serpents corpse")]
    public class HPDeepSeaSerpent : DeepSeaSerpent
    {
        private Timer m_Timer;

        [Constructable]
        public HPDeepSeaSerpent() : base()
        {
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPDeepSeaSerpent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    [CorpseName("a sea serpents corpse")]
    public class HPSeaSerpent : SeaSerpent
    {
        private Timer m_Timer;

        [Constructable]
        public HPSeaSerpent() : base()
        {
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPSeaSerpent(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }
    }

    [CorpseName("a water elemental corpse")]
    public class HPWaterElemental : WaterElemental
    {
        private Timer m_Timer;

        [Constructable]
        public HPWaterElemental() : base()
        {
            this.m_Timer = new InternalDeleteTimer(this);
            this.m_Timer.Start();
        }

        public HPWaterElemental(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

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