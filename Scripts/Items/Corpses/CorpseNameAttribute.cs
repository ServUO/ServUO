using System;

namespace Server
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CorpseNameAttribute : Attribute
    {
        private readonly string m_Name;
        public CorpseNameAttribute(string name)
        {
            this.m_Name = name;
        }

        public string Name => this.m_Name;
    }
}