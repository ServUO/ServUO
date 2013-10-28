using System;

namespace Server.Engines.XmlSpawner2
{
    public class MobFactionRegion : Region
    {
        private XmlMobFactions.GroupTypes m_FactionType;
        private int m_FactionLevel;
        private DateTime m_lastmsg;
        private Point3D m_EjectLocation;
        private Map m_EjectMap;
        public MobFactionRegion(string name, Map map, int priority, params Rectangle3D[] area)
            : base(name, map, priority, area)
        {
        }

        public XmlMobFactions.GroupTypes FactionType
        {
            get
            {
                return this.m_FactionType;
            }
            set
            {
                this.m_FactionType = value;
            }
        }
        public int FactionLevel
        {
            get
            {
                return this.m_FactionLevel;
            }
            set
            {
                this.m_FactionLevel = value;
            }
        }
        public Point3D EjectLocation
        {
            get
            {
                return this.m_EjectLocation;
            }
            set
            {
                this.m_EjectLocation = value;
            }
        }
        public Map EjectMap
        {
            get
            {
                return this.m_EjectMap;
            }
            set
            {
                this.m_EjectMap = value;
            }
        }
        public override bool OnMoveInto(Mobile m, Direction d, Point3D newLocation, Point3D oldLocation)
        {
            if (m.AccessLevel > AccessLevel.Player || this.Contains(oldLocation))
                return true;

            // do they have enough faction to enter?
            XmlMobFactions a = (XmlMobFactions)XmlAttach.FindAttachment(m, typeof(XmlMobFactions));
            
            if (a == null)
                return false;
            
            int fac = a.GetFactionLevel(this.m_FactionType);
            
            if (fac < this.FactionLevel)
            {
                // throttle message display
                if (DateTime.UtcNow - this.m_lastmsg > TimeSpan.FromSeconds(1))
                {
                    m.SendMessage("Your {0} faction is too low to enter here", this.FactionType);
                    this.m_lastmsg = DateTime.UtcNow;
                }
                return false;
            }

            return true;
        }

        public void KickOut(Mobile m)
        {
            if (m == null || this.EjectMap == null)
                return;

            m.SendMessage("Your {0} faction is too low to enter here", this.FactionType);
            m.MoveToWorld(this.EjectLocation, this.EjectMap);
            Effects.SendLocationParticles(Server.Items.EffectItem.Create(m.Location, m.Map, Server.Items.EffectItem.DefaultDuration), 0x3728, 10, 10, 2023);
        }

        public override void OnEnter(Mobile m)
        {
            if (m == null || m.AccessLevel > AccessLevel.Player)
                return;

            // do they have enough faction to enter?
            XmlMobFactions a = (XmlMobFactions)XmlAttach.FindAttachment(m, typeof(XmlMobFactions));
            
            if (a == null)
            {
                // kick them out
                this.KickOut(m);
                return;
            }
            
            int fac = a.GetFactionLevel(this.m_FactionType);
            
            if (fac < this.FactionLevel)
            {
                this.KickOut(m);
            }
        }

        public override void OnExit(Mobile m)
        {
        }
    }
}