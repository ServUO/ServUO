using System;

namespace Server.Engines.Craft
{
    public class CraftSubRes
    {
        private readonly Type m_Type;
        private readonly double m_ReqSkill;
        private readonly string m_NameString;
        private readonly int m_NameNumber;
        private readonly int m_GenericNameNumber;
        private readonly object m_Message;
        public CraftSubRes(Type type, TextDefinition name, double reqSkill, object message)
            : this(type, name, reqSkill, 0, message)
        {
        }

        public CraftSubRes(Type type, TextDefinition name, double reqSkill, int genericNameNumber, object message)
        {
            this.m_Type = type;
            this.m_NameNumber = name;
            this.m_NameString = name;
            this.m_ReqSkill = reqSkill;
            this.m_GenericNameNumber = genericNameNumber;
            this.m_Message = message;
        }

        public Type ItemType
        {
            get
            {
                return this.m_Type;
            }
        }
        public string NameString
        {
            get
            {
                return this.m_NameString;
            }
        }
        public int NameNumber
        {
            get
            {
                return this.m_NameNumber;
            }
        }
        public int GenericNameNumber
        {
            get
            {
                return this.m_GenericNameNumber;
            }
        }
        public object Message
        {
            get
            {
                return this.m_Message;
            }
        }
        public double RequiredSkill
        {
            get
            {
                return this.m_ReqSkill;
            }
        }
    }
}