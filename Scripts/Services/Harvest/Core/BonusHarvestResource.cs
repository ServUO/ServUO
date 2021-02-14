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
            m_ReqSkill = reqSkill;

            m_Chance = chance;
            m_Type = type;
            m_SuccessMessage = message;
            RequiredMap = requiredMap;
        }

        public Map RequiredMap { get; }

        public Type Type { get => m_Type; set => m_Type = value; }

        public double ReqSkill { get => m_ReqSkill; set => m_ReqSkill = value; }

        public double Chance { get => m_Chance; set => m_Chance = value; }

        public TextDefinition SuccessMessage => m_SuccessMessage;
        public void SendSuccessTo(Mobile m)
        {
            TextDefinition.SendMessageTo(m, m_SuccessMessage);
        }
    }
}
