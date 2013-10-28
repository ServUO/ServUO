using System;
using Server.Mobiles;

namespace Server.Engines.XmlSpawner2
{
    public class XmlSocketable : XmlAttachment
    {
        private int m_MaxSockets;
        private SkillName m_RequiredSkill = XmlSockets.DefaultSocketSkill;
        private double m_MinSkillLevel = XmlSockets.DefaultSocketDifficulty;
        private SkillName m_RequiredSkill2 = XmlSockets.DefaultSocketSkill;
        private double m_MinSkillLevel2 = 0;// second skill requirement turned off by default
        private Type m_RequiredResource = XmlSockets.DefaultSocketResource;
        private int m_ResourceQuantity = XmlSockets.DefaultSocketResourceQuantity;
        // a serial constructor is REQUIRED
        public XmlSocketable(ASerial serial)
            : base(serial)
        {
        }

        [Attachable]
        public XmlSocketable(int maxsockets, string skillname, double minskilllevel, string skillname2, double minskilllevel2, string resource, int quantity)
        {
            this.MaxSockets = maxsockets;

            try
            {
                this.RequiredResource = SpawnerType.GetType(resource);
            }
            catch
            {
            }

            try
            {
                this.RequiredSkill = (SkillName)Enum.Parse(typeof(SkillName), skillname);
            }
            catch
            {
            }

            this.MinSkillLevel = minskilllevel;
            
            try
            {
                this.RequiredSkill2 = (SkillName)Enum.Parse(typeof(SkillName), skillname2);
            }
            catch
            {
            }

            this.MinSkillLevel2 = minskilllevel2;

            this.ResourceQuantity = quantity;
            this.InvalidateParentProperties();
        }

        [Attachable]
        public XmlSocketable(int maxsockets, string skillname, double minskilllevel, string resource, int quantity)
        {
            this.MaxSockets = maxsockets;

            try
            {
                this.RequiredResource = SpawnerType.GetType(resource);
            }
            catch
            {
            }

            try
            {
                this.RequiredSkill = (SkillName)Enum.Parse(typeof(SkillName), skillname);
            }
            catch
            {
            }

            this.MinSkillLevel = minskilllevel;
            this.ResourceQuantity = quantity;
            this.InvalidateParentProperties();
        }

        [Attachable]
        public XmlSocketable(int maxsockets)
        {
            this.MaxSockets = maxsockets;
            this.InvalidateParentProperties();
        }

        [Attachable]
        public XmlSocketable()
        {
            this.MaxSockets = -1;
            this.InvalidateParentProperties();
        }

        public XmlSocketable(int maxsockets, SkillName skillname, double minskilllevel, Type resource, int quantity)
        {
            this.MaxSockets = maxsockets;
            this.RequiredResource = resource;
            this.ResourceQuantity = quantity;
            this.RequiredSkill = skillname;
            this.MinSkillLevel = minskilllevel;
            this.InvalidateParentProperties();
        }

