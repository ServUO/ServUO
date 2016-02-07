using System;
using System.Collections.Generic;

namespace Server.Factions
{
    public class GuardList
    {
        private readonly GuardDefinition m_Definition;
        private readonly List<BaseFactionGuard> m_Guards;
        public GuardList(GuardDefinition definition)
        {
            this.m_Definition = definition;
            this.m_Guards = new List<BaseFactionGuard>();
        }

        public GuardDefinition Definition
        {
            get
            {
                return this.m_Definition;
            }
        }
        public List<BaseFactionGuard> Guards
        {
            get
            {
                return this.m_Guards;
            }
        }
        public BaseFactionGuard Construct()
        {
            try
            {
                return Activator.CreateInstance(this.m_Definition.Type) as BaseFactionGuard;
            }
            catch
            {
                return null;
            }
        }
    }
}