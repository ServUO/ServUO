using System;
using System.Collections.Generic;

using Server;
using Server.Commands;
using Server.Gumps;
using Server.Mobiles;

namespace CustomsFramework.Systems.SlayerTitleSystem
{
    public class SlayerTitleCore : BaseCore
    {
        #region Event Handlers
        public delegate void SlayerTitleAwardedEventHandler(Mobile from, string titleDefinitionName, string titleAwarded);

        public static event SlayerTitleAwardedEventHandler TitleAwarded;
        public static event SlayerTitleAwardedEventHandler MaximumTitleAchieved;
        #endregion

        private List<TitleDefinition> m_TitleDefinitions = new List<TitleDefinition>();
        public List<TitleDefinition> TitleDefinitions { get { return m_TitleDefinitions; } }

        private Dictionary<Type, List<Int32>> m_CrossReference = new Dictionary<Type, List<int>>();

        public const String SystemVersion = "3.1";

        private static SlayerTitleCore m_Core;
        public static SlayerTitleCore Core { get { return m_Core; } }

        public SlayerTitleCore() : base()
        {
            this.Enabled = false;
        }

        public SlayerTitleCore(CustomSerial serial) : base(serial)
        {
        }

        public override String Name { get { return "Slayer Title Core"; } }
        public override String Description { get { return "Core that contains everything for the Slayer Title System."; } }
        public override String Version { get { return SystemVersion; } }
        public override AccessLevel EditLevel { get { return AccessLevel.Developer; } }
        public override Gump SettingsGump { get { return new SlayerTitleSetupGump(); } }

        public static void Initialize()
        {
            SlayerTitleCore core = World.GetCore(typeof(SlayerTitleCore)) as SlayerTitleCore;

            if (core == null)
            {
                core = new SlayerTitleCore();

                SlayerTitleDef.InitializeConfiguration(core);

                core.Prep();
            }

            m_Core = core;

            CommandSystem.Register("STS", AccessLevel.Developer, new CommandEventHandler(STS_OnCommand));
        }

        // Called after all cores are loaded
        public override void Prep()
        {
            EventSink.OnKilledBy += EventSink_OnKilledBy;
        }

        [Usage("STS")]
        [Description("Displays the Slayer Title System configuration gump.")]
        public static void STS_OnCommand(CommandEventArgs e)
        {
            if (SlayerTitleCore.Core == null)
                return;

            e.Mobile.SendGump(new SlayerTitleSetupGump());
        }

        public void CrossReferenceDefinitions()
        {
            m_CrossReference = new Dictionary<Type, List<Int32>>();

            for (Int32 i = 0; i < m_TitleDefinitions.Count; i++)
            {
                TitleDefinition def = m_TitleDefinitions[i];

                if (def != null)
                {
                    foreach (Type t in def.CreatureRegistry)
                    {
                        if (m_CrossReference.ContainsKey(t))
                            m_CrossReference[t].Add(i);
                        else
                            m_CrossReference[t] = new List<int>() { i };
                    }
                }
            }
        }

        private void EventSink_OnKilledBy(OnKilledByEventArgs e)
        {
            if (!this.Enabled)
                return;

            BaseCreature creature = null;
            PlayerMobile player = null;

            if (e.Killed is BaseCreature)
                creature = (BaseCreature)e.Killed;

            if (e.KilledBy is PlayerMobile)
                player = (PlayerMobile)e.KilledBy;

            SlayerModule module = player.GetModule(typeof(SlayerModule)) as SlayerModule;

            if (module == null)
                module = new SlayerModule(player);

            if (m_CrossReference.ContainsKey(creature.GetType()))
            {
                foreach(Int32 index in m_CrossReference[creature.GetType()])
                {
                    if (index < m_TitleDefinitions.Count && m_TitleDefinitions[index].CreatureRegistry.Contains(creature.GetType()))
                    {
                        TitleDefinition def = m_TitleDefinitions[index];

                        module.IncrementCounter(def.DefinitionName);

                        Int32 value = module.GetSlayerCount(def.DefinitionName);
                        TitleEntry titleToSet = null;

                        foreach (TitleEntry entry in def.TitleRegistry)
                            if (entry.CountNeeded == value)
                                titleToSet = entry;

                        if (titleToSet != null)
                        {
                            foreach (TitleEntry entry in def.TitleRegistry)
                                if (player.CollectionTitles.Contains(entry.Title) && entry != titleToSet)
                                    player.CollectionTitles.Remove(entry.Title);

                            player.AddCollectionTitle(titleToSet.Title);
                            player.SendSound(0x3D);
                            player.SendMessage(0xC8, String.Format("Your have been awarded the title of '{0}' for {1} kills.", titleToSet.Title, value));

                            if (TitleAwarded != null)
                                TitleAwarded(player, def.DefinitionName, titleToSet.Title);

                            if (IsMaxTitle(titleToSet.Title, def.TitleRegistry) && MaximumTitleAchieved != null)
                                MaximumTitleAchieved(player, def.DefinitionName, titleToSet.Title);

                        }
                    }
                }
            }
        }

        private Boolean IsMaxTitle(String title, List<TitleEntry> entries)
        {
            Int32 maxCount = 0;
            Int32 titleCount = 0;

            foreach (TitleEntry entry in entries)
            {
                if (entry.CountNeeded > maxCount)
                    maxCount = entry.CountNeeded;

                if (entry.Title == title)
                    titleCount = entry.CountNeeded;
            }

            if (titleCount == maxCount)
                return true;
            else
                return false;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            Utilities.WriteVersion(writer, 1);

            writer.Write((Int32)m_TitleDefinitions.Count);

            foreach (TitleDefinition def in m_TitleDefinitions)
            {
                writer.Write((String)def.DefinitionName);
                writer.Write((Boolean)def.Enabled);

                writer.Write((Int32)def.CreatureRegistry.Count);

                foreach (Type t in def.CreatureRegistry)
                    writer.Write(t.Name.ToString());

                writer.Write((Int32)def.TitleRegistry.Count);

                foreach (TitleEntry entry in def.TitleRegistry)
                {
                    writer.Write((String)entry.Title);
                    writer.Write((Int32)entry.CountNeeded);
                }
            }
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            Int32 version = reader.ReadInt();

            switch (version)
            {
                case 1:
                    Int32 definitionCount = reader.ReadInt();

                    for (Int32 i = 0; i < definitionCount; i++)
                    {
                        String systemName = reader.ReadString();
                        Boolean enabled = reader.ReadBool();

                        List<Type> creatureRegistry = new List<Type>();
                        List<TitleEntry> titleRegistry = new List<TitleEntry>();

                        Int32 creatureCount = reader.ReadInt();

                        for (Int32 j = 0; j < creatureCount; j++)
                        {
                            Type t = ScriptCompiler.FindTypeByName(reader.ReadString());

                            if (t != null)
                                creatureRegistry.Add(t);
                        }

                        Int32 titleCount = reader.ReadInt();

                        for (Int32 j = 0; j < titleCount; j++)
                            titleRegistry.Add(new TitleEntry(reader.ReadString(), reader.ReadInt()));

                        m_TitleDefinitions.Add(new TitleDefinition(systemName, enabled, creatureRegistry, titleRegistry));
                    }

                    break;
                case 0:
                    SlayerTitleDef.InitializeConfiguration(this);
                    break;
            }

            CrossReferenceDefinitions();
        }
    }
}