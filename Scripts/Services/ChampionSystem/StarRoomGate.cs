using System;

namespace Server.Items
{
    public class StarRoomGate : Moongate
    {
        private bool m_Decays;
        private DateTime m_DecayTime;
        private Timer m_Timer;
        [Constructable]
        public StarRoomGate()
            : this(false)
        {
        }

        [Constructable]
        public StarRoomGate(bool decays, Point3D loc, Map map)
            : this(decays)
        {
            this.MoveToWorld(loc, map);
            Effects.PlaySound(loc, map, 0x20E);
        }

        [Constructable]
        public StarRoomGate(bool decays)
            : base(new Point3D(5143, 1774, 0), Map.Felucca)
        {
            this.Dispellable = false;
            this.ItemID = 0x1FD4;

            if (decays)
            {
                this.m_Decays = true;
                this.m_DecayTime = DateTime.UtcNow + TimeSpan.FromMinutes(2.0);

                this.m_Timer = new InternalTimer(this, this.m_DecayTime);
                this.m_Timer.Start();
            }
        }

        public StarRoomGate(Serial serial)
            : base(serial)
        {
        }

        public override int LabelNumber
        {
            get
            {
                return 1049498;
            }
        }// dark moongate
        public override void OnAfterDelete()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            base.OnAfterDelete();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write(this.m_Decays);

            if (this.m_Decays)
                writer.WriteDeltaTime(this.m_DecayTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_Decays = reader.ReadBool();

                        if (this.m_Decays)
                        {
                            this.m_DecayTime = reader.ReadDeltaTime();

                            this.m_Timer = new InternalTimer(this, this.m_DecayTime);
                            this.m_Timer.Start();
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
                this.m_Item = item;
            }

            protected override void OnTick()
            {
                this.m_Item.Delete();
            }
        }
    }
}