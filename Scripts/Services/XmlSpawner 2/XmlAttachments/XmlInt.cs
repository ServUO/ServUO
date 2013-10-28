using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlInt : XmlAttachment
    {
        private readonly TimeSpan m_Duration = TimeSpan.FromSeconds(30.0);// default 30 sec duration
        private int m_Value = 10;// default value of 10

        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments

        // a serial constructor is REQUIRED
        public XmlInt(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlInt()
        {
        }

        [Attachable]
        public XmlInt(int value)
        {
            this.m_Value = value;
        }

        [Attachable]
        public XmlInt(int value, double duration)
        {
            this.m_Value = value;
            this.m_Duration = TimeSpan.FromSeconds(duration);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Value
        {
            get
            {
                return this.m_Value;
            }
            set
            {
                this.m_Value = value;
            }
        }
        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).AddStatMod(new StatMod(StatType.Int, "XmlInt" + this.Name, this.m_Value, this.m_Duration));
            }
            // and then remove the attachment
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
            //Delete();
        }
    }
}