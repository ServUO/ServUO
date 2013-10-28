using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlSound : XmlAttachment
    {
        private int m_SoundValue = 500;// default sound
        private string m_Word = null;// no word activation by default
        private TimeSpan m_Refractory = TimeSpan.FromSeconds(5);// 5 seconds default time between activations
        private DateTime m_EndTime;
        private int m_Charges = 0;// no charge limit
        private int proximityrange = 5;// default movement activation from 5 tiles away

        // a serial constructor is REQUIRED
        public XmlSound(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlSound()
        {
        }

        [Attachable]
        public XmlSound(int sound)
        {
            this.SoundValue = sound;
        }

        [Attachable]
        public XmlSound(int sound, double refractory)
        {
            this.SoundValue = sound;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [Attachable]
        public XmlSound(int sound, double refractory, string word)
        {
            this.ActivationWord = word;
            this.SoundValue = sound;
            this.Refractory = TimeSpan.FromSeconds(refractory);
        }

        [Attachable]
        public XmlSound(int sound, double refractory, string word, int charges)
        {
            this.ActivationWord = word;
            this.SoundValue = sound;
            this.Refractory = TimeSpan.FromSeconds(refractory);
            this.Charges = charges;
        }

        [Attachable]
        public XmlSound(int sound, double refractory, int charges)
        {
            this.SoundValue = sound;
            this.Refractory = TimeSpan.FromSeconds(refractory);
            this.Charges = charges;
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
        [CommandProperty(AccessLevel.GameMaster)]
        public int SoundValue
        {
            get
            {
                return this.m_SoundValue;
            }
            set
            {
                this.m_SoundValue = value;
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
        public int Charges
        {
            get
            {
                return this.m_Charges;
            }
            set
            {
                this.m_Charges = value;
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
            writer.Write(this.m_SoundValue);
            writer.Write(this.m_Word);
            writer.Write(this.m_Charges);
            writer.Write(this.m_Refractory);
            writer.Write(this.m_EndTime - DateTime.UtcNow);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch(version)
            {
                case 1:
                    // version 1
                    this.proximityrange = reader.ReadInt();
                    goto case 0;
                case 0:
                    // version 0
                    this.SoundValue = reader.ReadInt();
                    this.ActivationWord = reader.ReadString();
                    this.Charges = reader.ReadInt();
                    this.Refractory = reader.ReadTimeSpan();
                    TimeSpan remaining = reader.ReadTimeSpan();
                    this.m_EndTime = DateTime.UtcNow + remaining;
                    break;
            }
        }

        public override string OnIdentify(Mobile from)
        {
            if (from == null || from.IsPlayer())
                return null;

            string msg = null;

            if (this.Charges > 0)
            {
                msg = String.Format("Sound #{0} : {1} secs between uses - {2} charges left", this.SoundValue, this.Refractory.TotalSeconds, this.Charges);
            }
            else
            {
                msg = String.Format("Sound #{0} : {1} secs between uses", this.SoundValue, this.Refractory.TotalSeconds);
            }

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

            if (this.AttachedTo is Item && (((Item)this.AttachedTo).Parent == null) && Utility.InRange(e.Mobile.Location, ((Item)this.AttachedTo).Location, this.proximityrange))
            {
                this.OnTrigger(null, e.Mobile);
            }
            else
                return;
        }

        public override void OnTrigger(object activator, Mobile m)
        {
            if (m == null)
                return;

            if (DateTime.UtcNow < this.m_EndTime)
                return;

            // play a sound
            if (this.AttachedTo is Mobile)
            {
                try
                {
                    Effects.PlaySound(((Mobile)this.AttachedTo).Location, ((IEntity)this.AttachedTo).Map, this.SoundValue);
                }
                catch
                {
                }
            }
            else if (this.AttachedTo is Item)
            {
                Item i = this.AttachedTo as Item;

                if (i.Parent == null)
                {
                    try
                    {
                        Effects.PlaySound(i.Location, i.Map, this.SoundValue);
                    }
                    catch
                    {
                    }
                }
                else if (i.RootParent is IEntity)
                {
                    try
                    {
                        Effects.PlaySound(((IEntity)i.RootParent).Location, ((IEntity)i.RootParent).Map, this.SoundValue);
                    }
                    catch
                    {
                    }
                }
            }

            this.Charges--;

            // remove the attachment either after the charges run out or if refractory is zero, then it is one use only
            if (this.Refractory == TimeSpan.Zero || this.Charges == 0)
            {
                this.Delete();
            }
            else
            {
                this.m_EndTime = DateTime.UtcNow + this.Refractory;
            }
        }
    }
}