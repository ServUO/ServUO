using System;

namespace Server.Engines.Harvest
{
    public class BonusHarvestResource
    {
        private readonly TextDefinition m_SuccessMessage;
        private Type m_Type;
        private double m_ReqSkill, m_Chance;
		public BonusHarvestResource(double reqSkill, double chance, TextDefinition message, Type type)
			: this(reqSkill, chance, message, type, null)
		{ }
        public BonusHarvestResource(double reqSkill, double chance, TextDefinition message, Type type, Map requiredMap)
        {
            this.m_ReqSkill = reqSkill;

            this.m_Chance = chance;
            this.m_Type = type;
            this.m_SuccessMessage = message;
			RequiredMap = requiredMap;
        }

		public Map RequiredMap { get; private set; }

        public Type Type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                this.m_Type = value;
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
        public double Chance
        {
            get
            {
                return this.m_Chance;
            }
            set
            {
                this.m_Chance = value;
            }
        }
        public TextDefinition SuccessMessage
        {
            get
            {
                return this.m_SuccessMessage;
            }
        }
        public void SendSuccessTo(Mobile m)
        {
            TextDefinition.SendMessageTo(m, this.m_SuccessMessage);
        }
    }
}