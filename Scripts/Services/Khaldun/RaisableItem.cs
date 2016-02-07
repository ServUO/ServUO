using System;

namespace Server.Items
{
    public class RaisableItem : Item
    {
        private int m_MaxElevation;
        private int m_MoveSound;
        private int m_StopSound;
        private TimeSpan m_CloseDelay;
        private int m_Elevation;
        private RaiseTimer m_RaiseTimer;
        [Constructable]
        public RaisableItem(int itemID)
            : this(itemID, 20, -1, -1, TimeSpan.FromMinutes(1.0))
        {
        }

        [Constructable]
        public RaisableItem(int itemID, int maxElevation, TimeSpan closeDelay)
            : this(itemID, maxElevation, -1, -1, closeDelay)
        {
        }

        [Constructable]
        public RaisableItem(int itemID, int maxElevation, int moveSound, int stopSound, TimeSpan closeDelay)
            : base(itemID)
        {
            this.Movable = false;

            this.m_MaxElevation = maxElevation;
            this.m_MoveSound = moveSound;
            this.m_StopSound = stopSound;
            this.m_CloseDelay = closeDelay;
        }

        public RaisableItem(Serial serial)
            : base(serial)
        {
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxElevation
        {
            get
            {
                return this.m_MaxElevation;
            }
            set
            {
                if (value <= 0)
                    this.m_MaxElevation = 0;
                else if (value >= 60)
                    this.m_MaxElevation = 60;
                else
                    this.m_MaxElevation = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int MoveSound
        {
            get
            {
                return this.m_MoveSound;
            }
            set
            {
                this.m_MoveSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int StopSound
        {
            get
            {
                return this.m_StopSound;
            }
            set
            {
                this.m_StopSound = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan CloseDelay
        {
            get
            {
                return this.m_CloseDelay;
            }
            set
            {
                this.m_CloseDelay = value;
            }
        }
        public bool IsRaisable
        {
            get
            {
                return this.m_RaiseTimer == null;
            }
        }
        public void Raise()
        {
            if (!this.IsRaisable)
                return;

            this.m_RaiseTimer = new RaiseTimer(this);
            this.m_RaiseTimer.Start();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.WriteEncodedInt((int)0); // version

            writer.WriteEncodedInt((int)this.m_MaxElevation);
            writer.WriteEncodedInt((int)this.m_MoveSound);
            writer.WriteEncodedInt((int)this.m_StopSound);
            writer.Write((TimeSpan)this.m_CloseDelay);

            writer.WriteEncodedInt((int)this.m_Elevation);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadEncodedInt();

            this.m_MaxElevation = reader.ReadEncodedInt();
            this.m_MoveSound = reader.ReadEncodedInt();
            this.m_StopSound = reader.ReadEncodedInt();
            this.m_CloseDelay = reader.ReadTimeSpan();

            int elevation = reader.ReadEncodedInt();
            this.Z -= elevation;
        }

        private class RaiseTimer : Timer
        {
            private readonly RaisableItem m_Item;
            private readonly DateTime m_CloseTime;
            private bool m_Up;
            private int m_Step;
            public RaiseTimer(RaisableItem item)
                : base(TimeSpan.Zero, TimeSpan.FromSeconds(0.5))
            {
                this.m_Item = item;
                this.m_CloseTime = DateTime.UtcNow + item.CloseDelay;
                this.m_Up = true;

                this.Priority = TimerPriority.TenMS;
            }

            protected override void OnTick()
            {
                if (this.m_Item.Deleted)
                {
                    this.Stop();
                    return;
                }

                if (this.m_Step++ % 3 == 0)
                {
                    if (this.m_Up)
                    {
                        this.m_Item.Z++;

                        if (++this.m_Item.m_Elevation >= this.m_Item.MaxElevation)
                        {
                            this.Stop();

                            if (this.m_Item.StopSound >= 0)
                                Effects.PlaySound(this.m_Item.Location, this.m_Item.Map, this.m_Item.StopSound);

                            this.m_Up = false;
                            this.m_Step = 0;

                            TimeSpan delay = this.m_CloseTime - DateTime.UtcNow;
                            Timer.DelayCall(delay > TimeSpan.Zero ? delay : TimeSpan.Zero, new TimerCallback(Start));

                            return;
                        }
                    }
                    else
                    {
                        this.m_Item.Z--;

                        if (--this.m_Item.m_Elevation <= 0)
                        {
                            this.Stop();

                            if (this.m_Item.StopSound >= 0)
                                Effects.PlaySound(this.m_Item.Location, this.m_Item.Map, this.m_Item.StopSound);

                            this.m_Item.m_RaiseTimer = null;

                            return;
                        }
                    }
                }

                if (this.m_Item.MoveSound >= 0)
                    Effects.PlaySound(this.m_Item.Location, this.m_Item.Map, this.m_Item.MoveSound);
            }
        }
    }
}