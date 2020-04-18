using System;

namespace Server
{
    public class UsageAttribute : Attribute
    {
        private readonly string m_Usage;
        public UsageAttribute(string usage)
        {
            m_Usage = usage;
        }

        public string Usage => m_Usage;
    }

    public class DescriptionAttribute : Attribute
    {
        private readonly string m_Description;
        public DescriptionAttribute(string description)
        {
            m_Description = description;
        }

        public string Description => m_Description;
    }

    public class AliasesAttribute : Attribute
    {
        private readonly string[] m_Aliases;
        public AliasesAttribute(params string[] aliases)
        {
            m_Aliases = aliases;
        }

        public string[] Aliases => m_Aliases;
    }
}