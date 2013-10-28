using System;
using System.Collections.Generic;

namespace Server.Factions
{
    public class FactionPersistance : Item
    {
        private static FactionPersistance m_Instance;
        public FactionPersistance()
            : base(1)
        {
            this.Movable = false;

            if (m_Instance == null || m_Instance.Deleted)
                m_Instance = this;
            else
                base.Delete();
        }

        public FactionPersistance(Serial serial)
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
        public static FactionPersistance Instance
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

            writer.Write((int)0); // version

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
        }

        public override void Delete()
        {
        }
    }
}