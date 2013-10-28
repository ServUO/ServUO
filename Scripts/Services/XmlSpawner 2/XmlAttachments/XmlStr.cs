using System;

namespace Server.Engines.XmlSpawner2
{
    public class XmlStr : XmlAttachment
    {
        private readonly TimeSpan m_Duration = TimeSpan.FromSeconds(30.0);// default 30 sec duration
        private int m_Value = 10;// default value of 10

        // a serial constructor is REQUIRED
        public XmlStr(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlStr()
        {
        }

        [Attachable]
        public XmlStr(int value)
        {
            this.m_Value = value;
        }

        [Attachable]
        public XmlStr(int value, double duration)
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
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void OnAttach()
        {
            base.OnAttach();

            // apply the mod
            if (this.AttachedTo is Mobile)
            {
                ((Mobile)this.AttachedTo).AddStatMod(new StatMod(StatType.Str, "XmlStr" + this.Name, this.m_Value, this.m_Duration));
            }
            // and then remove the attachment
            Timer.DelayCall(TimeSpan.Zero, new TimerCallback(Delete));
            //Delete();
        }
    }
}