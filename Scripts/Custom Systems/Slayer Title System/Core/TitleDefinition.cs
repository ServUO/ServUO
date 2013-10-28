using System;
using System.Collections.Generic;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class TitleDefinition
    {
        private String m_DefinitionName;
        public String DefinitionName { get { return m_DefinitionName; } }

        private Boolean m_Enabled;
        public Boolean Enabled { get { return m_Enabled; } }

        private List<Type> m_CreatureRegistry;
        public List<Type> CreatureRegistry { get { return m_CreatureRegistry; } }

        private List<TitleEntry> m_TitleRegistry;
        public List<TitleEntry> TitleRegistry { get { return m_TitleRegistry; } }

        public TitleDefinition(String definitionName, Boolean enabled, List<Type> creatureTypes, List<TitleEntry> titleEntries)
        {
            m_DefinitionName = definitionName;
            m_Enabled = enabled;
            m_CreatureRegistry = creatureTypes;
            m_TitleRegistry = titleEntries;
        }
    }
}