        public XmlSocketable(int maxsockets, SkillName skillname, double minskilllevel, SkillName skillname2, double minskilllevel2, Type resource, int quantity)
        {
            this.MaxSockets = maxsockets;
            this.RequiredResource = resource;
            this.ResourceQuantity = quantity;
            this.RequiredSkill = skillname;
            this.MinSkillLevel = minskilllevel;
            this.RequiredSkill2 = skillname2;
            this.MinSkillLevel2 = minskilllevel2;
            this.InvalidateParentProperties();
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxSockets
        {
            get
            {
                return this.m_MaxSockets;
            }
            set
            {
                this.m_MaxSockets = value;
                this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName RequiredSkill
        {
            get
            {
                return this.m_RequiredSkill;
            }
            set
            {
                this.m_RequiredSkill = value;
                this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkillLevel
        {
            get
            {
                return this.m_MinSkillLevel;
            }
            set
            {
                this.m_MinSkillLevel = value;
                this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName RequiredSkill2
        {
            get
            {
                return this.m_RequiredSkill2;
            }
            set
            {
                this.m_RequiredSkill2 = value;
                this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public double MinSkillLevel2
        {
            get
            {
                return this.m_MinSkillLevel2;
            }
            set
            {
                this.m_MinSkillLevel2 = value;
                this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public int ResourceQuantity
        {
            get
            {
                return this.m_ResourceQuantity;
            }
            set
            {
                this.m_ResourceQuantity = value;
                this.InvalidateParentProperties();
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public Type RequiredResource
        {
            get
            {
                return this.m_RequiredResource;
            }
            set
            {
                this.m_RequiredResource = value;
                this.InvalidateParentProperties();
            }
        }
        // These are the various ways in which the message attachment can be constructed.  
        // These can be called via the [addatt interface, via scripts, via the spawner ATTACH keyword.
        // Other overloads could be defined to handle other types of arguments
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)1);
            // version 1
            writer.Write((int)this.m_RequiredSkill2);
            writer.Write(this.m_MinSkillLevel2);
            // version 0
            writer.Write(this.m_MaxSockets);
            if (this.m_RequiredResource != null)
                writer.Write(this.m_RequiredResource.ToString());
            else
                writer.Write((string)null);
            writer.Write(this.m_ResourceQuantity);
            writer.Write((int)this.m_RequiredSkill);
            writer.Write(this.m_MinSkillLevel);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
            switch(version)
            {
                case 1:
                    // version 1
                    this.m_RequiredSkill2 = (SkillName)reader.ReadInt();
                    this.m_MinSkillLevel2 = reader.ReadDouble();
                    goto case 0;
                case 0:
                    // version 0
                    this.m_MaxSockets = reader.ReadInt();
                    string resourcetype = reader.ReadString();
                    try
                    {
                        this.m_RequiredResource = Type.GetType(resourcetype);
                    }
                    catch
                    {
                    }
                    this.m_ResourceQuantity = reader.ReadInt();
                    this.m_RequiredSkill = (SkillName)reader.ReadInt();
                    this.m_MinSkillLevel = reader.ReadDouble();
                    break;
            }
        }

        public override void OnAttach()
        {
            base.OnAttach();
		
            this.InvalidateParentProperties();
        }

        public override string OnIdentify(Mobile from)
        {
            string msg = null;
            int nSockets = 0;

            // first see if the target has any existing sockets

            XmlSockets s = XmlAttach.FindAttachment(this.AttachedTo, typeof(XmlSockets)) as XmlSockets;

            if (s != null)
            {
                // find out how many sockets it has
                nSockets = s.NSockets;
            }

            if (nSockets > this.MaxSockets)
                this.m_MaxSockets = nSockets;

            if (this.MaxSockets == nSockets)
            {
                // already full so no chance of socketing
                return "Cannot be socketed any further.";
            }

            if (this.MaxSockets > 0)
            {
                msg = String.Format("Maximum sockets allowed is {0}.", this.MaxSockets);
            }
            else if (this.MaxSockets == 0)
            {
                return "You cannot add sockets to this.";
            }
            else if (this.MaxSockets == -1)
            {
                msg = String.Format("Can be socketed.");
            }
                
            // compute difficulty based upon existing sockets

            if (from != null)
            {
                if (this.MinSkillLevel > 0)
                    msg += String.Format("\nRequires {0} skill in {1} to socket.", this.MinSkillLevel, this.RequiredSkill);

                if (this.MinSkillLevel2 > 0)
                    msg += String.Format("\nas well as {0} skill in {1} to socket.", this.MinSkillLevel2, this.RequiredSkill2);

                if (this.RequiredResource != null && this.ResourceQuantity > 0)
                    msg += String.Format("\nSocketing consumes {0} {1}.", this.ResourceQuantity, this.RequiredResource.Name);

                int success = XmlSockets.ComputeSuccessChance(from, nSockets, this.MinSkillLevel, this.RequiredSkill, this.MinSkillLevel2, this.RequiredSkill2);

                msg += String.Format("\n{0}% chance of socketing\n", success);
            }

            return msg;
        }
    }
}