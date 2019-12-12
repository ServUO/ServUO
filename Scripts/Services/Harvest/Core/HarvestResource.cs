using System;

namespace Server.Engines.Harvest
{
    public class HarvestResource
    {
        private readonly object m_SuccessMessage;
        private Type[] m_Types;
        private double m_ReqSkill, m_MinSkill, m_MaxSkill;
        public HarvestResource(double reqSkill, double minSkill, double maxSkill, object message, params Type[] types)
        {
            this.m_ReqSkill = reqSkill;
            this.m_MinSkill = minSkill;
            this.m_MaxSkill = maxSkill;
            this.m_Types = types;
            this.m_SuccessMessage = message;
        }

        public Type[] Types
        {
            get
            {
                return this.m_Types;
            }
            set
            {
                this.m_Types = value;
            }
        }
        public double ReqSkill
        {
            get
            {
                return this.m_ReqSkill;
            }
            set
            {
                this.m_ReqSkill = value;
            }
        }
        public double MinSkill
        {
            get
            {
                return this.m_MinSkill;
            }
            set
            {
                this.m_MinSkill = value;
            }
        }
        public double MaxSkill
        {
            get
            {
                return this.m_MaxSkill;
            }
            set
            {
                this.m_MaxSkill = value;
            }
        }
        public object SuccessMessage
        {
            get
            {
                return this.m_SuccessMessage;
            }
        }
        public void SendSuccessTo(Mobile m)
        {
            if (this.m_SuccessMessage is int)
                m.SendLocalizedMessage((int)this.m_SuccessMessage);
            else if (this.m_SuccessMessage is string)
                m.SendMessage((string)this.m_SuccessMessage);
        }
    }
}