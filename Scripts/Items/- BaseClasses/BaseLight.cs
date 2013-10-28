using System;

namespace Server.Items
{
    public abstract class BaseLight : Item
    {
        public static readonly bool Burnout = false;
        private Timer m_Timer;
        private DateTime m_End;
        private bool m_BurntOut = false;
        private bool m_Burning = false;
        private bool m_Protected = false;
        private TimeSpan m_Duration = TimeSpan.Zero;
        [Constructable]
        public BaseLight(int itemID)
            : base(itemID)
        {
        }

        public BaseLight(Serial serial)
            : base(serial)
        {
        }

        public abstract int LitItemID { get; }
        public virtual int UnlitItemID
        {
            get
            {
                return 0;
            }
        }
        public virtual int BurntOutItemID
        {
            get
            {
                return 0;
            }
        }
        public virtual int LitSound
        {
            get
            {
                return 0x47;
            }
        }
        public virtual int UnlitSound
        {
            get
            {
                return 0x3be;
            }
        }
        public virtual int BurntOutSound
        {
            get
            {
                return 0x4b8;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Burning
        {
            get
            {
                return this.m_Burning;
            }
            set
            {
                if (this.m_Burning != value)
                {
                    this.m_Burning = true;
                    this.DoTimer(this.m_Duration);
                }
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool BurntOut
        {
            get
            {
                return this.m_BurntOut;
            }
            set
            {
                this.m_BurntOut = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Protected
        {
            get
            {
                return this.m_Protected;
            }
            set
            {
                this.m_Protected = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Duration
        {
            get
            {
                if (this.m_Duration != TimeSpan.Zero && this.m_Burning)
                {
                    return this.m_End - DateTime.UtcNow;
                }
                else
                    return this.m_Duration;
            }

            set
            {
                this.m_Duration = value;
            }
        }
        public virtual void PlayLitSound()
        {
            if (this.LitSound != 0)
            {
                Point3D loc = this.GetWorldLocation();
                Effects.PlaySound(loc, this.Map, this.LitSound);
            }
        }

        public virtual void PlayUnlitSound()
        {
            int sound = this.UnlitSound;

            if (this.m_BurntOut && this.BurntOutSound != 0)
                sound = this.BurntOutSound;

            if (sound != 0)
            {
                Point3D loc = this.GetWorldLocation();
                Effects.PlaySound(loc, this.Map, sound);
            }
        }

        public virtual void Ignite()
        {
            if (!this.m_BurntOut)
            {
                this.PlayLitSound();

                this.m_Burning = true;
                this.ItemID = this.LitItemID;
                this.DoTimer(this.m_Duration);
            }
        }

        public virtual void Douse()
        {
            this.m_Burning = false;
			
            if (this.m_BurntOut && this.BurntOutItemID != 0)
                this.ItemID = this.BurntOutItemID;
            else
                this.ItemID = this.UnlitItemID;

            if (this.m_BurntOut)
                this.m_Duration = TimeSpan.Zero;
            else if (this.m_Duration != TimeSpan.Zero)
                this.m_Duration = this.m_End - DateTime.UtcNow;

            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.PlayUnlitSound();
        }

        public virtual void Burn()
        {
            this.m_BurntOut = true;
            this.Douse();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (this.m_BurntOut)
                return;

            if (this.m_Protected && from.IsPlayer())
                return;

            if (!from.InRange(this.GetWorldLocation(), 2))
                return;

            if (this.m_Burning)
            {
                if (this.UnlitItemID != 0)
                    this.Douse();
            }
            else
            {
                this.Ignite();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            writer.Write(this.m_BurntOut);
            writer.Write(this.m_Burning);
            writer.Write(this.m_Duration);
            writer.Write(this.m_Protected);

            if (this.m_Burning && this.m_Duration != TimeSpan.Zero)
                writer.WriteDeltaTime(this.m_End);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        this.m_BurntOut = reader.ReadBool();
                        this.m_Burning = reader.ReadBool();
                        this.m_Duration = reader.ReadTimeSpan();
                        this.m_Protected = reader.ReadBool();

                        if (this.m_Burning && this.m_Duration != TimeSpan.Zero)
                            this.DoTimer(reader.ReadDeltaTime() - DateTime.UtcNow);

                        break;
                    }
            }
        }

        private void DoTimer(TimeSpan delay)
        {
            this.m_Duration = delay;
			
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            if (delay == TimeSpan.Zero)
                return;

            this.m_End = DateTime.UtcNow + delay;

            this.m_Timer = new InternalTimer(this, delay);
            this.m_Timer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly BaseLight m_Light;
            public InternalTimer(BaseLight light, TimeSpan delay)
                : base(delay)
            {
                this.m_Light = light;
                this.Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (this.m_Light != null && !this.m_Light.Deleted)
                    this.m_Light.Burn();
            }
        }
    }
}