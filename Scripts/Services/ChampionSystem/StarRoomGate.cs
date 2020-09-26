using System;

namespace Server.Items
{
    public class StarRoomGate : Moongate
    {
        private bool m_Decays;
        private DateTime m_DecayTime;
        private Timer m_Timer;

        public override int LabelNumber => 1049498;// dark moongate

        [Constructable]
        public StarRoomGate()
            : this(false)
        {
        }

        [Constructable]
        public StarRoomGate(bool decays, Point3D loc, Map map)
            : this(decays)
        {
            MoveToWorld(loc, map);
            Effects.PlaySound(loc, map, 0x20E);
        }

        [Constructable]
        public StarRoomGate(bool decays)
            : base(new Point3D(5143, 1774, 0), Map.Felucca)
        {
            Dispellable = false;
            ItemID = 0x1FD4;

            if (decays)
            {
                m_Decays = true;
                m_DecayTime = DateTime.UtcNow + TimeSpan.FromMinutes(2.0);

                m_Timer = new InternalTimer(this, m_DecayTime);
                m_Timer.Start();
            }
        }

        public StarRoomGate(Serial serial)
            : base(serial)
        {
        }

        public override void OnAfterDelete()
        {
            if (m_Timer != null)
                m_Timer.Stop();

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write(0); // version

            writer.Write(m_Decays);

            if (m_Decays)
                writer.WriteDeltaTime(m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        m_Decays = reader.ReadBool();

                        if (m_Decays)
                        {
                            m_DecayTime = reader.ReadDeltaTime();

                            m_Timer = new InternalTimer(this, m_DecayTime);
                            m_Timer.Start();
                        }

                        break;
                    }
            }
        }

        private class InternalTimer : Timer
        {
            private readonly Item m_Item;
            public InternalTimer(Item item, DateTime end)
                : base(end - DateTime.UtcNow)
            {
                m_Item = item;
            }

            protected override void OnTick()
            {
                m_Item.Delete();
            }
        }
    }
}
