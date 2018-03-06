using System;
using System.Collections.Generic;

namespace Server.Factions
{
    [TypeAlias("Server.Factions.FactionPersistance")]
    public class FactionPersistence : Item
    {
        private static FactionPersistence m_Instance;

        public FactionPersistence()
            : base(1)
        {
            this.Movable = false;

            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public FactionPersistence(Serial serial)
            : base(serial)
        {
            m_Instance = this;
        }

        private enum PersistedType
        {
            Terminator,
            Faction,
            Town
        }
        public static FactionPersistence Instance
        {
            get
            {
                return m_Instance;
            }
        }
        public override string DefaultName
        {
            get
            {
                return "Faction Persistance - Internal";
            }
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            // Version 0 removes faction items, ie monoliths, stones, etc
            writer.Write((int)1); // version

            List<Faction> factions = Faction.Factions;

            for (int i = 0; i < factions.Count; ++i)
            {
                writer.WriteEncodedInt((int)PersistedType.Faction);
                factions[i].State.Serialize(writer);
            }

            List<Town> towns = Town.Towns;

            for (int i = 0; i < towns.Count; ++i)
            {
                writer.WriteEncodedInt((int)PersistedType.Town);
                towns[i].State.Serialize(writer);
            }

            writer.WriteEncodedInt((int)PersistedType.Terminator);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                case 0:
                    {
                        PersistedType type;

                        while ((type = (PersistedType)reader.ReadEncodedInt()) != PersistedType.Terminator)
                        {
                            switch ( type )
                            {
                                case PersistedType.Faction:
                                    new FactionState(reader);
                                    break;
                                case PersistedType.Town:
                                    new TownState(reader);
                                    break;
                            }
                        }

                        break;
                    }
            }

            if (!Settings.Enabled && version == 0)
                Timer.DelayCall(TimeSpan.FromSeconds(10), () => Generator.RemoveFactions());
        }

        public override void Delete()
        {
        }
    }
}