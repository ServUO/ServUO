using System;

namespace Server.Items
{
    public abstract class BaseArtifactLight : BaseDecorationArtifact
    {
        private Timer m_ArtifactTimer;
        private DateTime m_ArtifactEnd;
        private bool m_Burning = false;
        private TimeSpan m_Duration = TimeSpan.Zero;

        public BaseArtifactLight(int itemID)
            : base(itemID)
        {
        }

        public BaseArtifactLight(Serial serial)
            : base(serial)
        {
        }
		
		public abstract int LitItemID { get; }
        public virtual int UnlitItemID { get { return 0; } }
		public virtual int BurntOutItemID { get { return 0; } }
        public virtual int LitSound { get { return 0x47; } }
        public virtual int UnlitSound { get { return 0x3be; } }
        public virtual int BurntOutSound { get { return 0x4b8; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Burning
        {
            get { return m_Burning; }
            set
            {
                if (m_Burning != value)
                {
                    m_Burning = true;
                    DoTimer(m_Duration);
                }
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool BurntOut { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Protected { get; set; }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Duration
        {
            get { return m_Duration != TimeSpan.Zero && m_Burning ? m_ArtifactEnd - DateTime.UtcNow : m_Duration; }
            set { m_Duration = value; }
        }

        public virtual void PlayLitSound()
        {
            if (LitSound != 0)
            {
                Point3D loc = GetWorldLocation();
                Effects.PlaySound(loc, Map, LitSound);
            }
        }

        public virtual void PlayUnlitSound()
        {
            int sound = UnlitSound;

            if (BurntOut && BurntOutSound != 0)
                sound = BurntOutSound;

            if (sound != 0)
            {
                Point3D loc = GetWorldLocation();
                Effects.PlaySound(loc, Map, sound);
            }
        }

        public virtual void Ignite()
        {
            if (!BurntOut)
            {
                PlayLitSound();

                m_Burning = true;
                ItemID = LitItemID;
                DoTimer(m_Duration);
            }
        }

		public virtual void Douse()
        {
            m_Burning = false;

            if (BurntOut && BurntOutItemID != 0)
            {
                ItemID = BurntOutItemID;
            }
            else
            {
                ItemID = UnlitItemID;
            }

            if (BurntOut)
            {
                m_Duration = TimeSpan.Zero;
            }
            else if (m_Duration != TimeSpan.Zero)
            {
                m_Duration = m_ArtifactEnd - DateTime.UtcNow;
            }

            if (m_ArtifactTimer != null)
            {
                m_ArtifactTimer.Stop();
            }

            PlayUnlitSound();
        }

		public virtual void Burn()
        {
            BurntOut = true;
            Douse();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (BurntOut)
                return;

            if (Protected && from.IsPlayer())
                return;

            if (!from.InRange(GetWorldLocation(), 2))
                return;

            if (m_Burning)
            {
                if (UnlitItemID != 0)
                    Douse();
            }
            else
            {
                Ignite();
            }
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);

            writer.Write(BurntOut);
            writer.Write(m_Burning);
            writer.Write(m_Duration);
            writer.Write(Protected);

            if (m_Burning && m_Duration != TimeSpan.Zero)
                writer.WriteDeltaTime(m_ArtifactEnd);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();

            switch ( version )
            {
                case 0:
                    {
                        BurntOut = reader.ReadBool();
                        m_Burning = reader.ReadBool();
                        m_Duration = reader.ReadTimeSpan();
                        Protected = reader.ReadBool();

                        if (m_Burning && m_Duration != TimeSpan.Zero)
                            DoTimer(reader.ReadDeltaTime() - DateTime.UtcNow);

                        break;
                    }
            }
        }

        private void DoTimer(TimeSpan delay)
        {
            m_Duration = delay;
			
            if (m_ArtifactTimer != null)
                m_ArtifactTimer.Stop();

            if (delay == TimeSpan.Zero)
                return;

            m_ArtifactEnd = DateTime.UtcNow + delay;

            m_ArtifactTimer = new InternalTimer(this, delay);
            m_ArtifactTimer.Start();
        }

        private class InternalTimer : Timer
        {
            private readonly BaseArtifactLight m_Light;
            public InternalTimer(BaseArtifactLight light, TimeSpan delay)
                : base(delay)
            {
                m_Light = light;
                Priority = TimerPriority.FiveSeconds;
            }

            protected override void OnTick()
            {
                if (m_Light != null && !m_Light.Deleted)
                    m_Light.Burn();
            }
        }
    }
}