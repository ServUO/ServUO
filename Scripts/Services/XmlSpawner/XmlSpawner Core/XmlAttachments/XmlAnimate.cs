using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlAnimate : XmlAttachment
    {
        private int m_AnimationValue = 0;// default animation
        private int m_FrameCount = 7;// default framecount
        private int m_RepeatCount = 1;// default repeatcount
        private int m_AnimationDelay = 0;// default animation delay
        private bool m_Repeat = false;// default repeat
        private bool m_Forward = true;// default animation direction
        private string m_ActivationWord = null;// no word activation by default
        private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);// 5 seconds default time between activations
        private DateTime m_EndTime;
        private int m_ProximityRange = 5;// default movement activation from 5 tiles away
        private LoopTimer m_Timer;
        private int m_LoopCount = 0;// repeat animations using a timed loop
        private int m_LoopDelay = 5;// interval in seconds between loop ticks
        private int m_CurrentCount = 0;
        // a serial constructor is REQUIRED
        public XmlAnimate(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlAnimate()
        {
        }

        [Attachable]
        public XmlAnimate(int animation)
        {
            this.AnimationValue = animation;
        }

        [Attachable]
        public XmlAnimate(int animation, double refractory)
        {
            this.AnimationValue = animation;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [Attachable]
        public XmlAnimate(int animation, int framecount, double refractory)
        {
            this.AnimationValue = animation;
            this.FrameCount = framecount;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [Attachable]
        public XmlAnimate(int animation, double refractory, int loopcount, int loopdelay)
        {
            this.LoopCount = loopcount;
            this.LoopDelay = loopdelay;
            this.AnimationValue = animation;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [Attachable]
        public XmlAnimate(int animation, int framecount, double refractory, int loopcount, int loopdelay)
        {
            this.LoopCount = loopcount;
            this.LoopDelay = loopdelay;
            this.AnimationValue = animation;
            this.FrameCount = framecount;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ProximityRange
        {
            get
            {
                return this.m_ProximityRange;
            }
            set
            {
                this.m_ProximityRange = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int FrameCount
        {
            get
            {
                return this.m_FrameCount;
            }
            set
            {
                this.m_FrameCount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int RepeatCount
        {
            get
            {
                return this.m_RepeatCount;
            }
            set
            {
                this.m_RepeatCount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AnimationDelay
        {
            get
            {
                return this.m_AnimationDelay;
            }
            set
            {
                this.m_AnimationDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Repeat
        {
            get
            {
                return this.m_Repeat;
            }
            set
            {
                this.m_Repeat = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool Forward
        {
            get
            {
                return this.m_Forward;
            }
            set
            {
                this.m_Forward = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int AnimationValue
        {
            get
            {
                return this.m_AnimationValue;
            }
            set
            {
                this.m_AnimationValue = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ActivationWord
        {
            get
            {
                return this.m_ActivationWord;
            }
            set
            {
                this.m_ActivationWord = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Refractory
        {
            get
            {
                return this.m_Refractory;
            }
            set
            {
                this.m_Refractory = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int LoopCount
        {
            get
            {
                return this.m_LoopCount;
            }
            set
            {
                this.m_LoopCount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int LoopDelay
        {
            get
            {
                return this.m_LoopDelay;
            }
            set
            {
                this.m_LoopDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int CurrentCount
        {
            get
            {
                return this.m_CurrentCount;
            }
            set
            {
                this.m_CurrentCount = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoAnimate
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                    this.OnTrigger(null, null);
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool DoReset
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                    this.Reset();
            }
        }
        // These are the various ways in which the message attachment can be constructed.
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override bool HandlesOnSpeech
        {
            get
            {
                return (this.ActivationWord != null);
            }
        }
        public override bool HandlesOnMovement
        {
            get
            {
                return (this.ProximityRange >= 0 && this.ActivationWord == null);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);
            // version 0
            writer.Write(this.m_CurrentCount);
            writer.Write(this.m_LoopCount);
            writer.Write(this.m_LoopDelay);
            writer.Write(this.m_ProximityRange);
            writer.Write(this.m_AnimationValue);
            writer.Write(this.m_FrameCount);
            writer.Write(this.m_RepeatCount);
            writer.Write(this.m_AnimationDelay);
            writer.Write(this.m_Forward);
            writer.Write(this.m_Repeat);
            writer.Write(this.m_ActivationWord);
            writer.Write(this.m_Refractory);
            writer.Write(this.m_EndTime - DateTime.UtcNow);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    // version 0
                    this.m_CurrentCount = reader.ReadInt();
                    this.m_LoopCount = reader.ReadInt();
                    this.m_LoopDelay = reader.ReadInt();
                    this.m_ProximityRange = reader.ReadInt();
                    this.m_AnimationValue = reader.ReadInt();
                    this.m_FrameCount = reader.ReadInt();
                    this.m_RepeatCount = reader.ReadInt();
                    this.m_AnimationDelay = reader.ReadInt();
                    this.m_Forward = reader.ReadBool();
                    this.m_Repeat = reader.ReadBool();
                    this.m_ActivationWord = reader.ReadString();
                    this.m_Refractory = reader.ReadTimeSpan();
                    TimeSpan remaining = reader.ReadTimeSpan();
                    this.m_EndTime = DateTime.UtcNow + remaining;
                    break;
            }

            // restart any animation loops that were active
            if (this.CurrentCount > 0)
            {
                this.DoTimer(TimeSpan.FromSeconds(this.LoopDelay));
            }
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.AccessLevel < AccessLevel.Counselor)
                return null;

            string msg = String.Format("Animation #{0},{1} : {2} secs between uses", this.AnimationValue, this.FrameCount, this.Refractory.TotalSeconds);

            if (this.ActivationWord == null)
            {
                return msg;
            }
            else
            {
                return String.Format("{0} : trigger on '{1}'", msg, this.ActivationWord);
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player)
                return;

            if (e.Speech == this.ActivationWord)
            {
                this.OnTrigger(null, e.Mobile);
            }
        }

        public override void OnMovement(MovementEventArgs e)
        {
            base.OnMovement(e);

            if (e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player)
                return;

            if (this.AttachedTo is Item && (((Item)this.AttachedTo).Parent == null) && Utility.InRange(e.Mobile.Location, ((Item)this.AttachedTo).Location, this.ProximityRange))
            {
                this.OnTrigger(null, e.Mobile);
            }
            else
                return;
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // only attach to mobiles
            if (!(this.AttachedTo is Mobile))
            {
                this.Delete();
            }
        }

        public void Reset()
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.CurrentCount = 0;
            this.m_EndTime = DateTime.UtcNow;
        }

        public void Animate()
        {
            // play a animation
            if (this.AttachedTo is Mobile && this.AnimationValue >= 0)
            {
                ((Mobile)this.AttachedTo).Animate(this.AnimationValue, this.FrameCount, this.RepeatCount, this.Forward, this.Repeat, this.AnimationDelay);
            }

            this.UpdateRefractory();

            this.CurrentCount--;
        }

        public void UpdateRefractory()
        {
            this.m_EndTime = DateTime.UtcNow + this.Refractory;
        }

        public override void OnTrigger(object activator, Mobile m)
        {
            if (DateTime.UtcNow < this.m_EndTime)
                return;

            if (this.LoopCount > 0)
            {
                this.CurrentCount = this.LoopCount;
                // check to make sure the timer is running
                this.DoTimer(TimeSpan.FromSeconds(this.LoopDelay));
            }
            else
            {
                this.Animate();
            }
        }

        private void DoTimer(TimeSpan delay)
        {
            if (this.m_Timer != null)
                this.m_Timer.Stop();

            this.m_Timer = new LoopTimer(this, delay);
            this.m_Timer.Start();
        }

        private class LoopTimer : Timer
        {
            public readonly TimeSpan m_delay;
            private readonly XmlAnimate m_attachment;
            public LoopTimer(XmlAnimate attachment, TimeSpan delay)
                : base(delay, delay)
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_attachment = attachment;
                this.m_delay = delay;
            }

            protected override void OnTick()
            {
                if (this.m_attachment != null && !this.m_attachment.Deleted)
                {
                    this.m_attachment.Animate();

                    if (this.m_attachment.CurrentCount <= 0)
                        this.Stop();
                }
                else
                {
                    this.Stop();
                }
            }
        }
    }
}