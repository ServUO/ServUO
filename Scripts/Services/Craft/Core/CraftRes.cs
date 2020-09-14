using System;

namespace Server.Engines.Craft
{
    public class CraftRes
    {
        private readonly Type m_Type;
        private readonly int m_Amount;
        private readonly string m_MessageString;
        private readonly int m_MessageNumber;
        private readonly string m_NameString;
        private readonly int m_NameNumber;
        public CraftRes(Type type, int amount)
        {
            m_Type = type;
            m_Amount = amount;
        }

        public CraftRes(Type type, TextDefinition name, int amount, TextDefinition message)
            : this(type, amount)
        {
            m_NameNumber = name;
            m_MessageNumber = message;

            m_NameString = name;
            m_MessageString = message;
        }

        public Type ItemType => m_Type;

        public string MessageString => m_MessageString;

        public int MessageNumber => m_MessageNumber;

        public string NameString => m_NameString;

        public int NameNumber => m_NameNumber;

        public int Amount => m_Amount;

        public void SendMessage(Mobile from)
        {
            if (m_MessageNumber > 0)
                from.SendLocalizedMessage(m_MessageNumber);
            else if (!string.IsNullOrEmpty(m_MessageString))
                from.SendMessage(m_MessageString);
            else
                from.SendLocalizedMessage(502925); // You don't have the resources required to make that item.
        }
    }
}