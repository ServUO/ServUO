using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlMorph : XmlAttachment
    {
        private string m_Word = null;// no word activation by default
        private int m_OriginalID = -1;// default value indicating that it has not been morphed
        private int m_MorphID;
        private int proximityrange = 2;// default movement activation from 5 tiles away
        private TimeSpan m_Duration = TimeSpan.FromSeconds(30.0);// default 30 second duration
        private MorphTimer m_MorphTimer;
        private DateTime m_MorphEnd;
        // a serial constructor is REQUIRED
        public XmlMorph(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlMorph(int morphID)
        {
            this.m_MorphID = morphID;
        }

        [Attachable]
        public XmlMorph(int morphID, double duration)
        {
            this.m_MorphID = morphID;
            this.m_Duration = TimeSpan.FromMinutes(duration);
        }

        [Attachable]
        public XmlMorph(int morphID, double duration, string word)
        {
            this.m_MorphID = morphID;
            this.m_Duration = TimeSpan.FromMinutes(duration);
            this.ActivationWord = word;
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MorphID
        {
            get
            {
                return this.m_MorphID;
            }
            set
            {
                this.m_MorphID = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan Duration
        {
            get
            {
                return this.m_Duration;
            }
            set
            {
                this.m_Duration = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public DateTime MorphEnd
        {
            get
            {
                return this.m_MorphEnd;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public string ActivationWord
        {
            get
            {
                return this.m_Word;
            }
            set
            {
                this.m_Word = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int Range
        {
            get
            {
                return this.proximityrange;
            }
            set
            {
                this.proximityrange = value;
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
                return (this.ActivationWord == null);
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            // version 1
            writer.Write(this.proximityrange);
            // version 0
            writer.Write(this.m_OriginalID);
            writer.Write(this.m_MorphID);
            writer.Write(this.m_Duration);
            writer.Write(this.m_Word);
            if (this.m_MorphTimer != null)
            {
                writer.Write(this.m_MorphEnd - DateTime.UtcNow);
            }
            else
            {
                writer.Write(TimeSpan.Zero);
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch(version)
            {
                case 1:
                    this.Range = reader.ReadInt();
                    goto case 0;
                case 0:
                    // version 0
    
                    this.m_OriginalID = reader.ReadInt();
                    this.m_MorphID = reader.ReadInt();
                    this.m_Duration = reader.ReadTimeSpan();
                    this.ActivationWord = reader.ReadString();
                    TimeSpan remaining = (TimeSpan)reader.ReadTimeSpan();
    
                    if (remaining > TimeSpan.Zero)
                        this.DoTimer(remaining);
                    break;
            }
        }

        public override string OnIdentify(Mobile from)
        {
            base.OnIdentify(from);

            if (from == null || from.IsPlayer())
                return null;

            string msg = null;

            if (this.Expiration > TimeSpan.Zero)
            {
                msg = String.Format("Morph to {0} expires in {1} mins", this.m_MorphID, this.Expiration.TotalMinutes);
            }
            else
            {
                msg = String.Format("Morph to {0} duration {1} mins", this.m_MorphID, this.m_Duration.TotalMinutes);
            }

            if (this.ActivationWord != null)
            {
                return String.Format("{0} activated by '{1}'", msg, this.ActivationWord);
            }
            else
            {
                return msg;
            }
        }

        public override void OnDelete()
        {
            base.OnDelete();

            // remove the mod
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).BodyMod = this.m_OriginalID;
            }
            else if (this.AttachedTo is Item)
            {
                ((Item)this.AttachedTo).ItemID = this.m_OriginalID;
            }
        }

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);
		    
            if (e.Mobile == null || e.Mobile.AccessLevel > AccessLevel.Player)
                return;
		    
            // dont respond to other players speech if this is attached to a mob
            if (this.AttachedTo is Mobile && (Mobile)this.AttachedTo != e.Mobile)
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

            if (this.AttachedTo is Item && (((Item)this.AttachedTo).Parent == null) && Utility.InRange(e.Mobile.Location, ((Item)this.AttachedTo).Location, this.proximityrange))
            {
                this.OnTrigger(null, e.Mobile);
            }
            else
                return;
        }

        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod immediately if attached to a mob
            if (this.AttachedTo is Mobile)
            {
                Mobile m = this.AttachedTo as Mobile;
                this.m_OriginalID = m.BodyMod;
                m.BodyMod = this.m_MorphID;
                this.Expiration = this.m_Duration;
            }
        }

        public override void OnReattach()
        {
            base.OnReattach();

            // reapply the mod if attached to a mob
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).BodyMod = this.m_MorphID;
            }
        }

        public override void OnTrigger(object activator, Mobile m)
        {
            if (m == null)
                return;
            
            // if attached to an item then morph and then reset after duration
            // note that OriginalID will be -1 if the target is not already morphed
            if (this.AttachedTo is Item && this.m_OriginalID == -1)
            {
                Item i = this.AttachedTo as Item;
                this.m_OriginalID = i.ItemID;
                i.ItemID = this.m_MorphID;

                // start the timer to reset the ID
                this.DoTimer(this.m_Duration);
            }
        }

        // ----------------------------------------------
        // Private methods
        // ----------------------------------------------
        private void DoTimer(TimeSpan delay)
        {
            this.m_MorphEnd = DateTime.UtcNow + delay;

            if (this.m_MorphTimer != null)
                this.m_MorphTimer.Stop();

            this.m_MorphTimer = new MorphTimer(this, delay);
            this.m_MorphTimer.Start();
        }

        // a timer that can be implement limited lifetime morph
        private class MorphTimer : Timer
        {
            private readonly XmlMorph m_Attachment;
            public MorphTimer(XmlMorph attachment, TimeSpan delay)
                : base(delay)
            {
                this.Priority = TimerPriority.OneSecond;

                this.m_Attachment = attachment;
            }

            protected override void OnTick()
            {
                if (this.m_Attachment != null && !this.m_Attachment.Deleted && this.m_Attachment.AttachedTo is Item && !((Item)this.m_Attachment.AttachedTo).Deleted)
                {
                    Item i = this.m_Attachment.AttachedTo as Item;
                    i.ItemID = this.m_Attachment.m_OriginalID;
                    this.m_Attachment.m_OriginalID = -1;
                }
            }
        }
    }
}